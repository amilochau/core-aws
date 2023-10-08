using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Endpoints;

namespace Milochau.Core.Aws.Lambda.Internal
{
    /// <summary>
    /// Amazon Lambda endpoint resolver.
    /// Custom PipelineHandler responsible for resolving endpoint and setting authentication parameters for Lambda service requests.
    /// Collects values for LambdaEndpointParameters and then tries to resolve endpoint by calling 
    /// ResolveEndpoint method on GlobalEndpoints.Provider if present, otherwise uses LambdaEndpointProvider.
    /// Responsible for setting authentication and http headers provided by resolved endpoint.
    /// </summary>
    public class AmazonLambdaEndpointResolver : BaseEndpointResolver
    {
        /// <inheritdoc/>
        protected override EndpointParameters MapEndpointsParameters(IRequestContext requestContext)
        {
            var config = (AmazonLambdaConfig)requestContext.ClientConfig;
            return new EndpointParameters
            {
                Region = config.RegionEndpoint.SystemName
            };
        }
    }
}