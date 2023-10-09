using Milochau.Core.Aws.Core.Runtime.Credentials;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.Handlers
{
    /// <summary>
    /// This handler retrieved the AWS credentials to be used for the current call.
    /// </summary>
    public class CredentialsRetriever : PipelineHandler
    {
        /// <summary>
        /// The constructor for CredentialsRetriever.
        /// </summary>
        /// <param name="credentials">An AWSCredentials instance.</param>
        public CredentialsRetriever(AWSCredentials credentials)
        {
            this.Credentials = credentials;
        }

        protected AWSCredentials Credentials
        {
            get;
            private set;
        }

        /// <summary>
        /// Calls pre invoke logic before calling the next handler 
        /// in the pipeline.
        /// </summary>
        /// <typeparam name="T">The response type for the current request.</typeparam>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            executionContext.RequestContext.ImmutableCredentials = await Credentials.GetCredentialsAsync().ConfigureAwait(false);

            return await base.InvokeAsync<T>(executionContext).ConfigureAwait(false);
        }
    }
}
