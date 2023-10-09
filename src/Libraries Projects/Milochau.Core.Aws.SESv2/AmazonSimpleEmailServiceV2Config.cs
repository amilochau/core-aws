using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Util.Internal;
using Milochau.Core.Aws.SESv2.Internal;

namespace Milochau.Core.Aws.SESv2
{
    /// <summary>
    /// Configuration for accessing Amazon SimpleEmailServiceV2 service
    /// </summary>
    public partial class AmazonSimpleEmailServiceV2Config : ClientConfig
    {
        private static readonly string UserAgentString =
            InternalSDKUtils.BuildUserAgentString("3.7.201.22");

        private readonly string _userAgent = UserAgentString;

        /// <summary>
        /// Default constructor
        /// </summary>
        public AmazonSimpleEmailServiceV2Config()
            : base()
        {
            this.AuthenticationServiceName = "ses";
            this.EndpointProvider = new AmazonSimpleEmailServiceV2EndpointProvider();
        }

        /// <summary>
        /// The constant used to lookup in the region hash the endpoint.
        /// </summary>
        public override string RegionEndpointServiceName => "email";

        /// <summary>
        /// Gets the value of UserAgent property.
        /// </summary>
        public override string UserAgent => _userAgent;
    }
}