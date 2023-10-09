using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using System;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline
{
    public class HttpErrorResponseException : Exception
    {
        public IWebResponseData Response { get; private set; }

        public HttpErrorResponseException(IWebResponseData response)
        {
            Response = response;
        }

        public HttpErrorResponseException(string message, IWebResponseData response) :
            base(message)
        {
            Response = response;
        }

        public HttpErrorResponseException(string message, Exception innerException, IWebResponseData response) :
            base(message,innerException)
        {
            Response = response;
        }
    }
}
