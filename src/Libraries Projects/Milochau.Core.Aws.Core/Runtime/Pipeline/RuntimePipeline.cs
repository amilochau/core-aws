using Milochau.Core.Aws.Core.Runtime.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline
{
    /// <summary>
    /// A runtime pipeline contains a collection of handlers which represent
    /// different stages of request and response processing.
    /// </summary>
    public partial class RuntimePipeline : IDisposable
    {
        bool _disposed;

        /// <summary>
        /// The top-most handler in the pipeline.
        /// </summary>
        public IPipelineHandler Handler { get; private set; }

        /// <summary>
        /// Constructor for RuntimePipeline.
        /// </summary>
        /// <param name="handlers">List of handlers with which the pipeline is initialized.</param>
        /// <param name="logger">The logger used to log messages.</param>
        public RuntimePipeline(IList<IPipelineHandler> handlers)
        {
            if (handlers == null || handlers.Count == 0)
                throw new ArgumentNullException(nameof(handlers));

            foreach (var handler in handlers)
            {
                AddHandler(handler);
            }
        }

        /// <summary>
        /// Invokes the pipeline asynchronously.
        /// </summary>
        /// <param name="executionContext">Request context</param>
        /// <returns>Response context</returns>
        public System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
            where T : AmazonWebServiceResponse, new()
        {
            ThrowIfDisposed();

            return Handler.InvokeAsync<T>(executionContext);
        }

        /// <summary>
        /// Adds a new handler to the top of the pipeline.
        /// </summary>
        /// <param name="handler">The handler to be added to the pipeline.</param>
        public void AddHandler(IPipelineHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            ThrowIfDisposed();

            var innerMostHandler = GetInnermostHandler(handler);

            if (Handler != null)
            {
                innerMostHandler.InnerHandler = Handler;
            }
            
            Handler = handler;

            SetHandlerProperties(handler);
        }

        /// <summary>
        /// Adds a handler before the first instance of handler of type T.
        /// </summary>
        /// <typeparam name="T">Type of the handler before which the given handler instance is added.</typeparam>
        /// <param name="handler">The handler to be added to the pipeline.</param>
        public void AddHandlerBefore<T>(IPipelineHandler handler)
            where T : IPipelineHandler
        {
            ThrowIfDisposed();

            var type = typeof(T);
            if (Handler.GetType() == type)
            {
                // Add the handler to the top of the pipeline
                AddHandler(handler);
                SetHandlerProperties(handler);
                return;
            }

            var current = Handler;
            while (current != null)
            {
                if (current.InnerHandler != null &&
                    current.InnerHandler.GetType() == type)
                {
                    InsertHandler(handler, current);
                    SetHandlerProperties(handler);
                    return;
                }
                current = current.InnerHandler;
            }

            throw new InvalidOperationException(
                string.Format(CultureInfo.InvariantCulture, "Cannot find a handler of type {0}", type.Name));
        }

        /// <summary>
        /// Inserts the given handler after current handler in the pipeline.
        /// </summary>
        /// <param name="handler">Handler to be inserted in the pipeline.</param>
        /// <param name="current">Handler after which the given handler is inserted.</param>
        private static void InsertHandler(IPipelineHandler handler, IPipelineHandler current)
        {
            var next = current.InnerHandler;
            current.InnerHandler = handler;
            
            if (next != null)
            {
                var innerMostHandler = GetInnermostHandler(handler);
                innerMostHandler.InnerHandler = next;
            }
        }

        /// <summary>
        /// Gets the innermost handler by traversing the inner handler till 
        /// it reaches the last one.
        /// </summary>
        private static IPipelineHandler GetInnermostHandler(IPipelineHandler handler)
        {
            var current = handler;
            while (current.InnerHandler != null)
            {
                current = current.InnerHandler;
            }
            return current;
        }

        private void SetHandlerProperties(IPipelineHandler handler)
        {
            ThrowIfDisposed();
        }

        #region Dispose methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                var handler = Handler;
                while (handler != null)
                {
                    var innerHandler = handler.InnerHandler;
                    if (handler is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                    handler = innerHandler;
                }

                _disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        #endregion
    }
}
