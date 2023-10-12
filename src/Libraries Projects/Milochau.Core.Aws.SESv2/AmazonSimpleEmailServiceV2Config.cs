using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.SESv2.Internal;

namespace Milochau.Core.Aws.SESv2
{
    /// <summary>
    /// Configuration for accessing Amazon SimpleEmailServiceV2 service
    /// </summary>
    public partial class AmazonSimpleEmailServiceV2Config : ClientConfig
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AmazonSimpleEmailServiceV2Config()
            : base()
        {
            AuthenticationServiceName = "ses";
            EndpointProvider = new AmazonSimpleEmailServiceV2EndpointProvider();
        }
    }
}