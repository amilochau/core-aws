using Amazon.Runtime.Endpoints;
using Amazon.Runtime.Internal;
using Amazon.Runtime;
using Amazon.Util;
using Amazon;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Internal/AmazonDynamoDBEndpointResolver.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Internal
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
        protected override void ServiceSpecificHandler(IExecutionContext executionContext, EndpointParameters parameters)
        {

            InjectHostPrefix(executionContext.RequestContext);
        }

        /// <inheritdoc/>
        protected override EndpointParameters MapEndpointsParameters(IRequestContext requestContext)
        {
            var config = (AmazonDynamoDBConfig)requestContext.ClientConfig;
            var result = new DynamoDBEndpointParameters();
            result.Region = config.RegionEndpoint?.SystemName!;
            result.UseDualStack = config.UseDualstackEndpoint;
            result.UseFIPS = config.UseFIPSEndpoint;

            return result;
        }
    }
}
