using Milochau.Core.Aws.Core.Runtime.Endpoints;
using System;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.Handlers
{
    /// <summary>
    /// Custom PipelineHandler responsible for resolving endpoint and setting authentication parameters for service requests.
    /// Collects values for EndpointParameters and then resolves endpoint via global or service-specific EndpointProvider.
    /// Responsible for setting authentication and http headers provided by resolved endpoint.
    /// </summary>
    public class EndpointResolver : PipelineHandler
    {
        public override System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            PreInvoke(executionContext);
            return base.InvokeAsync<T>(executionContext);
        }

        protected void PreInvoke(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;

            var endpoint = GetEndpoint(executionContext);
            requestContext.Request.Endpoint = new Uri(endpoint.URL);
        }

        private static Endpoint GetEndpoint(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;
            var endpoint = requestContext.ClientConfig.EndpointProvider.ResolveEndpoint();

            // Ensure url ends with "/" to avoid signature mismatch issues.
            if (!endpoint.URL.EndsWith("/") && (string.IsNullOrEmpty(requestContext.Request.ResourcePath) || requestContext.Request.ResourcePath == "/"))
            {
                endpoint.URL += "/";
            }
            return endpoint;
        }
    }
}