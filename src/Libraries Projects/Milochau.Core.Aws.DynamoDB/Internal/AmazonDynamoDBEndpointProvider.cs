using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Runtime.Endpoints;

namespace Milochau.Core.Aws.DynamoDB.Internal
{
    /// <summary>
    /// Amazon DynamoDB endpoint provider.
    /// Resolves endpoint for given set of DynamoDBEndpointParameters.
    /// Can throw AmazonClientException if endpoint resolution is unsuccessful.
    /// </summary>
    public class AmazonDynamoDBEndpointProvider : IEndpointProvider
    {
        /// <summary>
        /// Resolve endpoint for DynamoDBEndpointParameters
        /// </summary>
        public Endpoint ResolveEndpoint()
        {
            string region = EnvironmentVariables.RegionName;
            return new Endpoint($"https://dynamodb.{region}.amazonaws.com");
        }
    }
}
