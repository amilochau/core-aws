using Amazon.Runtime.Endpoints;

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
        public Endpoint ResolveEndpoint(EndpointParameters parameters)
        {
            string region = parameters.Region;
            var dnsSuffix = "amazonaws.com";
            return new Endpoint($"https://dynamodb.{region}.{dnsSuffix}");
        }
    }
}
