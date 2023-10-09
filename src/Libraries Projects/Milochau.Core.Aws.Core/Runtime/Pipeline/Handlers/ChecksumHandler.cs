namespace Milochau.Core.Aws.Core.Runtime.Pipeline.Handlers
{
    /// <summary>
    /// This handler manages the calculation of the checksum of the payload request when the checksum of a request is supported.
    /// It also sets the checksum header of the request.
    /// </summary>
    public class ChecksumHandler : PipelineHandler
    {
        /// <summary>
        /// Calls pre invoke logic before calling the next handler 
        /// in the pipeline.
        /// </summary>
        /// <typeparam name="T">The response type for the current request.</typeparam>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            PreInvoke(executionContext);
            return base.InvokeAsync<T>(executionContext);
        }

        /// <summary>
        /// Calculates the checksum of the payload of a request, and sets the checksum request header only once.
        /// </summary>
        /// <param name="executionContext">The execution context which contains both the
        /// request and response context.</param>
        protected virtual void PreInvoke(IExecutionContext executionContext)
        {
        }
    }
}
