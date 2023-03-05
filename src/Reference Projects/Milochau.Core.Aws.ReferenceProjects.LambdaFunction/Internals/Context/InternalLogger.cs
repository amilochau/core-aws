using System;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals.Context
{
    public class InternalLogger
    {
        private const string DebuggingEnvironmentVariable = "LAMBDA_RUNTIMESUPPORT_DEBUG";

        public static readonly InternalLogger ConsoleLogger = new InternalLogger(Console.WriteLine);
        public static readonly InternalLogger NoOpLogger = new InternalLogger(message => { });

        private readonly Action<string> _internalLoggingAction;

        /// <summary>
        /// Constructs InternalLogger which logs to the internalLoggingAction.
        /// </summary>
        /// <param name="internalLoggingAction"></param>
        private InternalLogger(Action<string> internalLoggingAction)
        {
            _internalLoggingAction = internalLoggingAction;
            if (_internalLoggingAction == null)
            {
                throw new ArgumentNullException(nameof(internalLoggingAction));
            }
        }

        public void LogDebug(string message)
        {
            _internalLoggingAction($"[Debug] {message}");
        }

        public void LogError(Exception exception, string message)
        {
            _internalLoggingAction($"[Error] {message} - {exception.ToString()}");
        }

        public void LogInformation(string message)
        {
            _internalLoggingAction($"[Info] {message}");
        }

        /// <summary>
        /// Gets an InternalLogger with a custom logging action.
        /// Mainly used for unit testing
        /// </summary>
        /// <param name="loggingAction"></param>
        /// <returns></returns>
        public static InternalLogger GetCustomInternalLogger(Action<string> loggingAction)
        {
            return new InternalLogger(loggingAction);
        }

        /// <summary>
        /// Gets the default logger for the environment based on the "LAMBDA_RUNTIMESUPPORT_DEBUG" environment variable
        /// </summary>
        /// <returns></returns>
        public static InternalLogger GetDefaultLogger()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(DebuggingEnvironmentVariable)))
            {
                return NoOpLogger;
            }

            return ConsoleLogger;
        }
    }
}
