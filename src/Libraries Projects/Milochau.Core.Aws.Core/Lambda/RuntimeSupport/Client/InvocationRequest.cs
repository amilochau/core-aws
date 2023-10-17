using Milochau.Core.Aws.Core.Lambda.Core;
using System;
using System.IO;
using System.Net.Http;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client
{
    /// <summary>
    /// Class that contains all the information necessary to handle an invocation of an AWS Lambda function.
    /// </summary>
    public class InvocationRequest : IDisposable
    {
        private readonly HttpResponseMessage response;

        public InvocationRequest(HttpResponseMessage response)
        {
            this.response = response;
        }

        /// <summary>
        /// Input to the function invocation.
        /// </summary>
        public Stream InputStream { get; internal set; }

        /// <summary>
        /// Context for the invocation.
        /// </summary>
        public ILambdaContext LambdaContext { get; internal set; }

        internal InvocationRequest() { }

        public void Dispose()
        {
            response?.Dispose();
            InputStream?.Dispose();
        }
    }
}
