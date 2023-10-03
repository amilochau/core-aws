using System;
using System.Net;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/TransactionConflictException.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model
{
    /// <summary>
    /// Operation was rejected because there is an ongoing transaction for the item.
    /// </summary>
    public partial class TransactionConflictException : AmazonDynamoDBException
    {
        /// <summary>
        /// Constructs a new TransactionConflictException with the specified error
        /// message.
        /// </summary>
        /// <param name="message">
        /// Describes the error encountered.
        /// </param>
        public TransactionConflictException(string message)
            : base(message) { }

        /// <summary>
        /// Construct instance of TransactionConflictException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public TransactionConflictException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Construct instance of TransactionConflictException
        /// </summary>
        /// <param name="innerException"></param>
        public TransactionConflictException(Exception innerException)
            : base(innerException) { }

        /// <summary>
        /// Construct instance of TransactionConflictException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public TransactionConflictException(string message, Exception innerException, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, innerException, errorType, errorCode, requestId, statusCode) { }

        /// <summary>
        /// Construct instance of TransactionConflictException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public TransactionConflictException(string message, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, errorType, errorCode, requestId, statusCode) { }
    }
}
