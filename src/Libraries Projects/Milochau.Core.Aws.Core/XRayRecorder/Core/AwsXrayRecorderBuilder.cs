using Amazon.XRay.Recorder.Core.Strategies;

namespace Amazon.XRay.Recorder.Core
{
    /// <summary>
    /// This class provides utilities to build an instance of <see cref="AWSXRayRecorder"/> with different configurations.
    /// </summary>
    public class AWSXRayRecorderBuilder
    {
        /// <summary>
        /// Build a instance of <see cref="AWSXRayRecorder"/> with existing configuration added to the builder.
        /// </summary>
        /// <returns>A new instance of <see cref="AWSXRayRecorder"/>.</returns>
        public static AWSXRayRecorder Build()
        {
            return new AWSXRayRecorder
            {
                ContextMissingStrategy = ContextMissingStrategy.LOG_ERROR
            };
        }
    }
}
