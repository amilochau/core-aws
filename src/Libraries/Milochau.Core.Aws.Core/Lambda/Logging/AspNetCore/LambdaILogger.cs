using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging
{
    internal class LambdaILogger(string categoryName, LambdaLoggerOptions options) : ILogger
    {
        internal IExternalScopeProvider? ScopeProvider { get; set; }

        // ILogger methods
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => ScopeProvider?.Push(state) ?? new NoOpDisposable();

        public bool IsEnabled(LogLevel logLevel)
        {
            return options.Filter == null || options.Filter(categoryName, logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            ArgumentNullException.ThrowIfNull(formatter);

            if (!IsEnabled(logLevel))
            {
                return;
            }

            // Format of the logged text, optional components are in {}
            //  {[LogLevel] }{ => Scopes : }{Category: }{EventId: }MessageText {Exception}{\n}

            var components = new List<string?>(4);
            if (options.IncludeLogLevel)
            {
                components.Add($"[{logLevel}]");
            }

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
            if (options.IncludeNewline)
            {
                components.Add(Environment.NewLine);
            }

            var finalText = string.Join(" ", components);
            Milochau.Core.Aws.Core.Lambda.Core.LambdaLogger.Log(finalText);
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
