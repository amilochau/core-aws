using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Util;
using System;
using System.Linq;
using System.Net.Http;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.ErrorHandler
{
    /// <summary>
    /// The exception handler for HttpErrorResponseException exception.
    /// </summary>
    public class HttpErrorResponseExceptionHandler
    {
        /// <summary>
        /// Handles an exception for the given execution context.
        /// </summary>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        /// <returns>
        /// Returns a boolean value which indicates if the original exception
        /// should be rethrown.
        /// This method can also throw a new exception to replace the original exception.
        /// </returns>
        public static async System.Threading.Tasks.Task<Exception> HandleAsync(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;
            var httpErrorResponse = executionContext.ResponseContext.HttpResponse;

            var responseStream = await httpErrorResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);

            AmazonServiceException errorResponseException;
            // Unmarshall the service error response and throw the corresponding service exception.

            var errorContext = new JsonUnmarshallerContext(responseStream,
                maintainResponseBody: true,
                httpErrorResponse,
                isException: true);

            try
            {
                errorResponseException = requestContext.Unmarshaller.UnmarshallException(errorContext, httpErrorResponse.StatusCode);
            }
            catch (AmazonServiceException e)
            {
                // Rethrow Amazon service exceptions
                return e;
            }
            catch (Exception e)
            {
                // Else, there was an issue with the response body, throw AmazonUnmarshallingException
                var requestId = httpErrorResponse.Headers.GetValues(HeaderKeys.RequestIdHeader).FirstOrDefault();
                return new AmazonUnmarshallingException(requestId, errorContext.ResponseBody, innerException: e, httpErrorResponse.StatusCode);
            }

            return errorResponseException;
        }
    }
}
