using System;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.ErrorHandler
{
    /// <summary>
    /// This handler processes exceptions thrown from the HTTP handler and
    /// unmarshalls error responses.
    /// </summary>
    public class ErrorHandler : PipelineHandler
    {
        private readonly IExceptionHandler<HttpErrorResponseException> exceptionHandler;

        /// <summary>
        /// Constructor for ErrorHandler.
        /// </summary>
        /// <param name="logger">an ILogger instance.</param>
        public ErrorHandler()
        {
            exceptionHandler = new HttpErrorResponseExceptionHandler();
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
                try
                {
                    return await base.InvokeAsync<T>(executionContext).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    DisposeReponse(executionContext.ResponseContext);
                    throw;
                }
            }
            catch (HttpErrorResponseException exception)
            {
                await exceptionHandler.HandleAsync(executionContext, exception).ConfigureAwait(false);
                throw; // The previous line always throw...
            }
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
    }
}
