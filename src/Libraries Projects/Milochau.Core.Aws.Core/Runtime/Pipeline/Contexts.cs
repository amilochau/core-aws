﻿using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Internal.Auth;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Runtime.Pipeline;
using System;
using System.Net.Http;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline
{
    public interface IRequestContext
    {
        AmazonWebServiceRequest OriginalRequest { get; }
        IMarshaller<IRequest, AmazonWebServiceRequest> Marshaller { get; }
        IHttpRequestMessageMarshaller<AmazonWebServiceRequest>? HttpRequestMessageMarshaller { get; }
        ResponseUnmarshaller Unmarshaller { get; }
        AWSSigner Signer { get; }
        IClientConfig ClientConfig { get; }

        IRequest? Request { get; set; }
        HttpRequestMessage? HttpRequestMessage { get; set; }

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
        public RequestContext(AWSSigner signer,
            IClientConfig clientConfig,
            IMarshaller<IRequest, AmazonWebServiceRequest> marshaller,
            IHttpRequestMessageMarshaller<AmazonWebServiceRequest>? httpRequestMessageMarshaller,
            ResponseUnmarshaller unmarshaller,
            AmazonWebServiceRequest originalRequest,
            System.Threading.CancellationToken cancellationToken)
        {
            Signer = signer;
            ClientConfig = clientConfig;
            Marshaller = marshaller;
            HttpRequestMessageMarshaller = httpRequestMessageMarshaller;
            Unmarshaller = unmarshaller;
            OriginalRequest = originalRequest;
            CancellationToken = cancellationToken;
        }

        public Guid InvocationId { get; } = Guid.NewGuid();

        public AWSSigner Signer { get; }
        public IClientConfig ClientConfig { get; }
        public IMarshaller<IRequest, AmazonWebServiceRequest> Marshaller { get; }
        public IHttpRequestMessageMarshaller<AmazonWebServiceRequest>? HttpRequestMessageMarshaller { get; }
        public ResponseUnmarshaller Unmarshaller { get; }
        public AmazonWebServiceRequest OriginalRequest { get; }
        public System.Threading.CancellationToken CancellationToken { get; }

        public IRequest? Request { get; set; }
        public HttpRequestMessage? HttpRequestMessage { get; set; }

        public int Retries { get; set; }
        public CapacityManager.CapacityType LastCapacityType { get; set; } = CapacityManager.CapacityType.Increment;
        public int EndpointDiscoveryRetries { get; set; }
        public bool IsSigned { get; set; }
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
