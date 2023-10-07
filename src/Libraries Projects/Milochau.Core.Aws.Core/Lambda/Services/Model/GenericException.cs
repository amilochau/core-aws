using System;
using System.Net;

namespace Amazon.Lambda.Model
{
    /// <summary>
    /// Lambda couldn't decrypt the environment variables because the KMS key used is disabled.
    /// Check the Lambda function's KMS key settings.
    /// </summary>
    public partial class GenericException : AmazonLambdaException
    {
        private string _type;

        /// <summary>
        /// Constructs a new KMSDisabledException with the specified error
        /// message.
        /// </summary>
        /// <param name="message">
        /// Describes the error encountered.
        /// </param>
        public GenericException(string message)
            : base(message) { }

        /// <summary>
        /// Construct instance of KMSDisabledException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public GenericException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Construct instance of KMSDisabledException
        /// </summary>
        /// <param name="innerException"></param>
        public GenericException(Exception innerException)
            : base(innerException) { }

        /// <summary>
        /// Construct instance of KMSDisabledException
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
        /// Construct instance of KMSDisabledException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorType"></param>
        /// <param name="errorCode"></param>
        /// <param name="requestId"></param>
        /// <param name="statusCode"></param>
        public GenericException(string message, Amazon.Runtime.ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, errorType, errorCode, requestId, statusCode) { }

        /// <summary>
        /// Gets and sets the property Type.
        /// </summary>
        public string Type
        {
            get { return this._type; }
            set { this._type = value; }
        }

        // Check to see if Type property is set
        internal bool IsSetType()
        {
            return this._type != null;
        }

    }
}
