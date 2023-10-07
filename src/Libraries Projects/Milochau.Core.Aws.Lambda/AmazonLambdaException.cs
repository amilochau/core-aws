using System;
using System.Net;
using Amazon.Runtime;

namespace Milochau.Core.Aws.Lambda
{
    ///<summary>
    /// Common exception for the Lambda service.
    /// </summary>
    public partial class AmazonLambdaException : AmazonServiceException
    {
        /// <summary>
        /// Construct instance of AmazonLambdaException
        /// </summary>
        public AmazonLambdaException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Construct instance of AmazonLambdaException
        /// </summary>
        public AmazonLambdaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Construct instance of AmazonLambdaException
        /// </summary>
        public AmazonLambdaException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }

        /// <summary>
        /// Construct instance of AmazonLambdaException
        /// </summary>
        public AmazonLambdaException(string message, ErrorType errorType, string? errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, errorType, errorCode, requestId, statusCode)
        {
        }

        /// <summary>
        /// Construct instance of AmazonLambdaException
        /// </summary>
        public AmazonLambdaException(string message, Exception innerException, ErrorType errorType, string? errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, innerException, errorType, errorCode, requestId, statusCode)
        {
        }
    }
}