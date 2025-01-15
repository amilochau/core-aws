using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Milochau.Core.Aws.Core.Lambda.Events;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Bootstrap;
using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Lambda.AspNetCoreServer.Internal
{
    /// <summary>
    /// IServer for handlying Lambda events from an API Gateway HTTP API.
    /// </summary>
    /// <param name="serviceProvider">The IServiceProvider created for the ASP.NET Core application</param>
    public class LambdaServer(IServiceProvider serviceProvider) : IServer
    {
        /// <summary>
        /// The application is used by the Lambda function to initiate a web request through the ASP.NET Core framework.
        /// </summary>
        public ApplicationWrapper? Application { get; set; }
        public IFeatureCollection Features { get; } = new FeatureCollection();

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Start Amazon.Lambda.RuntimeSupport to listen for Lambda events to be processed.
        /// </summary>
        public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
            where TContext : notnull
        {
            Application = new ApplicationWrapper<TContext>(application);

            var handler = new APIGatewayHttpApiV2ProxyFunction(serviceProvider, this).FunctionHandlerAsync;
            return LambdaBootstrap.RunAsync(handler, LambdaHostingJsonSerializerContext.Default.APIGatewayHttpApiV2ProxyRequest, LambdaHostingJsonSerializerContext.Default.APIGatewayHttpApiV2ProxyResponse);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public abstract class ApplicationWrapper
        {
            internal abstract object CreateContext(IFeatureCollection features);

            internal abstract Task ProcessRequestAsync(object context);

            internal abstract void DisposeContext(object context, Exception? exception);
        }

        public class ApplicationWrapper<TContext>(IHttpApplication<TContext> application) : ApplicationWrapper, IHttpApplication<TContext>
            where TContext : notnull
        {
            internal override object CreateContext(IFeatureCollection features)
            {
                return ((IHttpApplication<TContext>)this).CreateContext(features);
            }

            TContext IHttpApplication<TContext>.CreateContext(IFeatureCollection features)
            {
                return application.CreateContext(features);
            }

            internal override void DisposeContext(object context, Exception? exception)
            {
                ((IHttpApplication<TContext>)this).DisposeContext((TContext)context, exception);
            }

            void IHttpApplication<TContext>.DisposeContext(TContext context, Exception? exception)
            {
                application.DisposeContext(context, exception);
            }

            internal override Task ProcessRequestAsync(object context)
            {
                return ((IHttpApplication<TContext>)this).ProcessRequestAsync((TContext)context);
            }

            Task IHttpApplication<TContext>.ProcessRequestAsync(TContext context)
            {
                return application.ProcessRequestAsync(context);
            }
        }
    }

    [JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
    [JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, PropertyNameCaseInsensitive = true)]
    internal partial class LambdaHostingJsonSerializerContext : JsonSerializerContext
    {
    }
}
