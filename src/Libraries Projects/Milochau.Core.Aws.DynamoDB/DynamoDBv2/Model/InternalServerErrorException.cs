using System;
using System.Net;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/InternalServerErrorException.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model
{
    /// <summary>
    /// An error occurred on the server side.
    /// </summary>
    public partial class InternalServerErrorException : AmazonDynamoDBException
    {

        /// <summary>
        /// Constructs a new InternalServerErrorException with the specified error
        /// message.
        /// </summary>
        /// <param name="message">
        /// Describes the error encountered.
        /// </param>
        public InternalServerErrorException(string message)
            : base(message) { }

        /// <summary>
        /// Construct instance of InternalServerErrorException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public InternalServerErrorException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Construct instance of InternalServerErrorException
        /// </summary>
        /// <param name="innerException"></param>
        public InternalServerErrorException(Exception innerException)
            : base(innerException) { }

        /// <summary>
        /// Construct instance of InternalServerErrorException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public InternalServerErrorException(string message, Exception innerException, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, innerException, errorType, errorCode, requestId, statusCode) { }

        /// <summary>
        /// Construct instance of InternalServerErrorException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public InternalServerErrorException(string message, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, errorType, errorCode, requestId, statusCode) { }
    }
}
