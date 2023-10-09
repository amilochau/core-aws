using System;

namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// Exception thrown by the SDK for errors that occur within the SDK.
    /// </summary>
    public class AmazonClientException : Exception
    {
        public AmazonClientException(string message) : base(message) { }

        public AmazonClientException(string message, Exception innerException) : base(message, innerException) { }
    }
}
