using Milochau.Core.Aws.Core.Lambda.Core;
using System;
using System.IO;
using System.Net.Http;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client
{
    /// <summary>
    /// Class that contains all the information necessary to handle an invocation of an AWS Lambda function.
    /// </summary>
    internal class InvocationRequest(HttpResponseMessage response) : IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// Input to the function invocation.
        /// </summary>
        public required Stream InputStream { get; set; }

        /// <summary>
        /// Context for the invocation.
        /// </summary>
        public required ILambdaContext LambdaContext { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    response?.Dispose();
                    InputStream?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
