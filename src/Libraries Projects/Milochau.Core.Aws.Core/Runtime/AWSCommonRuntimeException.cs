using System;

namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// Exception thrown for issues related to the AWS Common Runtime (CRT)
    /// </summary>
    public class AWSCommonRuntimeException : AmazonClientException
    {
        /// <summary>
        /// Initializes a new instance of an AWSCommonRuntimeException
        /// </summary>
        /// <param name="message">The message that desribes the error</param>
        public AWSCommonRuntimeException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of an AWSCommonRuntimeException
        /// </summary>
        /// <param name="message">The message that desribes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public AWSCommonRuntimeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
