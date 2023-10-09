using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Util.Internal;
using Milochau.Core.Aws.Lambda.Internal;

namespace Milochau.Core.Aws.Lambda
{
    /// <summary>
    /// Configuration for accessing Amazon Lambda service
    /// </summary>
    public partial class AmazonLambdaConfig : ClientConfig
    {
        private static readonly string UserAgentString =
            InternalSDKUtils.BuildUserAgentString("3.7.201.45");

        private readonly string _userAgent = UserAgentString;

        /// <summary>
        /// Default constructor
        /// </summary>
        public AmazonLambdaConfig()
            : base()
        {
            this.AuthenticationServiceName = "lambda";
            this.EndpointProvider = new AmazonLambdaEndpointProvider();
        }

        /// <summary>
        /// The constant used to lookup in the region hash the endpoint.
        /// </summary>
        public override string RegionEndpointServiceName => "lambda";

        /// <summary>
        /// Gets the value of UserAgent property.
        /// </summary>
        public override string UserAgent => _userAgent;
    }
}