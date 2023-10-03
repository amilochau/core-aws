using System;
using System.Net;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/RequestLimitExceededException.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model
{
    /// <summary>
    /// Throughput exceeds the current throughput quota for your account. Please contact <a
    /// href="https://aws.amazon.com/support">Amazon Web Services Support</a> to request a
    /// quota increase.
    /// </summary>
    public partial class RequestLimitExceededException : AmazonDynamoDBException
    {
        /// <summary>
        /// Constructs a new RequestLimitExceededException with the specified error
        /// message.
        /// </summary>
        /// <param name="message">
        /// Describes the error encountered.
        /// </param>
        public RequestLimitExceededException(string message)
            : base(message) { }

        /// <summary>
        /// Construct instance of RequestLimitExceededException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public RequestLimitExceededException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Construct instance of RequestLimitExceededException
        /// </summary>
        /// <param name="innerException"></param>
        public RequestLimitExceededException(Exception innerException)
            : base(innerException) { }

        /// <summary>
        /// Construct instance of RequestLimitExceededException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public RequestLimitExceededException(string message, Exception innerException, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, innerException, errorType, errorCode, requestId, statusCode) { }

        /// <summary>
        /// Construct instance of RequestLimitExceededException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public RequestLimitExceededException(string message, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, errorType, errorCode, requestId, statusCode) { }
    }
}
