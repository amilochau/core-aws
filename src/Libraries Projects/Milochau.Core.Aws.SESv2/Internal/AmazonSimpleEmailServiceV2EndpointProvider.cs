using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Runtime.Endpoints;

namespace Milochau.Core.Aws.SESv2.Internal
{
    /// <summary>
    /// Amazon SimpleEmailServiceV2 endpoint provider.
    /// Resolves endpoint for given set of SimpleEmailServiceV2EndpointParameters.
    /// Can throw AmazonClientException if endpoint resolution is unsuccessful.
    /// </summary>
    public class AmazonSimpleEmailServiceV2EndpointProvider : IEndpointProvider
    {
        /// <summary>
        /// Resolve endpoint for SimpleEmailServiceV2EndpointParameters
        /// </summary>
        public Endpoint ResolveEndpoint()
        {
            string region = EnvironmentVariables.RegionName;
            var dnsSuffix = "amazonaws.com";
            return new Endpoint($"https://email.{region}.{dnsSuffix}");
        }
    }
}