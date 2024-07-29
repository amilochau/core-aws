using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Runtime.Credentials;
using Milochau.Core.Aws.Core.Runtime.Internal;
using System.Net.Http;

namespace Amazon.Runtime.Internal
{
    public class RequestContext(ClientConfig clientConfig, AmazonWebServiceRequest originalRequest, string monitoringOriginalRequestName)
    {
        public ClientConfig ClientConfig { get; } = clientConfig;
        public AmazonWebServiceRequest OriginalRequest { get; } = originalRequest;
        public string MonitoringOriginalRequestName { get; } = monitoringOriginalRequestName;

        public required HttpRequestMessage HttpRequestMessage { get; set; }

        public required ImmutableCredentials ImmutableCredentials { get; set; }
    }

    public class ResponseContext
    {
        public AmazonWebServiceResponse? Response { get; set; }
        public HttpResponseMessage? HttpResponse { get; set; }
    }
}
