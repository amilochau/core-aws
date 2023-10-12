using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Util;
using System.Collections.Generic;

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

            var method = requestContext.Request.HttpMethod.ToUpperInvariant();
            if (method != "GET" && method != "DELETE" && method != "HEAD")
            {
                if (!requestContext.Request.Headers.ContainsKey(HeaderKeys.ContentTypeHeader))
                {
                    if (requestContext.Request.UseQueryString)
                        requestContext.Request.Headers[HeaderKeys.ContentTypeHeader] = "application/x-amz-json-1.0";
                    else
                        requestContext.Request.Headers[HeaderKeys.ContentTypeHeader] = AWSSDKUtils.UrlEncodedContent;
                }
            }

            SetRecursionDetectionHeader(requestContext.Request.Headers);
        }

        /// <summary>
        /// Sets the X-Amzn-Trace-Id header for recursion detection within Lambda workloads.
        /// </summary>
        /// <param name="headers">Current request headers before marshalling.</param>
        private static void SetRecursionDetectionHeader(IDictionary<string, string> headers)
        {
            if (!headers.ContainsKey(HeaderKeys.XAmznTraceIdHeader))
            {                
                var amznTraceId = EnvironmentVariables.GetEnvironmentVariable(EnvironmentVariables.Key_TraceId);

                if (!string.IsNullOrEmpty(amznTraceId))
                {
                    headers[HeaderKeys.XAmznTraceIdHeader] = AWSSDKUtils.EncodeTraceIdHeaderValue(amznTraceId);
                }
            }
        }
    }
}
