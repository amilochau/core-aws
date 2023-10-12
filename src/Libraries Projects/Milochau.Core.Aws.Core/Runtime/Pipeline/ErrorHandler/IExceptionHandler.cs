using System;
using System.Diagnostics.CodeAnalysis;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.ErrorHandler
{
    /// <summary>
    /// The interface for an exception handler.
    /// </summary>    
    public interface IExceptionHandler<TException> where TException : Exception
    {
        /// <summary>
        /// Handles an exception for the given execution context.
        /// </summary>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        /// <param name="exception">The exception to handle.</param>
        /// <returns>
        /// Returns a boolean value which indicates if the original exception
        /// should be rethrown.
        /// This method can also throw a new exception to replace the original exception.
        /// </returns>
        [DoesNotReturn]
        System.Threading.Tasks.Task HandleAsync(IExecutionContext executionContext, TException exception);
    }
}
