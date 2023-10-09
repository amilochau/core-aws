using System;
using System.IO;
using System.Net;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Transform
{
    public interface IWebResponseData
    {
        long ContentLength { get; }
        string ContentType { get; }
        HttpStatusCode StatusCode { get; }
        bool IsHeaderPresent(string headerName);
        string GetHeaderValue(string headerName);

        IHttpResponseBody ResponseBody { get; }
    }

    public interface IHttpResponseBody : IDisposable
    {
        Stream OpenResponse();

        System.Threading.Tasks.Task<Stream> OpenResponseAsync();
    }
}
