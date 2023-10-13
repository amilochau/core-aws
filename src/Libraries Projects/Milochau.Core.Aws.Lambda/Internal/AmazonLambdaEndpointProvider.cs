using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Runtime.Endpoints;

namespace Milochau.Core.Aws.Lambda.Internal
{
    /// <summary>
    /// Amazon Lambda endpoint provider.
    /// Resolves endpoint for given set of LambdaEndpointParameters.
    /// Can throw AmazonClientException if endpoint resolution is unsuccessful.
    /// </summary>
    public class AmazonLambdaEndpointProvider : IEndpointProvider
    {
        /// <summary>
        /// Resolve endpoint for LambdaEndpointParameters
        /// </summary>
        public Endpoint ResolveEndpoint()
        {
            string region = EnvironmentVariables.RegionName;
            return new Endpoint($"https://lambda.{region}.amazonaws.com");
        }
    }
}