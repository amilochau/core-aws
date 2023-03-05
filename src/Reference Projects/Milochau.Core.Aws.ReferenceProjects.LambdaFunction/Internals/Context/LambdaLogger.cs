using System;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals.Context
{
    internal class LambdaConsoleLogger : ILambdaLogger
    {
        private readonly IConsoleLoggerWriter _consoleLoggerRedirector;

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

        public string? CurrentAwsRequestId { get; set; }

        public void Log(string level, string message)
        {
            _consoleLoggerRedirector.FormattedWriteLine(level, message);
        }
    }
}
