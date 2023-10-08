using Amazon.Runtime.Internal;
using Amazon.XRay.Recorder.Handlers.AwsSdk.Internal;

namespace Amazon.XRay.Recorder.Handlers.AwsSdk
{
    /// <summary>
    /// The AWS SDK handler to register X-Ray with <see cref="Runtime.AmazonServiceClient"/> which can intercept downstream requests and responses.
    /// </summary>
    public static class AWSSDKHandler
    {
        private static XRayPipelineCustomizer _customizer;

        /// <summary>
        /// Registers X-Ray for all instances of <see cref="Runtime.AmazonServiceClient"/>.
        /// </summary>
        public static void RegisterXRayForAllServices()
        {
            _customizer = GetCustomizer();
            _customizer.RegisterAll = true;
        }

        private static XRayPipelineCustomizer GetCustomizer()
        {
            if (_customizer == null)
            {
                _customizer = new XRayPipelineCustomizer();
                RuntimePipelineCustomizerRegistry.Instance.Register(_customizer);
            }

            return _customizer;
        }
    }
}
