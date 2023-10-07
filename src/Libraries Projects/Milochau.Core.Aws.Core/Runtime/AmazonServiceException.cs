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
using System;
using System.Globalization;
using System.Net;

namespace Amazon.Runtime
{
    /// <summary>
    /// A base exception for some Amazon Web Services.
    /// <para>
    /// Most exceptions thrown to client code will be service-specific exceptions, though some services
    /// may throw this exception if there is a problem which is caught in the core client code.
    /// </para>
    /// </summary>
    public class AmazonServiceException : Exception
    {
        public AmazonServiceException()
            : base()
        {
        }

        public AmazonServiceException(string message)
            : base(message)
        {
        }

        public AmazonServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AmazonServiceException(string message, Exception innerException, HttpStatusCode statusCode)
            : base(message, innerException)
        {
            this.StatusCode = statusCode;
        }

        public AmazonServiceException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }

        public AmazonServiceException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message ??
                BuildGenericErrorMessage(errorCode, statusCode))
        {
            this.ErrorCode = errorCode;
            this.ErrorType = errorType;
            this.RequestId = requestId;
            this.StatusCode = statusCode;
        }

        public AmazonServiceException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message ??
                BuildGenericErrorMessage(errorCode, statusCode), 
                innerException)
        {
            this.ErrorCode = errorCode;
            this.ErrorType = errorType;
            this.RequestId = requestId;
            this.StatusCode = statusCode;
        }

        static string BuildGenericErrorMessage(string errorCode, HttpStatusCode statusCode)
        {
            return string.Format(CultureInfo.InvariantCulture,  
                "Error making request with Error Code {0} and Http Status Code {1}. No further error information was returned by the service.", errorCode, statusCode);
        }

        /// <summary>
        /// Whether the error was attributable to <c>Sender</c> or <c>Reciever</c>.
        /// </summary>
        public ErrorType ErrorType { get; set; }

        /// <summary>
        /// The error code returned by the service
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// The id of the request which generated the exception.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// The HTTP status code from the service response
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Flag indicating if the exception is retryable and the associated retry
        /// details. A null value indicates that the exception is not retryable.
        /// </summary>
        public virtual RetryableDetails Retryable => null;
    }

    /// <summary>
    /// Class containing the retryable details for an AmazonServiceException
    /// </summary>
    public class RetryableDetails
    {
        public RetryableDetails(bool throttling)
        {
            Throttling = throttling;
        }

        /// <summary>
        /// This property indicates that this exception is a 
        /// throttling exception and should be subject to congestion
        /// control throttling.
        /// </summary>
        public bool Throttling { get; private set; }
    }
}
