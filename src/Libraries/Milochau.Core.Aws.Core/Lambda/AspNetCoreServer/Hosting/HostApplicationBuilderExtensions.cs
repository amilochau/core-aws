using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Milochau.Core.Aws.Core.Lambda.AspNetCoreServer.Internal;
using Milochau.Core.Aws.Core.References;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>Extension methods to IServiceCollection</summary>
    public static class HostApplicationBuilderExtensions
    {
        /// <summary>
        /// Add the ability to run the ASP.NET Core Lambda function in AWS Lambda. If the project is not running in Lambda 
        /// this method will do nothing allowing the normal Kestrel webserver to host the application.
        /// </summary>
        public static IHostApplicationBuilder AddAWSLambdaHosting(this IHostApplicationBuilder builder)
        {
            // Not running in Lambda so exit and let Kestrel be the web server
            if (!string.IsNullOrEmpty(EnvironmentVariables.FunctionName))
            {
                builder.Services.RemoveAll<IServer>();
                builder.Services.AddSingleton<IServer, LambdaServer>();

                builder.Logging.ClearProviders();
                builder.Logging.AddLambdaLogger();
            }

            return builder;
        }
    }
}
