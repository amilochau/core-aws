using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Endpoints;

namespace Milochau.Core.Aws.SESv2.Internal
{
    /// <summary>
    /// Amazon SimpleEmailServiceV2 endpoint resolver.
    /// Custom PipelineHandler responsible for resolving endpoint and setting authentication parameters for SimpleEmailServiceV2 service requests.
    /// Collects values for SimpleEmailServiceV2EndpointParameters and then tries to resolve endpoint by calling 
    /// ResolveEndpoint method on GlobalEndpoints.Provider if present, otherwise uses SimpleEmailServiceV2EndpointProvider.
    /// Responsible for setting authentication and http headers provided by resolved endpoint.
    /// </summary>
    public class AmazonSimpleEmailServiceV2EndpointResolver : BaseEndpointResolver
    {
        /// <inheritdoc/>
        protected override EndpointParameters MapEndpointsParameters(IRequestContext requestContext)
        {
            var config = (AmazonSimpleEmailServiceV2Config)requestContext.ClientConfig;
            return new EndpointParameters
            {
                Region = config.RegionEndpoint?.SystemName
            };
        }
    }
}