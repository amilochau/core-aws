using System;
using System.Net.Http;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline
{
    public class HttpErrorResponseException : Exception
    {
        public HttpResponseMessage Response { get; private set; }

        public HttpErrorResponseException(HttpResponseMessage response)
        {
            Response = response;
        }

        public HttpErrorResponseException(string message, HttpResponseMessage response) :
            base(message)
        {
            Response = response;
        }

        public HttpErrorResponseException(string message, Exception innerException, HttpResponseMessage response) :
            base(message,innerException)
        {
            Response = response;
        }
    }
}
