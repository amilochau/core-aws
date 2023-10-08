namespace Amazon.XRay.Recorder.Core.Sampling
{
    /// <summary>
    /// Decisions for sampling
    /// </summary>
    public enum SampleDecision
    {
        /// <summary>
        /// Decision unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Tracing is enabled
        /// </summary>
        Sampled = '1',

        /// <summary>
        /// Tracing is disabled
        /// </summary>
        NotSampled = '0',

        /// <summary>
        /// The decision is not made, but need to be made
        /// </summary>
        Requested = '?'
    }
}
