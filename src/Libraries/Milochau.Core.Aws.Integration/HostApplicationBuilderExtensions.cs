using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Milochau.Core.Aws.Core.Runtime.Credentials;
using Milochau.Core.Aws.Integration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Scalar.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using Milochau.Core.Aws.Core.References;
using System.Security.Claims;
using Milochau.Core.Aws.Abstractions;
using Microsoft.AspNetCore.Http.Json;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>Options used to configure application</summary>
    public class IntegrationWebApplicationOptions : WebApplicationOptions
    {
        /// <summary>CORS origins</summary>
        public required string[] CorsOrigins { get; set; }

        /// <summary>JSON options, used to configure the JSON serializer for HTTP API payloads</summary>
        public required Action<JsonOptions> JsonOptions { get; set; }
    }

    /// <summary>Web Application used for integration</summary>
    public static class IntegrationWebApplication
    {
        /// <summary>Creates a <see cref="WebApplicationBuilder"/></summary>
        public static WebApplicationBuilder CreateBuilder(IntegrationWebApplicationOptions options)
        {
            var builder = WebApplication.CreateBuilder(options);

            builder.Services.AddOutputCache(options =>
            {
                options.AddBasePolicy(policy =>
                {
                    policy.Expire(TimeSpan.FromMinutes(30));
                });
            });
            builder.Services.AddCors();
            builder.Services.AddSingleton<IAWSCredentials>(new AssumeRoleAWSCredentials(EnvironmentVariables.RoleArn));
            builder.Services.AddAuthentication().AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = false,
                    ValidateActor = false,
                    ValidateSignatureLast = false,
                    ValidateWithLKG = false,
                    ValidateTokenReplay = false,
                    RequireExpirationTime = false,
                    RequireSignedTokens = false,
                    SignatureValidator = (token, _) => new JsonWebToken(token),
                };
            });
            builder.Services.AddAuthorizationBuilder().SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });

            builder.Services.ConfigureHttpJsonOptions(options.JsonOptions);

            return builder;
        }

        /// <summary>Registers middlewares</summary>
        public static WebApplication UseIntegrationMiddlewares(this WebApplication app, IntegrationWebApplicationOptions options)
        {
            app.UseOutputCache();
            app.UseCors(builder =>
            {
                builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins(options.CorsOrigins);
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapOpenApi().CacheOutput().AllowAnonymous();

            app.MapScalarApiReference(options =>
            {
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                options.WithClientButton(false);
                options.WithHttpBearerAuthentication(bearer =>
                {
                    bearer.Token = string.Empty;
                });
            }).AllowAnonymous();

            return app;
        }

        /// <summary>Add middlewares for <c>Milochau.Core</c></summary>
        public static TApplication UseMilochauServices<TApplication>(this TApplication app, IntegrationWebApplicationOptions options)
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

    /// <summary>
    /// https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/openapi/customize-openapi?view=aspnetcore-9.0
    /// </summary>
    /// <param name="authenticationSchemeProvider"></param>
    internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
    {
        private readonly string AuthenticationSchemeName = "Bearer";

        public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
            if (authenticationSchemes.Any(authScheme => authScheme.Name == AuthenticationSchemeName))
            {
                // Add the security scheme at the document level
                var requirements = new Dictionary<string, OpenApiSecurityScheme>
                {
                    [AuthenticationSchemeName] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = AuthenticationSchemeName.ToLower(), // "bearer" refers to the header name here
                        In = ParameterLocation.Header,
                        BearerFormat = "JWT"
                    }
                };
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes = requirements;

                // Apply it as a requirement for all operations
                foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
                {
                    operation.Value.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = AuthenticationSchemeName,
                                Type = ReferenceType.SecurityScheme
                            }
                        }] = Array.Empty<string>()
                    });
                }

            }
        }
    }
}
