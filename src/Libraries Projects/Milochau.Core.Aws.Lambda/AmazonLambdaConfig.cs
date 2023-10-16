using Milochau.Core.Aws.Core.Runtime;

namespace Milochau.Core.Aws.Lambda
{
    /// <summary>
    /// Configuration for accessing Amazon Lambda service
    /// </summary>
    public class AmazonLambdaConfig : ClientConfig
    {
        /// <summary>Constructor</summary>
        public AmazonLambdaConfig()
        {
            AuthenticationServiceName = "lambda";
            MonitoringServiceName = "Amazon.Lambda";
        }
    }
}