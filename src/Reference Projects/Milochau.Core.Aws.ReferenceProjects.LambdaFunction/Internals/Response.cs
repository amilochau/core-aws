using System;
using System.Collections.Generic;
using System.IO;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals
{
    public class StatusResponse
    {
        public string? Status { get; set; }
    }

    public class ErrorResponse
    {
        public string? ErrorMessage { get; set; }
        public string? ErrorType { get; set; }
    }

    public class SwaggerResponse
    {
        public int StatusCode { get; private set; }

        public Dictionary<string, IEnumerable<string>> Headers { get; private set; }

        public SwaggerResponse(int statusCode, Dictionary<string, IEnumerable<string>> headers)
        {
            StatusCode = statusCode;
            Headers = headers;
        }
    }

    public class SwaggerResponse<TResult> : SwaggerResponse
    {
        public TResult Result { get; private set; }

        public SwaggerResponse(int statusCode, Dictionary<string, IEnumerable<string>> headers, TResult result)
            : base(statusCode, headers)
        {
            Result = result;
        }
    }

    /// <summary>
    /// Class that contains the response for an invocation of an AWS Lambda function.
    /// </summary>
    public class InvocationResponse
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
