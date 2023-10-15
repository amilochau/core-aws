namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Context
{
    /// <summary></summary>
    public static class DefaultTraceContext
    {
        /// <summary>
        /// Gets default instance of <see cref="ITraceContext"/>.
        /// </summary>
        /// <returns>default instance of <see cref="ITraceContext"/></returns>
        public static ITraceContext GetTraceContext()
        {
            return new LambdaContextContainer();
        }
    }
}
