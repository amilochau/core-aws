using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Lambda.Logging.AspNetCore
{
    /// <summary>
    /// Scope provider that does nothing.
    /// </summary>
    internal class NullExternalScopeProvider : IExternalScopeProvider
    {
        private NullExternalScopeProvider()
        {
        }

        /// <summary>
        /// Returns a cached instance of <see cref="NullExternalScopeProvider"/>.
        /// </summary>
        public static IExternalScopeProvider Instance { get; } = new NullExternalScopeProvider();

        /// <inheritdoc />
        void IExternalScopeProvider.ForEachScope<TState>(Action<object?, TState> callback, TState state)
        {
        }

        /// <inheritdoc />
        IDisposable IExternalScopeProvider.Push(object? state)
        {
            return NullScope.Instance;
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new NullScope();

            private NullScope()
            {
            }

            public void Dispose()
            {
            }
        }
    }
}
