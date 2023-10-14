﻿using Milochau.Core.Aws.Core.Util;
using System;
using System.Linq;
using System.Net.Http;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.ErrorHandler
{
    /// <summary>
    /// The exception handler for HttpErrorResponseException exception.
    /// </summary>
    public class HttpErrorResponseExceptionHandler : IExceptionHandler<HttpErrorResponseException>
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
        public async System.Threading.Tasks.Task HandleAsync(IExecutionContext executionContext, HttpErrorResponseException exception)
        {
            var requestContext = executionContext.RequestContext;
            var httpErrorResponse = exception.Response;

            using (httpErrorResponse)
            {
                var responseStream = await httpErrorResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                HandleExceptionStream(requestContext, httpErrorResponse, exception, responseStream);
            }
        }

        /// <summary>
        /// Shared logic for the HandleException and HandleExceptionAsync
        /// </summary>
        private static void HandleExceptionStream(IRequestContext requestContext, HttpResponseMessage httpErrorResponse, HttpErrorResponseException exception, System.IO.Stream responseStream)
        {
            AmazonServiceException errorResponseException;
            // Unmarshall the service error response and throw the corresponding service exception.
            var unmarshaller = requestContext.Unmarshaller;
            var readEntireResponse = true;

            var errorContext = unmarshaller.CreateContext(httpErrorResponse,
                readEntireResponse,
                responseStream,
                true);

            try
            {
                errorResponseException = unmarshaller.UnmarshallException(errorContext, exception, httpErrorResponse.StatusCode);
            }
            catch (Exception e) when (e is AmazonServiceException || e is AmazonClientException)
            {
                // Rethrow Amazon service or client exceptions 
                throw;
            }
            catch (Exception e)
            {
                // Else, there was an issue with the response body, throw AmazonUnmarshallingException
                var requestId = httpErrorResponse.Headers.GetValues(HeaderKeys.RequestIdHeader).FirstOrDefault();
                var body = errorContext.ResponseBody;
                throw new AmazonUnmarshallingException(requestId, responseBody: body, innerException: e, statusCode: httpErrorResponse.StatusCode);
            }

            throw errorResponseException;
        }
    }
}
