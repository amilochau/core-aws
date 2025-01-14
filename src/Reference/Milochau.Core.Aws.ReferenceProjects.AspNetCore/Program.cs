using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Milochau.Core.Aws.Core.JsonConverters;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = AwsLambdaWebApplication.CreateBuilder(new AwsLambdaWebApplicationOptions
{
    Args = args,
    ApplicationName = "Test",
    EnvironmentName = "Production",
    JsonOptions = options =>
    {
        options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.SerializerOptions.Converters.Add(new GuidConverter());
        options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApplicationJsonSerializerContext.Default);
    }
});

var app = builder.Build();

app.Map("/", static () => TypedResults.Ok("OK.")).AllowAnonymous();

app.Run();

// Add here `[JsonSerializable] for Minimal API models`
[JsonSerializable(typeof(string))]
internal partial class ApplicationJsonSerializerContext : JsonSerializerContext
{
}
