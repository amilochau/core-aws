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
using System;
using Amazon.Runtime.Endpoints;

namespace Amazon.Runtime.Internal
{
    /// <summary>
    /// Custom PipelineHandler responsible for resolving endpoint and setting authentication parameters for service requests.
    /// Collects values for EndpointParameters and then resolves endpoint via global or service-specific EndpointProvider.
    /// Responsible for setting authentication and http headers provided by resolved endpoint.
    /// </summary>
    public abstract class BaseEndpointResolver : PipelineHandler
    {
        public override System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            PreInvoke(executionContext);
            return base.InvokeAsync<T>(executionContext);
        }

        protected virtual void PreInvoke(IExecutionContext executionContext)
        {
            ProcessRequestHandlers(executionContext);
        }

        public virtual void ProcessRequestHandlers(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;
            var parameters = MapEndpointsParameters(requestContext);

            var endpoint = GetEndpoint(executionContext, parameters);
            requestContext.Request.Endpoint = new Uri(endpoint.URL);
        }

        private Endpoint GetEndpoint(IExecutionContext executionContext, EndpointParameters parameters)
        {
            var requestContext = executionContext.RequestContext;
            var config = requestContext.ClientConfig;
            Endpoint endpoint = null;

            if (config.EndpointProvider != null)
            {
                endpoint = config.EndpointProvider.ResolveEndpoint(parameters);
            }

            // Ensure url ends with "/" to avoid signature mismatch issues.
            if (!endpoint.URL.EndsWith("/") && (string.IsNullOrEmpty(requestContext.Request.ResourcePath) || requestContext.Request.ResourcePath == "/"))
            {
                endpoint.URL += "/";
            }
            return endpoint;
        }

        /// <summary>
        /// Service-specific mapping of endpoints parameters, we code-gen override per service.
        /// </summary>
        protected abstract EndpointParameters MapEndpointsParameters(IRequestContext requestContext);
    }
}