using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Logging;
using Milochau.Core.Aws.Core.Lambda.AspNetCoreServer.Internal;
using Milochau.Core.Aws.Core.Runtime.Credentials;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    public class AwsLambdaWebApplicationOptions : WebApplicationOptions
    {
        public required Action<JsonOptions> JsonOptions { get; set; }
    }

    public static class AwsLambdaWebApplication
    {
        public static WebApplicationBuilder CreateBuilder(AwsLambdaWebApplicationOptions options)
        {
            var builder = WebApplication.CreateEmptyBuilder(options);

            builder.Services.AddSingleton<IServer, LambdaServer>();
            builder.Services.AddRoutingCore();
            builder.Services.AddSingleton<IAWSCredentials>(new EnvironmentVariablesAWSCredentials());
            builder.Services.AddAuthenticationCore();
            builder.Services.ConfigureHttpJsonOptions(options.JsonOptions);
            builder.Services.AddAuthorizationBuilder().SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

            builder.Logging.AddLambdaLogger();

            return builder;
        }

        public static WebApplication UseAwsLambdaMiddlewares(this WebApplication app)
        {
            app.UseXRay();
            app.UseAuthorization();

            return app;
        }
    }
}
