using System.Net.Http;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.HttpHandler
{
    /// <summary>
    /// The HTTP handler contains common logic for issuing an HTTP request that is 
    /// independent of the underlying HTTP infrastructure.
    /// </summary>
    public class HttpHandler : PipelineHandler
    {
        private readonly IHttpRequestFactory<HttpContent> _requestFactory;

        /// <summary>
        /// The constructor for HttpHandler.
        /// </summary>
        /// <param name="requestFactory">The request factory used to create HTTP Requests.</param>
        public HttpHandler()
        {
            _requestFactory = new HttpRequestMessageFactory();
        }

        /// <summary>
        /// Issues an HTTP request for the current request context.
        /// </summary>
        /// <typeparam name="T">The response type for the current request.</typeparam>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            IHttpRequest<HttpContent>? httpRequest = null;
            try
            {
                httpRequest = _requestFactory.CreateHttpRequest();
                httpRequest.HttpRequestMessage = executionContext.RequestContext.HttpRequestMessage;
                httpRequest.ConfigureRequest(executionContext.RequestContext);

                var response = await httpRequest.GetResponseAsync(executionContext.RequestContext.CancellationToken).ConfigureAwait(false);
                executionContext.ResponseContext.HttpResponse = response;

                // The response is not unmarshalled yet.
                return null;
            }            
            finally
            {
                httpRequest?.Dispose();
            }
        }
    }
}
