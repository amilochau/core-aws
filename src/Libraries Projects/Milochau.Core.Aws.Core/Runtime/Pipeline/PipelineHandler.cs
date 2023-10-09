using System;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline
{
    /// <summary>
    /// An abstract pipeline handler that has implements IPipelineHandler,
    /// and has the default implmentation. This is the base class for most of
    /// the handler implementations.
    /// </summary>    
    public abstract partial class PipelineHandler : IPipelineHandler
    {
        /// <summary>
        /// The inner handler which is called after the current 
        /// handler completes it's processing.
        /// </summary>
        public IPipelineHandler InnerHandler { get; set; }

        /// <summary>
        /// The outer handler which encapsulates the current handler.
        /// </summary>
        public IPipelineHandler OuterHandler { get; set; }

        /// <summary>
        /// Contains the processing logic for an asynchronous request invocation.
        /// This method calls InnerHandler.InvokeSync to continue processing of the
        /// request by the pipeline.
        /// </summary>
        /// <typeparam name="T">The response type for the current request.</typeparam>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
            where T : AmazonWebServiceResponse, new()
        {
            if (this.InnerHandler != null)
            {
                return InnerHandler.InvokeAsync<T>(executionContext);    
            }
            throw new InvalidOperationException("Cannot invoke InnerHandler. InnerHandler is not set.");
        }
    }
}
