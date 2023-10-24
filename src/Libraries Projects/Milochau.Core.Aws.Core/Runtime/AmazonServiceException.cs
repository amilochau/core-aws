using System;
using System.Globalization;
using System.Net;

namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// A base exception for some Amazon Web Services.
    /// <para>
    /// Most exceptions thrown to client code will be service-specific exceptions, though some services
    /// may throw this exception if there is a problem which is caught in the core client code.
    /// </para>
    /// </summary>
    public class AmazonServiceException : Exception
    {
        public AmazonServiceException()
            : base()
        {
        }

        public AmazonServiceException(string message)
            : base(message)
        {
        }

        public AmazonServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AmazonServiceException(string message, Exception innerException, HttpStatusCode statusCode)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }

        public AmazonServiceException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }

        public AmazonServiceException(string message, ErrorType errorType, string? errorCode, string? requestId, HttpStatusCode statusCode)
            : base(message ??
                BuildGenericErrorMessage(errorCode, statusCode))
        {
            ErrorType = errorType;
            RequestId = requestId;
            StatusCode = statusCode;
        }

        public AmazonServiceException(string message, Exception innerException, ErrorType errorType, string? errorCode, string? requestId, HttpStatusCode statusCode)
            : base(message ??
                BuildGenericErrorMessage(errorCode, statusCode), 
                innerException)
        {
            ErrorType = errorType;
            RequestId = requestId;
            StatusCode = statusCode;
        }

        static string BuildGenericErrorMessage(string? errorCode, HttpStatusCode statusCode)
        {
            return string.Format(CultureInfo.InvariantCulture,  
                "Error making request with Error Code {0} and Http Status Code {1}. No further error information was returned by the service.", errorCode, statusCode);
        }

        /// <summary>
        /// Whether the error was attributable to <c>Sender</c> or <c>Reciever</c>.
        /// </summary>
        public ErrorType ErrorType { get; set; }

        /// <summary>
        /// The id of the request which generated the exception.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// The HTTP status code from the service response
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
    }
}
