using Milochau.Core.Aws.Core.Lambda.AspNetCoreServer.Internal;
using Milochau.Core.Aws.Core.References;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>Extension methods to IServiceCollection</summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the ability to run the ASP.NET Core Lambda function in AWS Lambda. If the project is not running in Lambda 
        /// this method will do nothing allowing the normal Kestrel webserver to host the application.
        /// </summary>
        public static IServiceCollection AddAWSLambdaHosting(this IServiceCollection services)
        {
            // Not running in Lambda so exit and let Kestrel be the web server
            if (!string.IsNullOrEmpty(EnvironmentVariables.FunctionName))
            {
                Utilities.EnsureLambdaServerRegistered(services, typeof(LambdaServer));
            }

            return services;
        }
    }
}
