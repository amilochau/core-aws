using Milochau.Core.Aws.Core.Runtime.Credentials;
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
            await PreInvokeAsync(executionContext).ConfigureAwait(false);
            return await base.InvokeAsync<T>(executionContext).ConfigureAwait(false);
        }

        protected static async Task PreInvokeAsync(IExecutionContext executionContext)
        {
            if (ShouldSign(executionContext.RequestContext))
            {
                await SignRequestAsync(executionContext.RequestContext).ConfigureAwait(false);
                executionContext.RequestContext.IsSigned = true;
            }
        }

        /// <summary>
        /// Determines if the request should be signed.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <returns>A boolean value that indicated if the request should be signed.</returns>
        private static bool ShouldSign(IRequestContext requestContext)
        {
            return !requestContext.IsSigned;
        }

        /// <summary>
        /// Signs the request.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        private static async Task SignRequestAsync(IRequestContext requestContext)
        {
            ImmutableCredentials immutableCredentials = requestContext.ImmutableCredentials;

            // credentials would be null in the case of anonymous users getting public resources from S3
            if (immutableCredentials == null)
                return;

            if (immutableCredentials?.UseToken == true)
            {
                requestContext.Request.Headers[HeaderKeys.XAmzSecurityTokenHeader] = immutableCredentials.Token;
            }

            await requestContext.Signer
                .SignAsync(
                    requestContext.Request, 
                    requestContext.ClientConfig, 
                    immutableCredentials)
                .ConfigureAwait(false);
        }
    }
}
