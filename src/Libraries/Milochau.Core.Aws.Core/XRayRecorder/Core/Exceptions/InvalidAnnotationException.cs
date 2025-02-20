﻿using System;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an invalid annotation is seen.
    /// </summary>
    [Serializable]
    public class InvalidAnnotationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAnnotationException"/> class.
        /// </summary>
        public InvalidAnnotationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAnnotationException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">Error message</param>
        public InvalidAnnotationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAnnotationException"/> class 
        /// with a specified error message and a reference to the inner exception that is 
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Inner exception</param>
        public InvalidAnnotationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
