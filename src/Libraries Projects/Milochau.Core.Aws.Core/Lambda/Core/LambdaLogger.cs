using System;

namespace Milochau.Core.Aws.Core.Lambda.Core
{
    /// <summary>
    /// Static class which sends a message to AWS CloudWatch Logs.
    /// When used outside of a Lambda environment, logs are written to
    /// Console.Out.
    /// </summary>
    public static class LambdaLogger
    {
        // Logging action, logs to Console by default
        internal static Action<string> _loggingAction = LogToConsole;

        // Logs message to console
        private static void LogToConsole(string message)
        {
            Console.WriteLine(message);
        }
    }
}
