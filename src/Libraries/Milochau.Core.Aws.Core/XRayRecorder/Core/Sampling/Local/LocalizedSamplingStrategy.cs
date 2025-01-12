namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Sampling.Local
{
    /// <summary>
    /// This strategy loads the sample Rules from local JSON file, and make the sample decision locally
    /// according to the Rules.
    /// </summary>
    public class LocalizedSamplingStrategy
    {
        /// <summary>
        /// Gets the rate limiter which had the limit set to fixed target rate
        /// </summary>
        public RateLimiter RateLimiter { get; private set; } = new RateLimiter(1);

        /// <summary>
        /// Perform sampling decison.
        /// </summary>
        public SampleDecision ShouldTrace()
        {
            return RateLimiter.Request() ? SampleDecision.Sampled : SampleDecision.NotSampled;
        }
    }
}
