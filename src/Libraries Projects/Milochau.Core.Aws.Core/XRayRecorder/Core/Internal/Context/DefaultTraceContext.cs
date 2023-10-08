namespace Amazon.XRay.Recorder.Core.Internal.Context
{
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
