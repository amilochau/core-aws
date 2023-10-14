using System.Net.Http;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Transform
{
    public interface IWebResponseData
    {
        bool IsHeaderPresent(string headerName);
        string GetHeaderValue(string headerName);

        HttpResponseMessage HttpResponseMessage { get; }
    }
}
