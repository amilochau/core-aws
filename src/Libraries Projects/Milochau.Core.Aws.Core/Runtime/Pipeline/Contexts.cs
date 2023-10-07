/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 * 
 *  http://aws.amazon.com/apache2.0
 * 
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.Internal.Transform;
using System;

namespace Amazon.Runtime
{
    public interface IRequestContext
    {
        AmazonWebServiceRequest OriginalRequest { get; }
        string RequestName { get; }
        IMarshaller<IRequest, AmazonWebServiceRequest> Marshaller { get; }
        ResponseUnmarshaller Unmarshaller { get; }
        AbstractAWSSigner Signer { get; }
        IClientConfig ClientConfig { get; }
        ImmutableCredentials ImmutableCredentials { get; set; }

        IRequest Request { get; set; }
        bool IsSigned { get; set; }
        int Retries { get; set; }
        CapacityManager.CapacityType LastCapacityType { get; set; }
        int EndpointDiscoveryRetries { get; set; }

        System.Threading.CancellationToken CancellationToken { get; }

        Guid InvocationId { get; }
    }

    public interface IResponseContext
    {
        AmazonWebServiceResponse Response { get; set; }
        IWebResponseData HttpResponse { get; set; }
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
        public RequestContext(AbstractAWSSigner clientSigner)
        {
            if (clientSigner == null)
                throw new ArgumentNullException("clientSigner");

            this.Signer = clientSigner;
            this.InvocationId = Guid.NewGuid();
        }

        public IRequest Request { get; set; }
        public IClientConfig ClientConfig { get; set; }
        public int Retries { get; set; }
        public CapacityManager.CapacityType LastCapacityType { get; set; } = CapacityManager.CapacityType.Increment;
        public int EndpointDiscoveryRetries { get; set; }
        public bool IsSigned { get; set; }
        public AmazonWebServiceRequest OriginalRequest { get; set; }
        public IMarshaller<IRequest, AmazonWebServiceRequest> Marshaller { get; set; }
        public ResponseUnmarshaller Unmarshaller { get; set; }
        public ImmutableCredentials ImmutableCredentials { get; set; }
        public AbstractAWSSigner Signer { get; }

        public System.Threading.CancellationToken CancellationToken { get; set; }

        public string RequestName
        {
            get { return this.OriginalRequest.GetType().Name; }
        }

        public Guid InvocationId { get; private set; }
    }

    public class ResponseContext : IResponseContext
    {
        public AmazonWebServiceResponse Response { get; set; }        
        public IWebResponseData HttpResponse { get; set; }
    }

    public class ExecutionContext : IExecutionContext
    {
        public IRequestContext RequestContext { get; private set; }
        public IResponseContext ResponseContext { get; private set; }

        public ExecutionContext(IRequestContext requestContext, IResponseContext responseContext)
        {
            this.RequestContext = requestContext;
            this.ResponseContext = responseContext;
        }
    }
}
