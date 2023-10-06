using Amazon.Runtime.Endpoints;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Internal/AmazonDynamoDBEndpointProvider.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Internal
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
