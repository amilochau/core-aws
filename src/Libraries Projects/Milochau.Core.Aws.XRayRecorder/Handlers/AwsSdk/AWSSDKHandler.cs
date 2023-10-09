using Amazon.Runtime.Internal;
using Milochau.Core.Aws.XRayRecorder.Handlers.AwsSdk.Internal;

namespace Milochau.Core.Aws.XRayRecorder.Handlers.AwsSdk
{
    /// <summary>
    /// The AWS SDK handler to register X-Ray with <see cref="Amazon.Runtime.AmazonServiceClient"/> which can intercept downstream requests and responses.
    /// </summary>
    public static class AWSSDKHandler
    {
        /// <summary>
        /// Registers X-Ray for all instances of <see cref="Amazon.Runtime.AmazonServiceClient"/>.
        /// </summary>
        public static void RegisterXRayForAllServices()
        {
            var customizer = new XRayPipelineCustomizer();
            RuntimePipelineCustomizerRegistry.Instance.Register(customizer);
        }
    }
}
