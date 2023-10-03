using System;
using System.Net;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/ProvisionedThroughputExceededException.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model
{
    /// <summary>
    /// Your request rate is too high. The Amazon Web Services SDKs for DynamoDB automatically
    /// retry requests that receive this exception. Your request is eventually successful,
    /// unless your retry queue is too large to finish. Reduce the frequency of requests and
    /// use exponential backoff. For more information, go to <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Programming.Errors.html#Programming.Errors.RetryAndBackoff">Error
    /// Retries and Exponential Backoff</a> in the <i>Amazon DynamoDB Developer Guide</i>.
    /// </summary>
    public partial class ProvisionedThroughputExceededException : AmazonDynamoDBException
    {
        /// <summary>
        /// Constructs a new ProvisionedThroughputExceededException with the specified error
        /// message.
        /// </summary>
        /// <param name="message">
        /// Describes the error encountered.
        /// </param>
        public ProvisionedThroughputExceededException(string message)
            : base(message) { }

        /// <summary>
        /// Construct instance of ProvisionedThroughputExceededException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ProvisionedThroughputExceededException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Construct instance of ProvisionedThroughputExceededException
        /// </summary>
        /// <param name="innerException"></param>
        public ProvisionedThroughputExceededException(Exception innerException)
            : base(innerException) { }

        /// <summary>
        /// Construct instance of ProvisionedThroughputExceededException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public ProvisionedThroughputExceededException(string message, Exception innerException, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, innerException, errorType, errorCode, requestId, statusCode) { }

        /// <summary>
        /// Construct instance of ProvisionedThroughputExceededException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public ProvisionedThroughputExceededException(string message, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, errorType, errorCode, requestId, statusCode) { }
    }
}
