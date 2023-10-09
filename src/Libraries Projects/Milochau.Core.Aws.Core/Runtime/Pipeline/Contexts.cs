using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Runtime.Credentials;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Internal.Auth;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Runtime.Pipeline;
using System;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline
{
    public interface IRequestContext
    {
        AmazonWebServiceRequest OriginalRequest { get; }
        IMarshaller<IRequest, AmazonWebServiceRequest> Marshaller { get; }
        ResponseUnmarshaller Unmarshaller { get; }
        AbstractAWSSigner Signer { get; }
        IClientConfig ClientConfig { get; }
        ImmutableCredentials? ImmutableCredentials { get; set; }

        IRequest? Request { get; set; }
        bool IsSigned { get; set; }
        int Retries { get; set; }
        CapacityManager.CapacityType LastCapacityType { get; set; }
        int EndpointDiscoveryRetries { get; set; }

        System.Threading.CancellationToken CancellationToken { get; }

        Guid InvocationId { get; }
    }

    public interface IResponseContext
    {
        AmazonWebServiceResponse? Response { get; set; }
        IWebResponseData? HttpResponse { get; set; }
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
        public RequestContext(AbstractAWSSigner signer,
            IClientConfig clientConfig,
            IMarshaller<IRequest, AmazonWebServiceRequest> marshaller,
            ResponseUnmarshaller unmarshaller,
            AmazonWebServiceRequest originalRequest,
            System.Threading.CancellationToken cancellationToken)
        {
            Signer = signer;
            ClientConfig = clientConfig;
            Marshaller = marshaller;
            Unmarshaller = unmarshaller;
            OriginalRequest = originalRequest;
            CancellationToken = cancellationToken;
        }

        public Guid InvocationId { get; } = Guid.NewGuid();

        public AbstractAWSSigner Signer { get; }
        public IClientConfig ClientConfig { get; }
        public IMarshaller<IRequest, AmazonWebServiceRequest> Marshaller { get; }
        public ResponseUnmarshaller Unmarshaller { get; }
        public AmazonWebServiceRequest OriginalRequest { get; }
        public System.Threading.CancellationToken CancellationToken { get; }

        public IRequest? Request { get; set; }
        public int Retries { get; set; }
        public CapacityManager.CapacityType LastCapacityType { get; set; } = CapacityManager.CapacityType.Increment;
        public int EndpointDiscoveryRetries { get; set; }
        public bool IsSigned { get; set; }
        public ImmutableCredentials? ImmutableCredentials { get; set; }
    }

    public class ResponseContext : IResponseContext
    {
        public AmazonWebServiceResponse? Response { get; set; }        
        public IWebResponseData? HttpResponse { get; set; }
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
