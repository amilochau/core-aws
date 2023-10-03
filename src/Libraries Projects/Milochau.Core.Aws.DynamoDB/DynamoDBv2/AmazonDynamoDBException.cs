using Amazon.Runtime;
using System;
using System.Net;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/AmazonDynamoDBException.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2
{
    ///<summary>
    /// Common exception for the DynamoDB service.
    /// </summary>
    public partial class AmazonDynamoDBException : AmazonServiceException
    {
        /// <summary>
        /// Construct instance of AmazonDynamoDBException
        /// </summary>
        /// <param name="message"></param>
        public AmazonDynamoDBException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Construct instance of AmazonDynamoDBException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public AmazonDynamoDBException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Construct instance of AmazonDynamoDBException
        /// </summary>
        /// <param name="innerException"></param>
        public AmazonDynamoDBException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }

        /// <summary>
        /// Construct instance of AmazonDynamoDBException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public AmazonDynamoDBException(string message, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, errorType, errorCode, requestId, statusCode)
        {
        }

        /// <summary>
        /// Construct instance of AmazonDynamoDBException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public AmazonDynamoDBException(string message, Exception innerException, Amazon.Runtime.ErrorType errorType, string? errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, innerException, errorType, errorCode, requestId, statusCode)
        {
        }
    }
}
