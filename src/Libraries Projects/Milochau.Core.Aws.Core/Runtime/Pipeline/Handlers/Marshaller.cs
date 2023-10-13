using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Util;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.Handlers
{
    /// <summary>
    /// This handler marshalls the request before calling invoking the next handler.
    /// </summary>
    public class Marshaller : PipelineHandler
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
        /// Marshalls the request before calling invoking the next handler.
        /// </summary>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        protected static void PreInvoke(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;
            requestContext.Request = requestContext.Marshaller.Marshall(requestContext.OriginalRequest);
            requestContext.HttpRequestMessage = requestContext.HttpRequestMessageMarshaller.CreateHttpRequestMessage(requestContext.OriginalRequest);

            if (EnvironmentVariables.TryGetEnvironmentVariable(EnvironmentVariables.Key_TraceId, out string? amznTraceId))
            {
                requestContext.Request.Headers[HeaderKeys.XAmznTraceIdHeader] = AWSSDKUtils.EncodeTraceIdHeaderValue(amznTraceId);
                requestContext.HttpRequestMessage.Headers.Add(HeaderKeys.XAmznTraceIdHeader, amznTraceId);
            }
        }
    }
}
