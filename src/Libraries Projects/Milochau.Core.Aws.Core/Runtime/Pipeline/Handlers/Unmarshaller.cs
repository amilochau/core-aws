using Milochau.Core.Aws.Core.Runtime.Internal.Transform;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.Handlers
{
    /// <summary>
    /// This handler unmarshalls the HTTP response.
    /// </summary>
    public class Unmarshaller : PipelineHandler
    {
        private readonly bool _supportsResponseLogging;

        /// <summary>
        /// The constructor for Unmarshaller.
        /// </summary>
        /// <param name="supportsResponseLogging">
        /// Boolean value which indicated if the unmarshaller 
        /// handler supports response logging.
        /// </param>
        public Unmarshaller(bool supportsResponseLogging)
        {
            _supportsResponseLogging = supportsResponseLogging;
        }

        /// <summary>
        /// Unmarshalls the response returned by the HttpHandler.
        /// </summary>
        /// <typeparam name="T">The response type for the current request.</typeparam>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            await base.InvokeAsync<T>(executionContext).ConfigureAwait(false);
            // Unmarshall the response
            await UnmarshallAsync(executionContext).ConfigureAwait(false);
            return (T)executionContext.ResponseContext.Response;
        }

        /// <summary>
        /// Unmarshalls the HTTP response.
        /// </summary>
        /// <param name="executionContext">
        /// The execution context, it contains the request and response context.
        /// </param>
        private async System.Threading.Tasks.Task UnmarshallAsync(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;
            var responseContext = executionContext.ResponseContext;
            var unmarshaller = requestContext.Unmarshaller;
            try
            {
                var responseStream = await responseContext.HttpResponse.ResponseBody.OpenResponseAsync().ConfigureAwait(false);
                var context = unmarshaller.CreateContext(responseContext.HttpResponse,
                    false,
                    responseStream,
                    false,
                    requestContext);

                var response = UnmarshallResponse(context, requestContext);
                responseContext.Response = response;
            }
            finally
            {
                responseContext.HttpResponse.ResponseBody.Dispose();
            }
        }

        private AmazonWebServiceResponse UnmarshallResponse(UnmarshallerContext context,
            IRequestContext requestContext)
        {
            var unmarshaller = requestContext.Unmarshaller;
            AmazonWebServiceResponse response = unmarshaller.UnmarshallResponse(context);

            context.ValidateCRC32IfAvailable();
            return response;
        }
    }
}
