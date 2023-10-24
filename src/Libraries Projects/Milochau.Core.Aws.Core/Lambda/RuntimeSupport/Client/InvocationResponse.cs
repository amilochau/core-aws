using System;
using System.IO;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client
{
    /// <summary>
    /// Class that contains the response for an invocation of an AWS Lambda function.
    /// </summary>
    internal class InvocationResponse
    {
        /// <summary>
        /// Output from the function invocation.
        /// </summary>
        public Stream OutputStream { get; set; }

        /// <summary>
        /// True if the LambdaBootstrap should dispose the stream after it's read, false otherwise.
        /// Set this to false if you plan to reuse the same output stream for multiple invocations of the function.
        /// </summary>
        public bool DisposeOutputStream { get; private set; } = true;

        public InvocationResponse(Stream outputStream)
            : this(outputStream, true)
        { }

        public InvocationResponse(Stream outputStream, bool disposeOutputStream)
        {
            OutputStream = outputStream ?? throw new ArgumentNullException(nameof(outputStream));
            DisposeOutputStream = disposeOutputStream;
        }
    }
}
