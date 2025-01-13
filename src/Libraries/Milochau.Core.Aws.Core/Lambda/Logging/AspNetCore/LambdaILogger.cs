using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Context;
using Milochau.Core.Aws.Core.XRayRecorder.Core;
using System;

namespace Microsoft.Extensions.Logging
{
    internal class LambdaILogger(string categoryName) : LambdaConsoleLogger(RuntimeApiHeaders.RequestId), ILogger
    {
        internal IExternalScopeProvider? ScopeProvider { get; set; }

        // ILogger methods
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => ScopeProvider?.Push(state) ?? new NoOpDisposable();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            ArgumentNullException.ThrowIfNull(formatter);

            if (!IsEnabled(logLevel))
            {
                return;
            }

            // Format of the logged text, optional components are in {}
            //  {Category: }MessageText {Exception}{\n}

            var text = formatter.Invoke(state, exception);

            string[] components = [
                $"{categoryName}:",
                text,
                $"{exception}"
            ];

            var finalText = string.Join(" ", components);

            LogLine(logLevel, finalText);
        }

        // Private classes
        private class NoOpDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
