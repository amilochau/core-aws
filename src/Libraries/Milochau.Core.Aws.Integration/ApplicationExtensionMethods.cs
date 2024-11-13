using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Milochau.Core.Aws.Integration
{
    /// <summary>Extension methods used when configuring application</summary>
    public static class ApplicationExtensionMethods
    {
        /// <summary>Add services for <c>Milochau.Core</c></summary>
        public static IServiceCollection AddMilochauServices(this IServiceCollection services)
        {
            services.AddOutputCache(options =>
            {
                options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromMinutes(30)));
            });
            services.AddOpenApi();
            services.AddCors();

            return services;
        }

        /// <summary>Add middlewares for <c>Milochau.Core</c></summary>
        public static TApplication UseMilochauServices<TApplication>(this TApplication app, ApplicationOptions options)
            where TApplication: IApplicationBuilder, IEndpointRouteBuilder
        {
            app.UseOutputCache();
            app.MapOpenApi().CacheOutput();

            app.UseCors(builder =>
            {
                builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins(options.CorsOrigins);
            });

            app.MapGet("/", () => Results.Redirect("/openapi/v1.json")).ExcludeFromDescription();

            return app;
        }
    }

    /// <summary>Options used to configure application</summary>
    public class ApplicationOptions
    {
        /// <summary>CORS origins</summary>
        public required string[] CorsOrigins { get; set; }
    }
}
