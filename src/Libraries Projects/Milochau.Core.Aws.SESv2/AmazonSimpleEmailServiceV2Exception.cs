using Milochau.Core.Aws.Core.Runtime;
using System;
using System.Net;

namespace Milochau.Core.Aws.SESv2
{
    ///<summary>
    /// Common exception for the SimpleEmailServiceV2 service.
    /// </summary>
    public partial class AmazonSimpleEmailServiceV2Exception : AmazonServiceException
    {
        /// <summary>
        /// Construct instance of AmazonSimpleEmailServiceV2Exception
        /// </summary>
        public AmazonSimpleEmailServiceV2Exception(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Construct instance of AmazonSimpleEmailServiceV2Exception
        /// </summary>
        public AmazonSimpleEmailServiceV2Exception(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Construct instance of AmazonSimpleEmailServiceV2Exception
        /// </summary>
        public AmazonSimpleEmailServiceV2Exception(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }

        /// <summary>
        /// Construct instance of AmazonSimpleEmailServiceV2Exception
        /// </summary>
        public AmazonSimpleEmailServiceV2Exception(string message, ErrorType errorType, string? errorCode, string? requestId, HttpStatusCode statusCode)
            : base(message, errorType, errorCode, requestId, statusCode)
        {
        }

        /// <summary>
        /// Construct instance of AmazonSimpleEmailServiceV2Exception
        /// </summary>
        public AmazonSimpleEmailServiceV2Exception(string message, Exception innerException, ErrorType errorType, string? errorCode, string? requestId, HttpStatusCode statusCode)
            : base(message, innerException, errorType, errorCode, requestId, statusCode)
        {
        }
    }
}