using Milochau.Core.Aws.Core.References;

namespace Milochau.Core.Aws.Core.Lambda.Core
{
    /// <summary>
    /// Lambda runtime logger.
    /// </summary>
    public interface ILambdaLogger
    {
        /// <summary>
        /// Logs a message to AWS CloudWatch Logs.
        /// 
        /// Logging will not be done:
        ///  If the role provided to the function does not have sufficient permissions.
        /// </summary>
        /// <param name="message"></param>
        void Log(string message);

        /// <summary>
        /// Logs a message, followed by the current line terminator, to AWS CloudWatch Logs.
        /// 
        /// Logging will not be done:
        ///  If the role provided to the function does not have sufficient permissions.
        /// </summary>
        /// <param name="message"></param>
        void LogLine(string message);

        /// <summary>
        /// Log message catagorized by the given log level
        /// <para>
        /// To configure the minimum log level set the AWS_LAMBDA_HANDLER_LOG_LEVEL environment variable. The value should be set
        /// to one of the values in the LogLevel enumeration. The default minimum log level is "Information".
        /// </para>
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        void Log(LogLevel level, string message) => LogLine(message);

        /// <summary>
        /// Log error message
        /// <para>
        /// To configure the minimum log level set the AWS_LAMBDA_HANDLER_LOG_LEVEL environment variable. The value should be set
        /// to one of the values in the LogLevel enumeration. The default minimum log level is "Information".
        /// </para>
        /// </summary>
        /// <param name="message"></param>
        void LogError(string message) => Log(LogLevel.Error, message);
    }
}