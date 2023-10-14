using Milochau.Core.Aws.Core.Util;
using System.IO;
using System;
using System.Net.Http;
using Microsoft.Extensions.Http;
using Polly.Extensions.Http;
using Polly;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.HttpHandler
{
    /// <summary>
    /// The HTTP handler contains common logic for issuing an HTTP request that is 
    /// independent of the underlying HTTP infrastructure.
    /// </summary>
    public class HttpHandler : PipelineHandler
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
        /// Issues an HTTP request for the current request context.
        /// </summary>
        /// <typeparam name="T">The response type for the current request.</typeparam>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            try
            {
                ConfigureRequest(executionContext.RequestContext);

                try
                {
                    HttpResponseMessage responseMessage = await httpClient.SendAsync(executionContext.RequestContext.HttpRequestMessage, HttpCompletionOption.ResponseHeadersRead, executionContext.RequestContext.CancellationToken).ConfigureAwait(continueOnCapturedContext: false);

                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        // For all responses other than HTTP 2xx, return an exception.
                        throw new HttpErrorResponseException(responseMessage);
                    }

                    executionContext.ResponseContext.HttpResponse = responseMessage;
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
                    if (!executionContext.RequestContext.CancellationToken.IsCancellationRequested && canceledException.InnerException != null)
                    {
                        //OperationCanceledException thrown by HttpClient not the CancellationToken supplied by the user.
                        //This exception can wrap at least IOExceptions, ObjectDisposedExceptions and should be retried.
                        //Throw the underlying exception if it exists.
                        throw canceledException.InnerException;
                    }

                    throw;
                }

                // The response is not unmarshalled yet.
                return null;
            }            
            finally
            {
                executionContext.RequestContext.HttpRequestMessage?.Dispose();
            }
        }

        /// <summary>
        /// Configures a request as per the request context.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        private static void ConfigureRequest(IRequestContext requestContext)
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
    }
}
