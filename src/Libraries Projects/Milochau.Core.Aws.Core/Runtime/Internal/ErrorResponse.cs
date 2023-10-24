using System;
using System.Net;

namespace Milochau.Core.Aws.Core.Runtime.Internal
{
    public class ErrorResponse
    {
        /// <summary>
        /// Error type, one of Sender, Receiver, Unknown
        /// Only applies to XML-based services.
        /// </summary>
        public ErrorType Type { get; set; }

        /// <summary>
        /// Name of the exception class to return
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        /// RequestId of the error.
        /// Only applies to XML-based services.
        /// </summary>
        public string? RequestId { get; set; }

        public Exception? InnerException { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}
