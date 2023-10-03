using System;
using System.Net;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/ItemCollectionSizeLimitExceededException.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model
{
    /// <summary>
    /// An item collection is too large. This exception is only returned for tables that have
    /// one or more local secondary indexes.
    /// </summary>
    public partial class ItemCollectionSizeLimitExceededException : AmazonDynamoDBException
    {
        /// <summary>
        /// Constructs a new ItemCollectionSizeLimitExceededException with the specified error
        /// message.
        /// </summary>
        /// <param name="message">
        /// Describes the error encountered.
        /// </param>
        public ItemCollectionSizeLimitExceededException(string message)
            : base(message) { }

        /// <summary>
        /// Construct instance of ItemCollectionSizeLimitExceededException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ItemCollectionSizeLimitExceededException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Construct instance of ItemCollectionSizeLimitExceededException
        /// </summary>
        /// <param name="innerException"></param>
        public ItemCollectionSizeLimitExceededException(Exception innerException)
            : base(innerException) { }

        /// <summary>
        /// Construct instance of ItemCollectionSizeLimitExceededException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public ItemCollectionSizeLimitExceededException(string message, Exception innerException, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, innerException, errorType, errorCode, requestId, statusCode) { }

        /// <summary>
        /// Construct instance of ItemCollectionSizeLimitExceededException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public ItemCollectionSizeLimitExceededException(string message, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, errorType, errorCode, requestId, statusCode) { }
    }
}
