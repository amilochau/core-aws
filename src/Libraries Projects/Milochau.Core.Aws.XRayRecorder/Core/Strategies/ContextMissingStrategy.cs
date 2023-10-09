namespace Milochau.Core.Aws.XRayRecorder.Core.Strategies
{
    /// <summary>
    /// Represent all modes of ContextMissingStrategy.
    /// </summary>
    public enum ContextMissingStrategy
    {
        /// <summary>
        /// The EntityNotAvailableException will be thrown if occurs at runtime.
        /// </summary>
        RUNTIME_ERROR = 1,

        /// <summary>
        /// The EntityNotAvailableException will be logged if occurs.
        /// </summary>
        LOG_ERROR = 0,  // Set to 0, so it is default value
    }
}
