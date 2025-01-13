using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Context;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Logging
{
    internal class LambdaILogger(string categoryName, LambdaLoggerOptions options) : LambdaConsoleLogger(string.Empty), ILogger
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
            //  { => Scopes : }{Category: }{EventId: }MessageText {Exception}{\n}

            var components = new List<string?>(4);

            GetScopeInformation(components);

            if (options.IncludeCategory)
            {
                components.Add($"{categoryName}:");
            }
            if (options.IncludeEventId)
            {
                components.Add($"[{eventId}]:");
            }

            var text = formatter.Invoke(state, exception);
            components.Add(text);

            if (options.IncludeException)
            {
                components.Add($"{exception}");
            }

            var finalText = string.Join(" ", components);

            if (logLevel >= LogLevel.Error)
            {
                LogLineError(logLevel, finalText);
            }
            else
            {
                LogLine(logLevel, finalText);
            }
        }

        private void GetScopeInformation(List<string?> logMessageComponents)
        {
            var scopeProvider = ScopeProvider;

            if (options.IncludeScopes && scopeProvider != null)
            {
                var initialCount = logMessageComponents.Count;

                scopeProvider.ForEachScope((scope, list) =>
                {
                    if (scope != null)
                    {
                        list.Add(scope.ToString());
                    }
                }, logMessageComponents);

                if (logMessageComponents.Count > initialCount)
                {
                    logMessageComponents.Add("=>");
                }
            }
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
