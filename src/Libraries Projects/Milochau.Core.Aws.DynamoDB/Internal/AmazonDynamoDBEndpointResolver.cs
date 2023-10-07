using Amazon.Runtime.Endpoints;
using Amazon.Runtime.Internal;
using Amazon.Runtime;

namespace Milochau.Core.Aws.DynamoDB.Internal
{
    /// <summary>
    /// Amazon DynamoDB endpoint resolver.
    /// Custom PipelineHandler responsible for resolving endpoint and setting authentication parameters for DynamoDB service requests.
    /// Collects values for DynamoDBEndpointParameters and then tries to resolve endpoint by calling 
    /// ResolveEndpoint method on GlobalEndpoints.Provider if present, otherwise uses DynamoDBEndpointProvider.
    /// Responsible for setting authentication and http headers provided by resolved endpoint.
    /// </summary>
    public class AmazonDynamoDBEndpointResolver : BaseEndpointResolver
    {
        /// <inheritdoc/>
        protected override EndpointParameters MapEndpointsParameters(IRequestContext requestContext)
        {
            var config = (AmazonDynamoDBConfig)requestContext.ClientConfig;
            return new EndpointParameters
            {
                Region = config.RegionEndpoint?.SystemName
            };
        }
    }
}
