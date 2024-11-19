using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.ExceptionHandling
{
    internal static class LambdaXRayExceptionWriter
    {
        public static string WriteJson(Exception exception)
        {
            var lambdaXRayException = new LambdaXRayException
            {
                WorkingDirectory = Directory.GetCurrentDirectory() ?? string.Empty,
                Exceptions = [
                    new LambdaXRayExceptionException
                    {
                        Type = exception.GetType().Name,
                        Message = exception.Message,
                        Stack = string.IsNullOrEmpty(exception.StackTrace) ? null : new StackTrace(exception, true).GetFrames().Select(x => new LambdaXRayExceptionExceptionStack
                        {
                            Path = x.GetFileName(),
                            Line = x.GetFileLineNumber(),
                        }),
                    }
                ],
                Paths = string.IsNullOrEmpty(exception.StackTrace) ? [] : new StackTrace(exception, true).GetFrames().Select(x => x.GetFileName()).Where(x => x != null).Distinct(),
            };
            return JsonSerializer.Serialize(lambdaXRayException, LambdaXRayExceptionWriterSerializerContext.Default.LambdaXRayException);
        }
    }

    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(LambdaXRayException))]
    internal partial class LambdaXRayExceptionWriterSerializerContext : JsonSerializerContext
    {
    }

    internal class LambdaXRayException
    {
        [JsonPropertyName("working_directory")]
        public required string WorkingDirectory { get; init; }

        [JsonPropertyName("exceptions")]
        public required IEnumerable<LambdaXRayExceptionException> Exceptions { get; init; }

        [JsonPropertyName("paths")]
        public required IEnumerable<string?> Paths { get; init; }
    }

    internal class LambdaXRayExceptionException
    {
        [JsonPropertyName("type")]
        public required string Type { get; init; }

        [JsonPropertyName("message")]
        public required string Message { get; init; }

        [JsonPropertyName("stack")]
        public required IEnumerable<LambdaXRayExceptionExceptionStack>? Stack { get; init; }
    }

    internal class LambdaXRayExceptionExceptionStack
    {
        [JsonPropertyName("path")]
        public required string? Path { get; init; }

        [JsonPropertyName("line")]
        public required int Line { get; init; }
    }
}
