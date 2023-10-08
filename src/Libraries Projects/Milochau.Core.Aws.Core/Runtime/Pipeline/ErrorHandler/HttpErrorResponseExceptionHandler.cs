/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 * 
 *  http://aws.amazon.com/apache2.0
 * 
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

using Amazon.Runtime.Internal.Transform;
using Amazon.Util;
using System;
using System.IO;

namespace Amazon.Runtime.Internal
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
        private bool HandleExceptionStream(IRequestContext requestContext, IWebResponseData httpErrorResponse, HttpErrorResponseException exception, System.IO.Stream responseStream)
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
