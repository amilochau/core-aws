using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Util;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.Handlers
{
    /// <summary>
    /// This handler signs the request.
    /// </summary>
    public class Signer : PipelineHandler
    {
        /// <summary>
        /// Calls pre invoke logic before calling the next handler 
        /// in the pipeline.
        /// </summary>
        /// <typeparam name="T">The response type for the current request.</typeparam>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            await PreInvokeAsync(executionContext);
            return await base.InvokeAsync<T>(executionContext).ConfigureAwait(false);
        }

        protected static async Task PreInvokeAsync(IExecutionContext executionContext)
        {
            if (!executionContext.RequestContext.IsSigned)
            {
                await SignRequestAsync(executionContext.RequestContext);
                executionContext.RequestContext.IsSigned = true;
            }
        }

        /// <summary>
        /// Signs the request.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        private static Task SignRequestAsync(IRequestContext requestContext)
        {
            if (EnvironmentVariables.UseToken)
            {
                requestContext.HttpRequestMessage.Headers.Add(HeaderKeys.XAmzSecurityTokenHeader, EnvironmentVariables.Token);
            }

            return requestContext.Signer.SignAsync(requestContext);
        }
    }
}
