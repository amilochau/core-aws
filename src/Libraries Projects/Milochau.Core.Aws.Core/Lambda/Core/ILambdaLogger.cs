namespace Amazon.Lambda.Core
{
    /// <summary>
    /// Log Level for logging messages
    /// </summary>
    public enum LogLevel 
    {
        /// <summary>
        /// Trace level logging
        /// </summary>
        Trace = 0,
        /// <summary>
        /// Debug level logging
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Information level logging
        /// </summary>
        Information = 2,

        /// <summary>
        /// Warning level logging
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Error level logging
        /// </summary>
        Error = 4,

        /// <summary>
        /// Critical level logging
        /// </summary>
        Critical = 5
    }

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
        void Log(string level, string message) => LogLine(message);

        /// <summary>
        /// Log error message
        /// <para>
        /// To configure the minimum log level set the AWS_LAMBDA_HANDLER_LOG_LEVEL environment variable. The value should be set
        /// to one of the values in the LogLevel enumeration. The default minimum log level is "Information".
        /// </para>
        /// </summary>
        /// <param name="message"></param>
        void LogError(string message) => Log(LogLevel.Error.ToString(), message);
    }
}