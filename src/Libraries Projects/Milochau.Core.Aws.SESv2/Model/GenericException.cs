using System;
using System.Net;

namespace Milochau.Core.Aws.SESv2.Model
{
    /// <summary>
    /// Error while sending an email
    /// restricted.
    /// </summary>
    public partial class GenericException : AmazonSimpleEmailServiceV2Exception
    {
        /// <summary>
        /// Constructs a new GenericException with the specified error
        /// message.
        /// </summary>
        public GenericException(string message) 
            : base(message) {}

        /// <summary>
        /// Construct instance of GenericException
        /// </summary>
        public GenericException(string message, Exception innerException) 
            : base(message, innerException) {}

        /// <summary>
        /// Construct instance of GenericException
        /// </summary>
        public GenericException(Exception innerException) 
            : base(innerException) {}

        /// <summary>
        /// Construct instance of GenericException
        /// </summary>
        public GenericException(string message, Exception innerException, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode) 
            : base(message, innerException, errorType, errorCode, requestId, statusCode) {}

        /// <summary>
        /// Construct instance of GenericException
        /// </summary>
        public GenericException(string message, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode) 
            : base(message, errorType, errorCode, requestId, statusCode) {}
    }
}