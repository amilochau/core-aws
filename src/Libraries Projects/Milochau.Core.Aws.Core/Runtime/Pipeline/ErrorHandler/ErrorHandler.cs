using System;
using System.Collections.Generic;
using System.Reflection;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.ErrorHandler
{
    /// <summary>
    /// This handler processes exceptions thrown from the HTTP handler and
    /// unmarshalls error responses.
    /// </summary>
    public class ErrorHandler : PipelineHandler
    {
        /// <summary>
        /// Default set of exception handlers.
        /// </summary>
        public IDictionary<Type, IExceptionHandler> ExceptionHandlers { get; }

        /// <summary>
        /// Constructor for ErrorHandler.
        /// </summary>
        /// <param name="logger">an ILogger instance.</param>
        public ErrorHandler()
        {
            ExceptionHandlers = new Dictionary<Type, IExceptionHandler>
            {
                {typeof(HttpErrorResponseException), new HttpErrorResponseExceptionHandler()}
            };
        }

        /// <summary>
        /// Handles and processes any exception thrown from underlying handlers.
        /// </summary>
        /// <typeparam name="T">The response type for the current request.</typeparam>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            try
            {
                return await base.InvokeAsync<T>(executionContext).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                DisposeReponse(executionContext.ResponseContext);
                bool rethrowOriginalException = await ProcessExceptionAsync(executionContext, exception).ConfigureAwait(false);
                if (rethrowOriginalException)
                {
                    throw;
                }
            }

            // If response if set and an exception is not rethrown, return the response.
            // E.g. S3 GetLifecycleConfiguration, GetBucket policy and few other operations
            // return a 404 which is not returned back as an exception but as a empty response with 
            // error code.
            if(executionContext.ResponseContext != null && executionContext.ResponseContext.Response != null)
            {
                return executionContext.ResponseContext.Response as T;
            }

            return null;
        }

        /// <summary>
        /// Disposes the response body.
        /// </summary>
        /// <param name="responseContext">The response context.</param>
        private static void DisposeReponse(IResponseContext responseContext)
        {
            if (responseContext.HttpResponse != null &&
                responseContext.HttpResponse.ResponseBody != null)
            {
                responseContext.HttpResponse.ResponseBody.Dispose();
            }
        }

        /// <summary>
        /// Processes an exception by invoking a matching exception handler
        /// for the given exception.
        /// </summary>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        /// <param name="exception">The exception to be processed.</param>
        /// <returns>
        /// This method returns a boolean value which indicates if the original exception
        /// should be rethrown.
        /// This method can also throw a new exception that may be thrown by exception
        /// processing by a matching exception handler.
        /// </returns>
        private async System.Threading.Tasks.Task<bool> ProcessExceptionAsync(IExecutionContext executionContext, Exception exception)
        {
            // Find the matching handler which can process the exception
            // Start by checking if there is a matching handler for the specific exception type,
            // if not check for handlers for it's base type till we find a match.
            var exceptionType = exception.GetType();
            var exceptionTypeInfo = exception.GetType().GetTypeInfo().BaseType;
            do
            {

                if (ExceptionHandlers.TryGetValue(exceptionType, out IExceptionHandler? exceptionHandler))
                {
                    return await exceptionHandler.HandleAsync(executionContext, exception).ConfigureAwait(false);
                }
                exceptionType = exceptionTypeInfo.BaseType;
                exceptionTypeInfo = exceptionTypeInfo.BaseType.GetTypeInfo().BaseType;

            } while (exceptionType != typeof(Exception));

            // No match found, rethrow the original exception.
            return true;
        }
    }
}
