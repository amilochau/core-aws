using Amazon.Runtime;
using Milochau.Core.Aws.DynamoDB.Model;
using System;
using System.Collections.Generic;
using System.Net;

namespace Milochau.Core.Aws.DynamoDB
{
    ///<summary>
    /// Common exception for the DynamoDB service.
    /// </summary>
    public partial class AmazonDynamoDBException : AmazonServiceException
    {
        /// <summary>
        /// Construct instance of AmazonDynamoDBException
        /// </summary>
        public AmazonDynamoDBException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Construct instance of AmazonDynamoDBException
        /// </summary>
        public AmazonDynamoDBException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Construct instance of AmazonDynamoDBException
        /// </summary>
        public AmazonDynamoDBException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }

        /// <summary>
        /// Construct instance of AmazonDynamoDBException
        /// </summary>
        public AmazonDynamoDBException(string message, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, errorType, errorCode, requestId, statusCode)
        {
        }

        /// <summary>
        /// Construct instance of AmazonDynamoDBException
        /// </summary>
        public AmazonDynamoDBException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, innerException, errorType, errorCode, requestId, statusCode)
        {
        }
    }
}
