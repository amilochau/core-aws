using Microsoft.Extensions.Http;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Util;
using Polly;
using Polly.Extensions.Http;
using System;
using System.IO;
using System.Net.Http;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.HttpHandler
{
    /// <summary>
    /// A factory which creates HTTP requests which uses System.Net.Http.HttpClient.
    /// </summary>
    public class HttpRequestMessageFactory : IHttpRequestFactory<HttpContent>
    {
        private static readonly IAsyncPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(Math.Min(Math.Pow(2, retryAttempt), 20))); // Max 20 seconds
        private static readonly SocketsHttpHandler socketHandler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15), // Recreate every 15 minutes
        };
        private static PolicyHttpMessageHandler pollyHandler = new PolicyHttpMessageHandler(retryPolicy)
        {
            InnerHandler = socketHandler,
        };
        private static readonly HttpClient httpClient = new HttpClient(pollyHandler);

        /// <summary>
        /// Creates an HTTP request for the given URI.
        /// </summary>
        /// <returns>An HTTP request.</returns>
        public IHttpRequest<HttpContent> CreateHttpRequest()
        {
            return new HttpWebRequestMessage(httpClient);
        }
    }

    /// <summary>
    /// HTTP request wrapper for System.Net.Http.HttpRequestMessage.
    /// </summary>
    public class HttpWebRequestMessage : IHttpRequest<HttpContent>
    {
        private bool _disposed;
        private readonly HttpClient httpClient;

        /// <summary>
        /// The constructor for HttpWebRequestMessage.
        /// </summary>
        /// <param name="httpClient">The HttpClient used to make the request.</param>
        public HttpWebRequestMessage(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>The HTTP request message</summary>
        public HttpRequestMessage HttpRequestMessage { get; set; }

        /// <summary>
        /// Configures a request as per the request context.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        public void ConfigureRequest(IRequestContext requestContext)
        {
            // Configure the Expect 100-continue header
            if (requestContext != null && requestContext.OriginalRequest != null)
            {
                requestContext.HttpRequestMessage.Headers.ExpectContinue = false;
            }

            if (!requestContext.HttpRequestMessage.Headers.Contains(HeaderKeys.AmzSdkInvocationId))
            {
                requestContext.HttpRequestMessage.Headers.Add(HeaderKeys.AmzSdkInvocationId, requestContext.InvocationId.ToString());
            }
        }

        /// <summary>
        /// Returns the HTTP response.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        public async System.Threading.Tasks.Task<IWebResponseData> GetResponseAsync(System.Threading.CancellationToken cancellationToken)
        {
            try
            {
                HttpResponseMessage responseMessage = await httpClient.SendAsync(HttpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    // For all responses other than HTTP 2xx, return an exception.
                    throw new HttpErrorResponseException(new HttpClientResponseData(responseMessage));
                }

                return new HttpClientResponseData(responseMessage);
            }
            catch (HttpRequestException httpException)
            {
                if (httpException.InnerException != null && httpException.InnerException is IOException)
                {
                    throw httpException.InnerException;
                }

                throw;
            }
            catch (OperationCanceledException canceledException)
            {
                if (!cancellationToken.IsCancellationRequested && canceledException.InnerException != null)
                {
                    //OperationCanceledException thrown by HttpClient not the CancellationToken supplied by the user.
                    //This exception can wrap at least IOExceptions, ObjectDisposedExceptions and should be retried.
                    //Throw the underlying exception if it exists.
                    throw canceledException.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Disposes the HttpWebRequestMessage.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                HttpRequestMessage.Dispose();

                _disposed = true;
            }
        }
    }
}
