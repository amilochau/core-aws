using Milochau.Core.Aws.Core.Lambda.Core;
using System;
using System.Text;

namespace Milochau.Core.Aws.Integration
{
    /// <summary>
    /// An implementation if ILambdaLogger that stores all the messages in a buffer and writes the messages to the console.
    /// </summary>
    public class TestLambdaLogger : ILambdaLogger
    {
        /// <summary>
        /// Buffer for all the log messages written to the logger.
        /// </summary>
        public StringBuilder Buffer { get; } = new StringBuilder();

        /// <summary>
        /// Write log messages to the console and the Buffer without appending a newline terminator.
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message)
        {
            Buffer.Append(message);
            Console.Write(message);
        }

        /// <summary>
        /// Write log messages to the console and the Buffer with a newline terminator.
        /// </summary>
        /// <param name="message"></param>
        public void LogLine(string message)
        {
            Buffer.AppendLine(message);
            Console.WriteLine(message);
        }
    }
}
