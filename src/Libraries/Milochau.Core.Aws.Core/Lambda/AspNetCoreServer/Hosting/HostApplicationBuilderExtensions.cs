using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Logging;
using Milochau.Core.Aws.Core.Lambda.AspNetCoreServer.Internal;
using Milochau.Core.Aws.Core.Runtime.Credentials;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Milochau.Core.Aws.Abstractions;
using System.Linq;
using System.Security.Claims;

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

            // HTTP Context and identity
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped(serviceProvider =>
            {
                IHttpContextAccessor httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);
                var claims = httpContextAccessor.HttpContext.User.Claims;
                return new IdentityUser
                (
                    sub: claims.First(x => x.Type == ClaimTypes.NameIdentifier || x.Type == "sub").Value,
                    name: claims.First(x => x.Type == "name").Value,
                    email: claims.First(x => x.Type == ClaimTypes.Email || x.Type == "email").Value,
                    userId: Guid.Parse(claims.First(x => x.Type == "custom:user_id").Value)
                );
            });

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
