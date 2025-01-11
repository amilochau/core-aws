using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Milochau.Core.Aws.Core.Lambda.AspNetCoreServer.Internal;
using Milochau.Core.Aws.Core.Lambda.Core;
using Milochau.Core.Aws.Core.Lambda.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Milochau.Core.Aws.Core.Lambda.AspNetCoreServer
{
    /// <summary>Base class for ASP.NET Core Lambda functions that are getting request from API Gateway HTTP API V2 payload format</summary>
    public class APIGatewayHttpApiV2ProxyFunction
    {
        private IServiceProvider serviceProvider;
        private LambdaServer? lambdaServer;
        private ILogger logger;
        
        // Defines a mapping from registered content types to the response encoding format
        // which dictates what transformations should be applied before returning response content
        private readonly Dictionary<string, ResponseContentEncoding> _responseContentEncodingForContentType = new Dictionary<string, ResponseContentEncoding>
        {
            // The complete list of registered MIME content-types can be found at:
            //    http://www.iana.org/assignments/media-types/media-types.xhtml

            // Here we just include a few commonly used content types found in
            // Web API responses and allow users to add more as needed below

            ["text/plain"] = ResponseContentEncoding.Default,
            ["text/xml"] = ResponseContentEncoding.Default,
            ["application/xml"] = ResponseContentEncoding.Default,
            ["application/json"] = ResponseContentEncoding.Default,
            ["text/html"] = ResponseContentEncoding.Default,
            ["text/css"] = ResponseContentEncoding.Default,
            ["text/javascript"] = ResponseContentEncoding.Default,
            ["text/ecmascript"] = ResponseContentEncoding.Default,
            ["text/markdown"] = ResponseContentEncoding.Default,
            ["text/csv"] = ResponseContentEncoding.Default,

            ["application/octet-stream"] = ResponseContentEncoding.Base64,
            ["image/png"] = ResponseContentEncoding.Base64,
            ["image/gif"] = ResponseContentEncoding.Base64,
            ["image/jpeg"] = ResponseContentEncoding.Base64,
            ["image/jpg"] = ResponseContentEncoding.Base64,
            ["image/x-icon"] = ResponseContentEncoding.Base64,
            ["application/zip"] = ResponseContentEncoding.Base64,
            ["application/pdf"] = ResponseContentEncoding.Base64,
            ["application/x-protobuf"] = ResponseContentEncoding.Base64,
            ["application/wasm"] = ResponseContentEncoding.Base64
        };

        // Defines a mapping from registered content encodings to the response encoding format
        // which dictates what transformations should be applied before returning response content
        private readonly Dictionary<string, ResponseContentEncoding> _responseContentEncodingForContentEncoding = new Dictionary<string, ResponseContentEncoding>
        {
            ["gzip"] = ResponseContentEncoding.Base64,
            ["deflate"] = ResponseContentEncoding.Base64,
            ["br"] = ResponseContentEncoding.Base64
        };

        /// <summary>Constructor used by Amazon.Lambda.AspNetCoreServer.Hosting to support ASP.NET Core projects using the Minimal API style</summary>
        public APIGatewayHttpApiV2ProxyFunction(IServiceProvider hostedServices)
        {
            serviceProvider = hostedServices;
            lambdaServer = this.serviceProvider.GetService(typeof(Microsoft.AspNetCore.Hosting.Server.IServer)) as LambdaServer;
            logger = ActivatorUtilities.CreateInstance<Logger<APIGatewayHttpApiV2ProxyFunction>>(this.serviceProvider);

        }

        /// <summary>Defines the default treatment of response content</summary>
        public ResponseContentEncoding DefaulAPIGatewayHttpApiV2ProxyResponseContentEncoding { get; set; } = ResponseContentEncoding.Default;

        /// <summary>
        /// If true, information about unhandled exceptions thrown during request processing
        /// will be included in the HTTP response.
        /// </summary>
        public bool IncludeUnhandledExceptionDetailInResponse { get; set; }

        private protected bool IsStarted => lambdaServer != null;

        /// <summary>Should be called in the derived constructor</summary>
        protected void Start()
        {
            var builder = Host
                .CreateDefaultBuilder()
                .ConfigureWebHostLambdaDefaults()
                .ConfigureServices(Utilities.EnsureLambdaServerRegistered);

            var host = builder.Build();

            host.Start();
            this.serviceProvider = host.Services;

            lambdaServer = this.serviceProvider.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>() as LambdaServer;
            if (lambdaServer == null)
            {
                throw new Exception("Failed to find the Lambda implementation for the IServer interface in the IServiceProvider for the Host. This happens if UseLambdaServer was " +
                        "not called when constructing the IWebHostBuilder. If CreateHostBuilder was overridden it is recommended that ConfigureWebHostLambdaDefaults should be used " +
                        "instead of ConfigureWebHostDefaults to make sure the property Lambda services are registered.");
            }
            logger = ActivatorUtilities.CreateInstance<Logger<APIGatewayHttpApiV2ProxyFunction>>(this.serviceProvider);
        }

        /// <summary>Creates a context object using the <see cref="LambdaServer"/> field in the class</summary>
        protected object CreateContext(IFeatureCollection features)
        {
            return lambdaServer!.Application!.CreateContext(features);
        }

        /// <summary>Gets the response content encoding for a content type</summary>
        public ResponseContentEncoding GeAPIGatewayHttpApiV2ProxyResponseContentEncodingForContentType(string? contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return DefaulAPIGatewayHttpApiV2ProxyResponseContentEncoding;
            }

            // ASP.NET Core will typically return content type with encoding like this "application/json; charset=utf-8"
            // To find the content type in the dictionary we need to strip the encoding off.
            var contentTypeWithoutEncoding = contentType.Split(';')[0].Trim();
            if (_responseContentEncodingForContentType.TryGetValue(contentTypeWithoutEncoding, out var encoding))
            {
                return encoding;
            }

            return DefaulAPIGatewayHttpApiV2ProxyResponseContentEncoding;
        }

        /// <summary>Gets the response content encoding for a content encoding</summary>
        public ResponseContentEncoding GeAPIGatewayHttpApiV2ProxyResponseContentEncodingForContentEncoding(string? contentEncoding)
        {
            if (string.IsNullOrEmpty(contentEncoding))
            {
                return DefaulAPIGatewayHttpApiV2ProxyResponseContentEncoding;
            }

            if (_responseContentEncodingForContentEncoding.TryGetValue(contentEncoding, out var encoding))
            {
                return encoding;
            }

            return DefaulAPIGatewayHttpApiV2ProxyResponseContentEncoding;
        }

        /// <summary>Formats an Exception into a string, including all inner exceptions</summary>
        protected string ErrorReport(Exception e)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{e.GetType().Name}:\n{e}");

            Exception? inner = e;
            while (inner != null)
            {
                // Append the messages to the StringBuilder.
                sb.AppendLine($"{inner.GetType().Name}:\n{inner}");
                inner = inner.InnerException;
            }

            return sb.ToString();
        }

        public virtual async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandlerAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext lambdaContext, CancellationToken cancellationToken)
        {
            if (!IsStarted)
            {
                Start();
            }

            var features = new InvokeFeatures(request, serviceProvider, lambdaContext);

            var scope = this.serviceProvider.CreateScope();
            try
            {
                ((IServiceProvidersFeature)features).RequestServices = scope.ServiceProvider;

                var context = this.CreateContext(features);
                var response = await this.ProcessRequest(lambdaContext, context, features);

                return response;
            }
            finally
            {
                scope.Dispose();
            }
        }

        /// <summary>Processes the current request</summary>
        /// <param name="lambdaContext"><see cref="ILambdaContext"/> implementation.</param>
        /// <param name="context">The hosting application request context object.</param>
        /// <param name="features">An <see cref="InvokeFeatures"/> instance.</param>
        /// <param name="rethrowUnhandledError">
        /// If specified, an unhandled exception will be rethrown for custom error handling.
        /// Ensure that the error handling code calls 'this.MarshallResponse(features, 500);' after handling the error to return a the typed Lambda object to the user.
        /// </param>
        protected async Task<APIGatewayHttpApiV2ProxyResponse> ProcessRequest(ILambdaContext lambdaContext, object context, InvokeFeatures features, bool rethrowUnhandledError = false)
        {
            var defaultStatusCode = 200;
            Exception? ex = null;
            try
            {
                try
                {
                    await this.lambdaServer!.Application!.ProcessRequestAsync(context);
                }
                catch (AggregateException agex)
                {
                    ex = agex;
                    logger.LogError(agex, $"Caught AggregateException: '{agex}'");
                    var sb = new StringBuilder();
                    foreach (var newEx in agex.InnerExceptions)
                    {
                        sb.AppendLine(this.ErrorReport(newEx));
                    }

                    logger.LogError(sb.ToString());
                    ((IHttpResponseFeature)features).StatusCode = 500;
                }
                catch (ReflectionTypeLoadException rex)
                {
                    ex = rex;
                    logger.LogError(rex, $"Caught ReflectionTypeLoadException: '{rex}'");
                    var sb = new StringBuilder();
                    foreach (var loaderException in rex.LoaderExceptions)
                    {
                        var fileNotFoundException = loaderException as FileNotFoundException;
                        if (fileNotFoundException != null && !string.IsNullOrEmpty(fileNotFoundException.FileName))
                        {
                            sb.AppendLine($"Missing file: {fileNotFoundException.FileName}");
                        }
                        else if (loaderException != null)
                        {
                            sb.AppendLine(this.ErrorReport(loaderException));
                        }
                    }

                    logger.LogError(sb.ToString());
                    ((IHttpResponseFeature)features).StatusCode = 500;
                }
                catch (Exception e)
                {
                    ex = e;
                    if (rethrowUnhandledError) throw;
                    logger.LogError(e, $"Unknown error responding to request: {this.ErrorReport(e)}");
                    ((IHttpResponseFeature)features).StatusCode = 500;
                }

                if (features.ResponseStartingEvents != null)
                {
                    await features.ResponseStartingEvents.ExecuteAsync();
                }
                var response = this.MarshallResponse(features, lambdaContext, defaultStatusCode);

                if (ex != null && IncludeUnhandledExceptionDetailInResponse)
                {
                    InternalCustomResponseExceptionHandling(response, lambdaContext, ex);
                }

                if (features.ResponseCompletedEvents != null)
                {
                    await features.ResponseCompletedEvents.ExecuteAsync();
                }

                return response;
            }
            finally
            {
                this.lambdaServer!.Application!.DisposeContext(context, ex);
            }
        }





        private protected void InternalCustomResponseExceptionHandling(APIGatewayHttpApiV2ProxyResponse apiGatewayResponse, ILambdaContext lambdaContext, Exception ex)
        {
            apiGatewayResponse.SetHeaderValues("ErrorType", [ex.GetType().Name], false);
        }

        /// <summary>
        /// Convert the response coming from ASP.NET Core into APIGatewayProxyResponse which is
        /// serialized into the JSON object that API Gateway expects.
        /// </summary>
        /// <param name="responseFeatures"></param>
        /// <param name="statusCodeIfNotSet">Sometimes the ASP.NET server doesn't set the status code correctly when successful, so this parameter will be used when the value is 0.</param>
        /// <param name="lambdaContext"></param>
        protected APIGatewayHttpApiV2ProxyResponse MarshallResponse<TFeature>(TFeature responseFeatures, ILambdaContext lambdaContext, int statusCodeIfNotSet = 200)
            where TFeature : IHttpResponseFeature, IHttpResponseBodyFeature
        {
            var response = new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = responseFeatures.StatusCode != 0 ? responseFeatures.StatusCode : statusCodeIfNotSet
            };

            string? contentType = null;
            string? contentEncoding = null;
            if (responseFeatures.Headers != null)
            {
                response.Headers = new Dictionary<string, string?>();
                foreach (var kvp in responseFeatures.Headers)
                {
                    if (kvp.Key.Equals(HeaderNames.SetCookie, StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Cookies must be passed through the proxy response property and not as a 
                        // header to be able to pass back multiple cookies in a single request.
                        response.Cookies = kvp.Value.ToArray();
                        continue;
                    }

                    response.SetHeaderValues(kvp.Key, kvp.Value.ToArray(), false);

                    // Remember the Content-Type for possible later use
                    if (kvp.Key.Equals("Content-Type", StringComparison.CurrentCultureIgnoreCase) && response.Headers[kvp.Key]?.Length > 0)
                    {
                        contentType = response.Headers[kvp.Key];
                    }
                    else if (kvp.Key.Equals("Content-Encoding", StringComparison.CurrentCultureIgnoreCase) && response.Headers[kvp.Key]?.Length > 0)
                    {
                        contentEncoding = response.Headers[kvp.Key];
                    }
                }
            }

            if (contentType == null)
            {
                response.Headers["Content-Type"] = null;
            }

            if (responseFeatures.Stream != null)
            {
                // Figure out how we should treat the response content, check encoding first to see if body is compressed, then check content type
                var rcEncoding = GeAPIGatewayHttpApiV2ProxyResponseContentEncodingForContentEncoding(contentEncoding);
                if (rcEncoding != ResponseContentEncoding.Base64)
                {
                    rcEncoding = GeAPIGatewayHttpApiV2ProxyResponseContentEncodingForContentType(contentType);
                }

                (response.Body, response.IsBase64Encoded) = Utilities.ConvertAspNetCoreBodyToLambdaBody(responseFeatures.Stream, rcEncoding);

            }

            return response;
        }
    }
}
