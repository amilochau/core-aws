using Milochau.Core.Aws.Core.Runtime;

namespace Milochau.Core.Aws.SESv2
{
    /// <summary>
    /// Configuration for accessing Amazon SimpleEmailServiceV2 service
    /// </summary>
    public class AmazonSimpleEmailServiceV2Config : ClientConfig
    {
        /// <summary>Constructor</summary>
        public AmazonSimpleEmailServiceV2Config()
        {
            AuthenticationServiceName = "ses";
            MonitoringServiceName = "SimpleEmailV2";
        }
    }
}