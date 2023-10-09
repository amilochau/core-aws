using Milochau.Core.Aws.Core.Runtime.Endpoints;
using System;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.Handlers
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