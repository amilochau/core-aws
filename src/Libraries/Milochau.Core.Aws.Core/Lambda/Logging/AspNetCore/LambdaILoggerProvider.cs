using Milochau.Core.Aws.Core.Lambda.Logging.AspNetCore;
using System.Collections.Concurrent;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// The ILoggerProvider implementation that is added to the ASP.NET Core logging system to create loggers
    /// that will send the messages to the CloudWatch LogGroup associated with this Lambda function.
    /// </summary>
    /// <remarks>
    /// Creates the provider
    /// </remarks>
    internal class LambdaILoggerProvider() : ILoggerProvider//, ISupportExternalScope
    {
        private IExternalScopeProvider scopeProvider = NullExternalScopeProvider.Instance;
        private readonly ConcurrentDictionary<string, LambdaILogger> loggers = new();

        /// <summary>
        /// Creates the logger with the specified category.
        /// </summary>
        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, loggerName => new LambdaILogger(categoryName)
            {
                ScopeProvider = scopeProvider
            });
        }

        /// <inheritdoc />
        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            this.scopeProvider = scopeProvider;

            foreach (var logger in loggers)
            {
                logger.Value.ScopeProvider = this.scopeProvider;
            }
        }

        /// <summary></summary>
        public void Dispose()
        {
        }
    }
}
