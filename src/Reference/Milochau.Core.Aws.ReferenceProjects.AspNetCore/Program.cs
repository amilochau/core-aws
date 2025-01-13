using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Milochau.Core.Aws.Core.JsonConverters;
using System.Text.Json;
using System.Text.Json.Serialization;

#if DEBUG

var builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions
{
    Args = args,
    ApplicationName = "Maps",
    EnvironmentName = "Development",
});

//builder.Services.AddSingleton<IAWSCredentials>(new AssumeRoleAWSCredentials(Environment.GetEnvironmentVariable("AWS_ROLE_ARN")!));

#else

var builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions
{
    Args = args,
    ApplicationName = "Maps",
    EnvironmentName = "Production",
});

//builder.Services.AddSingleton<IAWSCredentials>(new Milochau.Core.Aws.Core.Runtime.Credentials.EnvironmentVariablesAWSCredentials());

#endif

builder.AddAWSLambdaHosting();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.Converters.Add(new GuidConverter());
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApplicationJsonSerializerContext.Default);
});

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorizationBuilder().SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

var app = builder.Build();

app.UseXRay();
app.UseAuthorization();

app.Run();

// Add here `[JsonSerializable] for Minimal API models`
[JsonSerializable(typeof(string))]
internal partial class ApplicationJsonSerializerContext : JsonSerializerContext
{
}
