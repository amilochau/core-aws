using System;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.ErrorHandler
{
    /// <summary>
    /// The abstract base class for exception handlers.
    /// </summary>
    /// <typeparam name="T">The exception type.</typeparam>
    public abstract class ExceptionHandler<T> : IExceptionHandler<T> where T : Exception
    {
        protected ExceptionHandler()
        {
        }

        public abstract bool HandleException(IExecutionContext executionContext, T exception);

        public async System.Threading.Tasks.Task<bool> HandleAsync(IExecutionContext executionContext, Exception exception)
        {
            return await HandleExceptionAsync(executionContext, exception as T).ConfigureAwait(false);
        }
        public abstract System.Threading.Tasks.Task<bool> HandleExceptionAsync(IExecutionContext executionContext, T exception);
    }
}
