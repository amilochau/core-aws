using System;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an unsupported operation is performed.
    /// </summary>
    public class UnsupportedOperationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedOperationException"/> class.
        /// </summary>
        public UnsupportedOperationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedOperationException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">Error message</param>
        public UnsupportedOperationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedOperationException"/> class 
        /// with a specified error message and a reference to the inner exception that is 
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Inner exception</param>
        public UnsupportedOperationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
