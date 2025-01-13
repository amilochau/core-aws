using Microsoft.Extensions.Logging;
using Milochau.Core.Aws.Core.Lambda.Core;
using System;

namespace Milochau.Core.Aws.Integration
{
    /// <summary>
    /// An implementation if ILambdaLogger that writes the messages to the console.
    /// </summary>
    public class TestLambdaLogger : ILambdaLogger
    {
        /// <inheritdoc/>
        public void LogLine(LogLevel logLevel, string message)
        {
            Console.WriteLine(message);
        }
    }
}
