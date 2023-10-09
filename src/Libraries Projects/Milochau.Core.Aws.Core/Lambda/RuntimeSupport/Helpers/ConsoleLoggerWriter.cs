using Milochau.Core.Aws.Core.Lambda.Core;
using System;
using System.IO;
using System.Text;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Helpers
{
    /// <summary>
    /// Interface used by bootstrap to format logging message as well as Console WriteLine messages.
    /// </summary>
    public interface IConsoleLoggerWriter
    {
        /// <summary>
        /// The current aws request id
        /// </summary>
        /// <param name="awsRequestId"></param>
        void SetCurrentAwsRequestId(string awsRequestId);

        /// <summary>
        /// Format message with default log level
        /// </summary>
        /// <param name="message"></param>
        void FormattedWriteLine(string message);

        /// <summary>
        /// Format message with given log level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        void FormattedWriteLine(string level, string message);
    }

    /// <summary>
    /// Formats log messages with time, request id, log level and message
    /// </summary>
    public class LogLevelLoggerWriter : IConsoleLoggerWriter
    {
        /// <summary>
        /// A mirror of the LogLevel defined in Amazon.Lambda.Core. The version in
        /// Amazon.Lambda.Core can not be relied on because the Lambda Function could be using
        /// an older version of Amazon.Lambda.Core before LogLevel existed in Amazon.Lambda.Core.
        /// </summary>
        enum LogLevel
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

        WrapperTextWriter _wrappedStdOutWriter;
        WrapperTextWriter _wrappedStdErrorWriter;

        /// <summary>
        /// Constructor used by bootstrap to put in place a wrapper TextWriter around stdout and stderror so all Console.WriteLine calls
        /// will be formatted.
        ///
        /// Stdout will default log messages to be Information
        /// Stderror will default log messages to be Error
        /// </summary>
        public LogLevelLoggerWriter()
        {
            Initialize(Console.Out, Console.Error);

            // SetOut will wrap our WrapperTextWriter with a synchronized TextWriter. Pass in the new synchronized
            // TextWriter into our writer to make sure we obtain a lock on that instance before writing to the stdout.
            Console.SetOut(_wrappedStdOutWriter);
            _wrappedStdOutWriter.LockObject = Console.Out;

            Console.SetError(_wrappedStdErrorWriter);
            _wrappedStdErrorWriter.LockObject = Console.Error;

            ConfigureLoggingActionField();
        }

        private void Initialize(TextWriter stdOutWriter, TextWriter stdErrorWriter)
        {
            _wrappedStdOutWriter = new WrapperTextWriter(stdOutWriter, LogLevel.Information.ToString());
            _wrappedStdErrorWriter = new WrapperTextWriter(stdErrorWriter, LogLevel.Error.ToString());
        }

        /// <summary>
        /// Set a special callback on Amazon.Lambda.Core.LambdaLogger to redirect its logging to FormattedWriteLine.
        /// This allows outputting logging with time and request id but not have LogLevel. This is important for
        /// Amazon.Lambda.Logging.AspNetCore which already provides a string with a log level.
        /// </summary>
        private void ConfigureLoggingActionField()
        {
            LambdaLogger._loggingAction = message => FormattedWriteLine(null, message);
        }

        public void SetCurrentAwsRequestId(string awsRequestId)
        {
            _wrappedStdOutWriter.CurrentAwsRequestId = awsRequestId;
            _wrappedStdErrorWriter.CurrentAwsRequestId = awsRequestId;
        }

        public void FormattedWriteLine(string message)
        {
            _wrappedStdOutWriter.FormattedWriteLine(message);
        }

        public void FormattedWriteLine(string level, string message)
        {
            _wrappedStdOutWriter.FormattedWriteLine(level, message);
        }


        /// <summary>
        /// Wraps around a provided TextWriter. In normal usage the wrapped TextWriter will either be stdout or stderr.
        /// For all calls besides WriteLine and WriteLineAsync call into the wrapped TextWriter. For the WriteLine and WriteLineAsync
        /// format the message with time, request id, log level and the provided message.
        /// </summary>
        class WrapperTextWriter : TextWriter
        {
            private readonly TextWriter _innerWriter;
            private string _defaultLogLevel;

            const string LOG_LEVEL_ENVIRONMENT_VARIABLE = "AWS_LAMBDA_HANDLER_LOG_LEVEL";
            const string LOG_FORMAT_ENVIRONMENT_VARIABLE = "AWS_LAMBDA_HANDLER_LOG_FORMAT";

            private LogLevel _minmumLogLevel = LogLevel.Information;

            enum LogFormatType { Default, Unformatted }

            private LogFormatType _logFormatType = LogFormatType.Default;

            public string CurrentAwsRequestId { get; set; } = string.Empty;

            /// <summary>
            /// This is typically set to either Console.Out or Console.Error to make sure we acquiring a lock
            /// on that object whenever we are going through FormattedWriteLine. This is important for
            /// logging that goes through ILambdaLogger that skips going through Console.WriteX. Without
            /// this ILambdaLogger only acquires one lock but Console.WriteX acquires 2 locks and we can get deadlocks.
            /// </summary>
            internal object LockObject { get; set; } = new object();

            public WrapperTextWriter(TextWriter innerWriter, string defaultLogLevel)
            {
                _innerWriter = innerWriter;
                _defaultLogLevel = defaultLogLevel;

                var envLogLevel = Environment.GetEnvironmentVariable(LOG_LEVEL_ENVIRONMENT_VARIABLE);
                if (!string.IsNullOrEmpty(envLogLevel))
                {
                    if (Enum.TryParse<LogLevel>(envLogLevel, true, out var result))
                    {
                        _minmumLogLevel = result;
                    }
                }

                var envLogFormat = Environment.GetEnvironmentVariable(LOG_FORMAT_ENVIRONMENT_VARIABLE);
                if (!string.IsNullOrEmpty(envLogFormat))
                {
                    if (Enum.TryParse<LogFormatType>(envLogFormat, true, out var result))
                    {
                        _logFormatType = result;
                    }
                }
            }

            internal void FormattedWriteLine(string message)
            {
                FormattedWriteLine(_defaultLogLevel, message);
            }

            internal void FormattedWriteLine(string level, string message)
            {
                lock(LockObject)
                {
                    var displayLevel = level;
                    if (Enum.TryParse<LogLevel>(level, true, out var levelEnum))
                    {
                        if (levelEnum < _minmumLogLevel)
                            return;

                        displayLevel = ConvertLogLevelToLabel(levelEnum);
                    }

                    if (_logFormatType == LogFormatType.Unformatted)
                    {
                        _innerWriter.WriteLine(message);
                    }
                    else
                    {
                        string line;
                        if (!string.IsNullOrEmpty(displayLevel))
                        {
                            line = $"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}\t{CurrentAwsRequestId}\t{displayLevel}\t{message ?? string.Empty}";
                        }
                        else
                        {
                            line = $"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}\t{CurrentAwsRequestId}\t{message ?? string.Empty}";
                        }

                        _innerWriter.WriteLine(line);
                    }
                }
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
            public override Encoding Encoding => _innerWriter.Encoding;
        }
    }
}
