using System;

namespace Amazon.XRay.Recorder.Core.Exceptions
{
    /// <summary>
    /// The exception that is thrown when segment is not available.
    /// </summary>
    [Serializable]
    public class EntityNotAvailableException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotAvailableException"/> class.
        /// </summary>
        public EntityNotAvailableException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotAvailableException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">Error message</param>
        public EntityNotAvailableException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotAvailableException"/> class 
        /// with a specified error message and a reference to the inner exception that is 
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Inner exception</param>
        public EntityNotAvailableException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
