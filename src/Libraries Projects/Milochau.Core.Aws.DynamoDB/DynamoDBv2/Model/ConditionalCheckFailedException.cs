using System;
using System.Collections.Generic;
using System.Net;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/ConditionalCheckFailedException.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model
{
    /// <summary>
    /// A condition specified in the operation could not be evaluated.
    /// </summary>
    public partial class ConditionalCheckFailedException : AmazonDynamoDBException
    {
        /// <summary>
        /// Constructs a new ConditionalCheckFailedException with the specified error
        /// message.
        /// </summary>
        /// <param name="message">
        /// Describes the error encountered.
        /// </param>
        public ConditionalCheckFailedException(string message)
            : base(message) { }

        /// <summary>
        /// Construct instance of ConditionalCheckFailedException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ConditionalCheckFailedException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Construct instance of ConditionalCheckFailedException
        /// </summary>
        /// <param name="innerException"></param>
        public ConditionalCheckFailedException(Exception innerException)
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
        public ConditionalCheckFailedException(string message, Exception innerException, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, innerException, errorType, errorCode, requestId, statusCode) { }

        /// <summary>
        /// Construct instance of ConditionalCheckFailedException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public ConditionalCheckFailedException(string message, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
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
