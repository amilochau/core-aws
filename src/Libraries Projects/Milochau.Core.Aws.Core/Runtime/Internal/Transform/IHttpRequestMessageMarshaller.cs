using System.Net.Http;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Transform
{
    public interface IHttpRequestMessageMarshaller<R>
    {
        HttpRequestMessage CreateHttpRequestMessage(R input);
    }
}
