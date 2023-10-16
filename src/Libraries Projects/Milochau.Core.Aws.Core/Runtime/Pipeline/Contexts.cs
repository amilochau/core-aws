using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Runtime.Pipeline;
using System.Net.Http;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline
{
    public interface IRequestContext
    {
        AmazonWebServiceRequest OriginalRequest { get; }
        IHttpRequestMessageMarshaller<AmazonWebServiceRequest> HttpRequestMessageMarshaller { get; }
        JsonResponseUnmarshaller Unmarshaller { get; }
        IClientConfig ClientConfig { get; }

        HttpRequestMessage? HttpRequestMessage { get; set; }

        System.Threading.CancellationToken CancellationToken { get; }
    }

    public interface IResponseContext
    {
        AmazonWebServiceResponse? Response { get; set; }
        HttpResponseMessage HttpResponse { get; set; }
    }

    public interface IExecutionContext
    {
        IResponseContext ResponseContext { get; }
        IRequestContext RequestContext { get; }
    }
}

namespace Amazon.Runtime.Internal
{
    public class RequestContext : IRequestContext
    {
        public RequestContext(IClientConfig clientConfig,
            IHttpRequestMessageMarshaller<AmazonWebServiceRequest> httpRequestMessageMarshaller,
            JsonResponseUnmarshaller unmarshaller,
            AmazonWebServiceRequest originalRequest,
            System.Threading.CancellationToken cancellationToken)
        {
            ClientConfig = clientConfig;
            HttpRequestMessageMarshaller = httpRequestMessageMarshaller;
            Unmarshaller = unmarshaller;
            OriginalRequest = originalRequest;
            CancellationToken = cancellationToken;
        }

        public IClientConfig ClientConfig { get; }
        public IHttpRequestMessageMarshaller<AmazonWebServiceRequest> HttpRequestMessageMarshaller { get; }
        public JsonResponseUnmarshaller Unmarshaller { get; }
        public AmazonWebServiceRequest OriginalRequest { get; }
        public System.Threading.CancellationToken CancellationToken { get; }

        public HttpRequestMessage? HttpRequestMessage { get; set; }
    }

    public class ResponseContext : IResponseContext
    {
        public AmazonWebServiceResponse? Response { get; set; }        
        public required HttpResponseMessage HttpResponse { get; set; }
    }

    public class ExecutionContext : IExecutionContext
    {
        public IRequestContext RequestContext { get; private set; }
        public IResponseContext ResponseContext { get; private set; }

        public ExecutionContext(IRequestContext requestContext, IResponseContext responseContext)
        {
            RequestContext = requestContext;
            ResponseContext = responseContext;
        }
    }
}
