namespace Milochau.Core.Aws.Core.Runtime.Pipeline
{
    /// <summary>
    /// Interface for a handler in a pipeline.
    /// </summary>
    public partial interface IPipelineHandler
    {
        /// <summary>
        /// The inner handler which is called after the current 
        /// handler completes it's processing.
        /// </summary>
        IPipelineHandler InnerHandler { get; set; }

        /// <summary>
        /// The outer handler which encapsulates the current handler.
        /// </summary>
        IPipelineHandler OuterHandler { get; set; }

        /// <summary>
        /// Contains the processing logic for an asynchronous request invocation.
        /// This method should call InnerHandler.InvokeSync to continue processing of the
        /// request by the pipeline, unless it's a terminating handler.
        /// </summary>
        /// <typeparam name="T">The response type for the current request.</typeparam>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
            where T : AmazonWebServiceResponse, new();
    }
}
