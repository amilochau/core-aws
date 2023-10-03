using System;
using System.Net;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/ResourceNotFoundException.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model
{
    /// <summary>
    /// The operation tried to access a nonexistent table or index. The resource might not
    /// be specified correctly, or its status might not be <code>ACTIVE</code>.
    /// </summary>
    public partial class ResourceNotFoundException : AmazonDynamoDBException
    {
        /// <summary>
        /// Constructs a new ResourceNotFoundException with the specified error
        /// message.
        /// </summary>
        /// <param name="message">
        /// Describes the error encountered.
        /// </param>
        public ResourceNotFoundException(string message)
            : base(message) { }

        /// <summary>
        /// Construct instance of ResourceNotFoundException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ResourceNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Construct instance of ResourceNotFoundException
        /// </summary>
        /// <param name="innerException"></param>
        public ResourceNotFoundException(Exception innerException)
            : base(innerException) { }

        /// <summary>
        /// Construct instance of ResourceNotFoundException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public ResourceNotFoundException(string message, Exception innerException, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, innerException, errorType, errorCode, requestId, statusCode) { }

        /// <summary>
        /// Construct instance of ResourceNotFoundException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public ResourceNotFoundException(string message, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, errorType, errorCode, requestId, statusCode) { }
    }
}
