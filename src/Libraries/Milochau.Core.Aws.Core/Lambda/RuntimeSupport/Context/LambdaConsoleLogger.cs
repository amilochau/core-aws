using Microsoft.Extensions.Logging;
using Milochau.Core.Aws.Core.Lambda.Core;
using Milochau.Core.Aws.Core.References;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Context
{
    internal class LambdaConsoleLogger(string currentAwsRequestId) : ILambdaLogger
    {
        protected static LogLevel MinimumLogLevel { get; } = EnvironmentVariables.LogLevel switch
        {
            "TRACE" => LogLevel.Trace,
            "DEBUG" => LogLevel.Debug,
            "INFO" => LogLevel.Information,
            "WARN" => LogLevel.Warning,
            "ERROR" => LogLevel.Error,
            "FATAL" => LogLevel.Critical,
            _ => LogLevel.Information,
        };

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= MinimumLogLevel && logLevel != LogLevel.None;
        }

        public void LogLine(LogLevel logLevel, string message)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var displayLevel = ConvertLogLevelToLabel(logLevel);

            // We assume that we always want to use JSON log format
            var formattedLine = new LogLine(displayLevel, message, currentAwsRequestId);
            var stringifiedLine = JsonSerializer.Serialize(formattedLine, LoggerJsonSerializerContext.Default.LogLine);

            Console.WriteLine(stringifiedLine);
        }

        /// <summary>
        /// Convert LogLevel enums to the the same string label that console provider for Microsoft.Extensions.Logging.ILogger uses.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static string ConvertLogLevelToLabel(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => "TRACE",
                LogLevel.Debug => "DEBUG",
                LogLevel.Information => "INFO",
                LogLevel.Warning => "WARN",
                LogLevel.Error => "ERROR",
                LogLevel.Critical => "FATAL",
                _ => level.ToString(),
            };
        }
    }

    internal class LogLine(string logLevel, string message, string requestId)
    {
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public string Level { get; } = logLevel;
        public string Message { get; } = message;
        public string RequestId { get; } = requestId;
        public string FunctionName { get; } = EnvironmentVariables.FunctionName;
    }

    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(LogLine))]
    internal partial class LoggerJsonSerializerContext : JsonSerializerContext
    {
    }
}
