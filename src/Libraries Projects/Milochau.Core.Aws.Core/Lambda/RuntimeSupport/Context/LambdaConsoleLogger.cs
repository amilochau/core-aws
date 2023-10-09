using Milochau.Core.Aws.Core.Lambda.Core;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Helpers;
using System;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Context
{
    internal class LambdaConsoleLogger : ILambdaLogger
    {
        private IConsoleLoggerWriter _consoleLoggerRedirector;

        public LambdaConsoleLogger(IConsoleLoggerWriter consoleLoggerRedirector)
        {
            _consoleLoggerRedirector = consoleLoggerRedirector;
        }

        public void Log(string message)
        {
            Console.Write(message);
        }

        public void LogLine(string message)
        {
            _consoleLoggerRedirector.FormattedWriteLine(message);
        }

        public void Log(string level, string message)
        {
            _consoleLoggerRedirector.FormattedWriteLine(level, message);
        }
    }
}
