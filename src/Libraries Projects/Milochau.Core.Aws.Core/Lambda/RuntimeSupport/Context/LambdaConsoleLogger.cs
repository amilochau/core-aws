using Microsoft.Extensions.Logging;
using Milochau.Core.Aws.Core.Lambda.Core;
using System;
using System.IO;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Context
{
    internal class LambdaConsoleLogger : ILambdaLogger
    {
        private readonly string? currentAwsRequestId;

        public LambdaConsoleLogger(string? currentAwsRequestId)
        {
            this.currentAwsRequestId = currentAwsRequestId;
        }

        public void Log(string message)
        {
            Console.Out.Write(message);
        }

        public void LogLine(LogLevel level, string message)
        {
            FormattedWriteLine(Console.Out, level, message);
        }

        public void LogLineError(LogLevel level, string message)
        {
            FormattedWriteLine(Console.Error, level, message);
        }

        private void FormattedWriteLine(TextWriter textWriter, LogLevel logLevel, string message)
        {
            if (logLevel < LogLevel.Information)
                return;

            var displayLevel = ConvertLogLevelToLabel(logLevel);
            var line = $"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ}\t{currentAwsRequestId}\t{displayLevel}\t{message ?? string.Empty}";

            textWriter.WriteLine(line);
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
                LogLevel.Trace => "trce",
                LogLevel.Debug => "dbug",
                LogLevel.Information => "info",
                LogLevel.Warning => "warn",
                LogLevel.Error => "fail",
                LogLevel.Critical => "crit",
                _ => level.ToString(),
            };
        }
    }
}
