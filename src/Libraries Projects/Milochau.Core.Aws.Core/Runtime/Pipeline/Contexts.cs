using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Pipeline;
using System.Net.Http;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline
{
    public interface IRequestContext
    {
        string MonitoringOriginalRequestName { get; }
        AmazonWebServiceRequest OriginalRequest { get; }
        IClientConfig ClientConfig { get; }

        HttpRequestMessage HttpRequestMessage { get; set; }
    }

    public interface IResponseContext
    {
        AmazonWebServiceResponse? Response { get; set; }
        HttpResponseMessage? HttpResponse { get; set; }
    }
}

namespace Amazon.Runtime.Internal
{
    public class RequestContext : IRequestContext
    {
        public RequestContext(IClientConfig clientConfig,
            AmazonWebServiceRequest originalRequest,
            string monitoringOriginalRequestName)
        {
            ClientConfig = clientConfig;
            OriginalRequest = originalRequest;
            MonitoringOriginalRequestName = monitoringOriginalRequestName;
        }

        public IClientConfig ClientConfig { get; }
        public AmazonWebServiceRequest OriginalRequest { get; }
        public string MonitoringOriginalRequestName { get; }

        public required HttpRequestMessage HttpRequestMessage { get; set; }
    }

    public class ResponseContext : IResponseContext
    {
        public AmazonWebServiceResponse? Response { get; set; }        
        public HttpResponseMessage? HttpResponse { get; set; }
    }
}
