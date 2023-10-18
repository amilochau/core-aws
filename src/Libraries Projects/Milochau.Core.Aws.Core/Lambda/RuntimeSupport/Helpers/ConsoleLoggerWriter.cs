using Milochau.Core.Aws.Core.Lambda.Core;
using Milochau.Core.Aws.Core.References;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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
        void SetCurrentAwsRequestId(string awsRequestId);

        /// <summary>
        /// Format message with default log level
        /// </summary>
        void FormattedWriteLine(string message);

        /// <summary>
        /// Format message with given log level
        /// </summary>
        void FormattedWriteLine(LogLevel? level, string message);
    }

    /// <summary>
    /// Formats log messages with time, request id, log level and message
    /// </summary>
    public class LogLevelLoggerWriter : IConsoleLoggerWriter
    {
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
            _wrappedStdOutWriter = new WrapperTextWriter(stdOutWriter, LogLevel.Information);
            _wrappedStdErrorWriter = new WrapperTextWriter(stdErrorWriter, LogLevel.Error);
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

        public void FormattedWriteLine(LogLevel? level, string message)
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
            private readonly LogLevel _defaultLogLevel;

            public string CurrentAwsRequestId { get; set; } = string.Empty;

            /// <summary>
            /// This is typically set to either Console.Out or Console.Error to make sure we acquiring a lock
            /// on that object whenever we are going through FormattedWriteLine. This is important for
            /// logging that goes through ILambdaLogger that skips going through Console.WriteX. Without
            /// this ILambdaLogger only acquires one lock but Console.WriteX acquires 2 locks and we can get deadlocks.
            /// </summary>
            internal object LockObject { get; set; } = new object();

            public WrapperTextWriter(TextWriter innerWriter, LogLevel defaultLogLevel)
            {
                _innerWriter = innerWriter;
                _defaultLogLevel = defaultLogLevel;
            }

            internal void FormattedWriteLine(string message)
            {
                FormattedWriteLine(_defaultLogLevel, message);
            }

            internal void FormattedWriteLine(LogLevel? logLevel, string message)
            {
                lock(LockObject)
                {
                    var displayLevel = logLevel.ToString();
                    if (logLevel != null)
                    {
                        if (logLevel < LogLevel.Information)
                            return;

                        displayLevel = ConvertLogLevelToLabel(logLevel);
                    }

                    string line;
                    if (!string.IsNullOrEmpty(displayLevel))
                    {
                        line = $"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ}\t{CurrentAwsRequestId}\t{displayLevel}\t{message ?? string.Empty}";
                    }
                    else
                    {
                        line = $"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ}\t{CurrentAwsRequestId}\t{message ?? string.Empty}";
                    }

                    _innerWriter.WriteLine(line);
                }
            }

            private Task FormattedWriteLineAsync(string message)
            {
                FormattedWriteLine(message);
                return Task.CompletedTask;
            }

            /// <summary>
            /// Convert LogLevel enums to the the same string label that console provider for Microsoft.Extensions.Logging.ILogger uses.
            /// </summary>
            /// <param name="level"></param>
            /// <returns></returns>
            private static string ConvertLogLevelToLabel(LogLevel? level)
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

            #region WriteLine redirects to formatting
            public override void WriteLine(ulong value) => FormattedWriteLine(value.ToString(FormatProvider));

            public override void WriteLine(uint value) => FormattedWriteLine(value.ToString(FormatProvider));


            public override void WriteLine(string format, params object[] arg) => FormattedWriteLine(string.Format(format, arg));

            public override void WriteLine(string format, object arg0, object arg1, object arg2) => FormattedWriteLine(string.Format(format, arg0, arg1, arg2));

            public override void WriteLine(string format, object arg0) => FormattedWriteLine(string.Format(format, arg0));

            public override void WriteLine(string value) => FormattedWriteLine(value);

            public override void WriteLine(float value) => FormattedWriteLine(value.ToString(FormatProvider));

            public override void WriteLine(string format, object arg0, object arg1) => FormattedWriteLine(string.Format(format, arg0, arg1));

            public override void WriteLine(object value) => FormattedWriteLine(value == null ? String.Empty : value.ToString());


            public override void WriteLine(bool value) => FormattedWriteLine(value.ToString(FormatProvider));

            public override void WriteLine(char value) => FormattedWriteLine(value.ToString(FormatProvider));

            public override void WriteLine(char[] buffer) => FormattedWriteLine(buffer == null ? String.Empty : new string(buffer));

            public override void WriteLine() => FormattedWriteLine(string.Empty);

            public override void WriteLine(decimal value) => FormattedWriteLine(value.ToString(FormatProvider));

            public override void WriteLine(double value) => FormattedWriteLine(value.ToString(FormatProvider));

            public override void WriteLine(int value) => FormattedWriteLine(value.ToString(FormatProvider));

            public override void WriteLine(long value) => FormattedWriteLine(value.ToString(FormatProvider));

            public override void WriteLine(char[] buffer, int index, int count) => FormattedWriteLine(new string(buffer, index, count));

            public override Task WriteLineAsync(char value) => FormattedWriteLineAsync(value.ToString());

            public override Task WriteLineAsync(char[] buffer, int index, int count) => FormattedWriteLineAsync(new string(buffer, index, count));


            public override Task WriteLineAsync(string value) => FormattedWriteLineAsync(value);
            public override Task WriteLineAsync() => FormattedWriteLineAsync(string.Empty);


            public override void WriteLine(StringBuilder? value) => FormattedWriteLine(value?.ToString());
            public override void WriteLine(ReadOnlySpan<char> buffer) => FormattedWriteLine(new string(buffer));
            public override Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default) => FormattedWriteLineAsync(new string(buffer.Span));
            public override Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = default) => FormattedWriteLineAsync(value?.ToString());

            #endregion

            #region Simple Redirects
            public override Encoding Encoding => _innerWriter.Encoding;

            public override IFormatProvider FormatProvider => _innerWriter.FormatProvider;

            public override string NewLine
            {
                get { return _innerWriter.NewLine; }
                set { _innerWriter.NewLine = value; }
            }

            public override void Close() => _innerWriter.Close();



            public override void Flush() => _innerWriter.Flush();

            public override Task FlushAsync() => _innerWriter.FlushAsync();

            public override void Write(ulong value) => _innerWriter.Write(value);

            public override void Write(uint value) => _innerWriter.Write(value);


            public override void Write(string format, params object[] arg) => _innerWriter.Write(format, arg);

            public override void Write(string format, object arg0, object arg1, object arg2) => _innerWriter.Write(format, arg0, arg1, arg2);

            public override void Write(string format, object arg0, object arg1) => _innerWriter.Write(format, arg0, arg1);

            public override void Write(string format, object arg0) => _innerWriter.Write(format, arg0);

            public override void Write(string value) => _innerWriter.Write(value);


            public override void Write(object value) => _innerWriter.Write(value);

            public override void Write(long value) => _innerWriter.Write(value);
            public override void Write(int value) => _innerWriter.Write(value);

            public override void Write(double value) => _innerWriter.Write(value);

            public override void Write(decimal value) => _innerWriter.Write(value);

            public override void Write(char[] buffer, int index, int count) => _innerWriter.Write(buffer, index, count);

            public override void Write(char[] buffer) => _innerWriter.Write(buffer);

            public override void Write(char value) => _innerWriter.Write(value);

            public override void Write(bool value) => _innerWriter.Write(value);

            public override void Write(float value) => _innerWriter.Write(value);


            public override Task WriteAsync(string value) => _innerWriter.WriteAsync(value);

            public override Task WriteAsync(char[] buffer, int index, int count) => _innerWriter.WriteAsync(buffer, index, count);

            public override Task WriteAsync(char value) => _innerWriter.WriteAsync(value);


            protected override void Dispose(bool disposing) => _innerWriter.Dispose();

            public override void Write(StringBuilder? value) => _innerWriter.Write(value);

            public override void Write(ReadOnlySpan<char> buffer) => _innerWriter.Write(buffer);

            public override Task WriteAsync(StringBuilder? value, CancellationToken cancellationToken = default) => _innerWriter.WriteAsync(value, cancellationToken);

            public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default) => _innerWriter.WriteAsync(buffer, cancellationToken);

            public override ValueTask DisposeAsync() => _innerWriter.DisposeAsync();
            #endregion
        }
    }
}
