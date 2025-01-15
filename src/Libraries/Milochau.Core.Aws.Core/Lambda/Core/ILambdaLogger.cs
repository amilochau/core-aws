using Microsoft.Extensions.Logging;

namespace Milochau.Core.Aws.Core.Lambda.Core
{
    /// <summary>
    /// Lambda runtime logger.
    /// </summary>
    public interface ILambdaLogger
    {
        /// <summary>
        /// Logs a message, followed by the current line terminator, to AWS CloudWatch Logs.
        /// </summary>
        void LogLine(LogLevel logLevel, string message);
    }
}