using System;
using System.Collections.Generic;
using System.Net;

namespace Milochau.Core.Aws.DynamoDB.Model
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
        public GenericException(string message)
            : base(message) { }

        /// <summary>
        /// Construct instance of ConditionalCheckFailedException
        /// </summary>
        public GenericException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Construct instance of ConditionalCheckFailedException
        /// </summary>
        public GenericException(Exception innerException)
            : base(innerException) { }

        /// <summary>
        /// Construct instance of ConditionalCheckFailedException
        /// </summary>
        public GenericException(string message, Exception innerException, Amazon.Runtime.ErrorType errorType, string? errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, innerException, errorType, errorCode, requestId, statusCode) { }

        /// <summary>
        /// Construct instance of ConditionalCheckFailedException
        /// </summary>
        public GenericException(string message, Amazon.Runtime.ErrorType errorType, string? errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, errorType, errorCode, requestId, statusCode) { }


        /// <summary>
        /// Gets and sets the property Item. 
        /// <para>
        /// Item which caused the <code>ConditionalCheckFailedException</code>.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? Item { get; set; }
    }
}
