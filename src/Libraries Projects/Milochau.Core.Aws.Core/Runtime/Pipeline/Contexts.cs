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
using Amazon.Runtime.Internal.Util;
using System;

namespace Amazon.Runtime
{
    public interface IRequestContext
    {
        AmazonWebServiceRequest OriginalRequest { get; }
        string RequestName { get; }
        IMarshaller<IRequest, AmazonWebServiceRequest> Marshaller { get; }
        ResponseUnmarshaller Unmarshaller { get; }
        InvokeOptionsBase Options { get; }        
        RequestMetrics Metrics { get; }
        AbstractAWSSigner Signer { get; }
        IClientConfig ClientConfig { get; }
        ImmutableCredentials ImmutableCredentials { get; set; }

        IRequest Request { get; set; }
        bool IsSigned { get; set; }
        bool IsAsync { get; }
        int Retries { get; set; }
        CapacityManager.CapacityType LastCapacityType { get; set; }
        int EndpointDiscoveryRetries { get; set; }

        System.Threading.CancellationToken CancellationToken { get; }
        MonitoringAPICallAttempt CSMCallAttempt { get; set; }
        MonitoringAPICallEvent CSMCallEvent { get; set; }
        IServiceMetadata ServiceMetaData { get; }

        bool CSMEnabled { get; }
        bool IsLastExceptionRetryable { get; set; }

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
        private IServiceMetadata _serviceMetadata;
        AbstractAWSSigner clientSigner;

        public RequestContext(bool enableMetrics, AbstractAWSSigner clientSigner)
        {
            if (clientSigner == null)
                throw new ArgumentNullException("clientSigner");

            this.clientSigner = clientSigner;
            this.Metrics = new RequestMetrics();
            this.Metrics.IsEnabled = enableMetrics;
            this.InvocationId = Guid.NewGuid();
        }

        public IRequest Request { get; set; }
        public RequestMetrics Metrics { get; private set; }
        public IClientConfig ClientConfig { get; set; }
        public int Retries { get; set; }
        public CapacityManager.CapacityType LastCapacityType { get; set; } = CapacityManager.CapacityType.Increment;
        public int EndpointDiscoveryRetries { get; set; }
        public bool IsSigned { get; set; }
        public bool IsAsync { get; set; }
        public AmazonWebServiceRequest OriginalRequest { get; set; }
        public IMarshaller<IRequest, AmazonWebServiceRequest> Marshaller { get; set; }
        public ResponseUnmarshaller Unmarshaller { get; set; }
        public InvokeOptionsBase Options { get; set; }        
        public ImmutableCredentials ImmutableCredentials { get; set; }
        public AbstractAWSSigner Signer
        {
            get
            {
                var requestSigner = OriginalRequest == null ? null : OriginalRequest.GetSigner();
                if (requestSigner == null)
                    return clientSigner;
                else
                    return requestSigner;
            }
        }

        public System.Threading.CancellationToken CancellationToken { get; set; }

        public string RequestName
        {
            get { return this.OriginalRequest.GetType().Name; }
        }
        public MonitoringAPICallAttempt CSMCallAttempt { get; set; }

        public MonitoringAPICallEvent CSMCallEvent { get; set; }
        public IServiceMetadata ServiceMetaData
        {
            get
            {
                return _serviceMetadata;
            }
            internal set
            {
                _serviceMetadata = value;
                // The CSMEnabled flag is referred in the runtime pipeline before capturing any CSM data.
                // Along with the customer set CSMEnabled flag, the ServiceMetadata.ServiceId needs to be set
                // to capture client side metrics. Older service nuget packages might not have a ServiceMetadata
                // implementation and in such cases client side metrics will not be captured.
                CSMEnabled = DeterminedCSMConfiguration.Instance.CSMConfiguration.Enabled && !string.IsNullOrEmpty(_serviceMetadata.ServiceId);
            }
        }

        public bool CSMEnabled { get; private set; }
        /// <summary>
        /// Property to denote that the last exception returned by an AWS Service
        /// was retryable or not.
        /// </summary>
        public bool IsLastExceptionRetryable { get; set; }

        public Guid InvocationId { get; private set; }
    }

    public class AsyncRequestContext : RequestContext, IRequestContext
    {
        public AsyncRequestContext(bool enableMetrics, AbstractAWSSigner clientSigner):
            base(enableMetrics, clientSigner)
        {
        }
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
