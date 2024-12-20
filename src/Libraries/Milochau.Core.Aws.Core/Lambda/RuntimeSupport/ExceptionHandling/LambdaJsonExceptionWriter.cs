using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.ExceptionHandling
{
    internal static class LambdaJsonExceptionWriter
    {
        private const int MAX_PAYLOAD_SIZE = 256 * 1024; // 256KB
        private const string TRUNCATED_MESSAGE = "{\"errorMessage\": \"Exception exceeded maximum payload size of 256KB.\"}";

        public static string WriteJson(Exception exception)
        {
            var lambdaJsonException = Build(exception);
            var json = JsonSerializer.Serialize(lambdaJsonException, LambdaJsonExceptionWriterSerializerContext.Default.LambdaJsonException);
            if (Encoding.UTF8.GetByteCount(json) > MAX_PAYLOAD_SIZE)
            {
                return TRUNCATED_MESSAGE;
            }
            else
            {
                return json;
            }
        }

        private static LambdaJsonException Build(Exception exception)
        {
            return new LambdaJsonException
            {
                Type = exception.GetType().Name,
                Message = exception.Message,
                StackTrace = string.IsNullOrEmpty(exception.StackTrace) ? null : new StackTrace(exception, true).ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                Cause = exception.InnerException == null ? null : Build(exception.InnerException),
            };
        }
    }

    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(LambdaJsonException))]
    internal partial class LambdaJsonExceptionWriterSerializerContext : JsonSerializerContext
    {
    }

    internal class LambdaJsonException
    {
        [JsonPropertyName("errorType")]
        public required string Type { get; init; }

        [JsonPropertyName("errorMessage")]
        public required string Message { get; init; }

        [JsonPropertyName("stackTrace")]
        public required IEnumerable<string>? StackTrace { get; init; }

        [JsonPropertyName("cause")]
        public required LambdaJsonException? Cause { get; init; }
    }
}
