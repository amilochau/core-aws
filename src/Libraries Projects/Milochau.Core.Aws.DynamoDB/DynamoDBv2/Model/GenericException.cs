using System;
using System.Collections.Generic;
using System.Net;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/ConditionalCheckFailedException.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model
{
    /// <summary>
    /// Generic exception when using DynamoDB
    /// </summary>
    public partial class GenericException : AmazonDynamoDBException
    {
        /// <summary>
        /// Constructs a new ConditionalCheckFailedException with the specified error
        /// message.
        /// </summary>
        /// <param name="message">
        /// Describes the error encountered.
        /// </param>
        public GenericException(string message)
            : base(message) { }

        /// <summary>
        /// Construct instance of ConditionalCheckFailedException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public GenericException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Construct instance of ConditionalCheckFailedException
        /// </summary>
        /// <param name="innerException"></param>
        public GenericException(Exception innerException)
            : base(innerException) { }

        /// <summary>
        /// Construct instance of ConditionalCheckFailedException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public GenericException(string message, Exception innerException, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, innerException, errorType, errorCode, requestId, statusCode) { }

        /// <summary>
        /// Construct instance of ConditionalCheckFailedException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public GenericException(string message, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, errorType, errorCode, requestId, statusCode) { }


        /// <summary>
        /// Gets and sets the property Item. 
        /// <para>
        /// Item which caused the <code>ConditionalCheckFailedException</code>.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue> Item { get; set; } = new Dictionary<string, AttributeValue>();
    }
}
