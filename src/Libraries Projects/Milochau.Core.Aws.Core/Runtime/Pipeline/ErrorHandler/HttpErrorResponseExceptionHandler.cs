using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Util;
using System;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.ErrorHandler
{
    /// <summary>
    /// The exception handler for HttpErrorResponseException exception.
    /// </summary>
    public class HttpErrorResponseExceptionHandler : ExceptionHandler<HttpErrorResponseException>
    {
        /// <summary>
        /// The constructor for HttpErrorResponseExceptionHandler.
        /// </summary>
        /// <param name="logger">in instance of ILogger.</param>
        public HttpErrorResponseExceptionHandler() :
            base()
        {
        }

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
        public override bool HandleException(IExecutionContext executionContext, HttpErrorResponseException exception)
        {
            var requestContext = executionContext.RequestContext;
            var httpErrorResponse = exception.Response;

            using (httpErrorResponse.ResponseBody)
            {
                var responseStream = httpErrorResponse.ResponseBody.OpenResponse();
                return HandleExceptionStream(requestContext, httpErrorResponse, exception, responseStream);
            }
        }

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
        public override async System.Threading.Tasks.Task<bool> HandleExceptionAsync(IExecutionContext executionContext, HttpErrorResponseException exception)
        {
            var requestContext = executionContext.RequestContext;
            var httpErrorResponse = exception.Response;

            using(httpErrorResponse.ResponseBody)
            {
                var responseStream = await httpErrorResponse.ResponseBody.OpenResponseAsync().ConfigureAwait(false);
                return HandleExceptionStream(requestContext, httpErrorResponse, exception, responseStream);
            }
        }

        /// <summary>
        /// Shared logic for the HandleException and HandleExceptionAsync
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="httpErrorResponse"></param>
        /// <param name="exception"></param>
        /// <param name="responseStream"></param>
        /// <returns></returns>
        private static bool HandleExceptionStream(IRequestContext requestContext, IWebResponseData httpErrorResponse, HttpErrorResponseException exception, System.IO.Stream responseStream)
        {
            AmazonServiceException errorResponseException;
            // Unmarshall the service error response and throw the corresponding service exception.
            var unmarshaller = requestContext.Unmarshaller;
            var readEntireResponse = true;

            var errorContext = unmarshaller.CreateContext(httpErrorResponse,
                readEntireResponse,
                responseStream,
                true,
                requestContext);

            try
            {
                errorResponseException = unmarshaller.UnmarshallException(errorContext,
                    exception, httpErrorResponse.StatusCode);
            }
            catch (Exception e)
            {
                // Rethrow Amazon service or client exceptions 
                if (e is AmazonServiceException || e is AmazonClientException)
                {
                    throw;
                }

                // Else, there was an issue with the response body, throw AmazonUnmarshallingException
                var requestId = httpErrorResponse.GetHeaderValue(HeaderKeys.RequestIdHeader);
                var body = errorContext.ResponseBody;
                throw new AmazonUnmarshallingException(requestId, lastKnownLocation: null, responseBody: body,
                    innerException: e, statusCode: httpErrorResponse.StatusCode);
            }

            throw errorResponseException;
        }
    }
}
