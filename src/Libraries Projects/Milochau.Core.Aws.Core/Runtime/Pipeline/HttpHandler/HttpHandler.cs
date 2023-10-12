using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Util;
using System;
using System.Globalization;
using System.Net.Http;
using System.Text;

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
                IRequest wrappedRequest = executionContext.RequestContext.Request;
                httpRequest = CreateWebRequest(executionContext.RequestContext);
                httpRequest.SetRequestHeaders(wrappedRequest.Headers);
                
                // Send request body if present.
                if (wrappedRequest.HasRequestBody())
                {
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo? edi = null;
                    try
                    {
                        // In .NET Framework, there needs to be a cancellation token in this method since GetRequestStreamAsync
                        // does not accept a cancellation token. A workaround is used. This isn't necessary in .NET Standard
                        // where the stream is a property of the request.

                        var requestContent = await httpRequest.GetRequestContentAsync().ConfigureAwait(false);
                        WriteContentToRequestBody(requestContent, httpRequest, executionContext.RequestContext);
                    }
                    catch(Exception e)
                    {
                        edi = System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e);
                    }

                    if (edi != null)
                    {
                        await CompleteFailedRequest(executionContext, httpRequest).ConfigureAwait(false);

                        edi.Throw();
                    }
                }
                
                var response = await httpRequest.GetResponseAsync(executionContext.RequestContext.CancellationToken).
                    ConfigureAwait(false);
                executionContext.ResponseContext.HttpResponse = response;

                // The response is not unmarshalled yet.
                return null;
            }            
            finally
            {
                httpRequest?.Dispose();
            }
        }

        private static async System.Threading.Tasks.Task CompleteFailedRequest(IExecutionContext executionContext, IHttpRequest<HttpContent> httpRequest)
        {
            // In some cases where writing the request body fails, HttpWebRequest.Abort
            // may not dispose of the underlying Socket, so we need to retrieve and dispose
            // the web response to close the socket
            IWebResponseData iwrd = null;
            try
            {
                iwrd = await httpRequest.GetResponseAsync(executionContext.RequestContext.CancellationToken).ConfigureAwait(false);
            }
            catch { }
            finally
            {
                if (iwrd != null && iwrd.ResponseBody != null)
                    iwrd.ResponseBody.Dispose();
            }
        }

        /// <summary>
        /// Determines the content for request body and uses the HTTP request
        /// to write the content to the HTTP request body.
        /// </summary>
        /// <param name="requestContent">Content to be written.</param>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <param name="requestContext">The request context.</param>
        private static void WriteContentToRequestBody(HttpContent requestContent,
            IHttpRequest<HttpContent> httpRequest,
            IRequestContext requestContext)
        {
            IRequest wrappedRequest = requestContext.Request;

            // This code path ends up using a ByteArrayContent for System.Net.HttpClient used by .NET Core.
            // HttpClient can't seem to handle ByteArrayContent with 0 length so in that case use
            // the StreamContent code path.
            if (wrappedRequest.Content != null && wrappedRequest.Content.Length > 0)
            {
                byte[] requestData = wrappedRequest.Content;
                httpRequest.WriteToRequestBody(requestContent, requestData, requestContext.Request.Headers);
            }
            else
            {
                System.IO.Stream originalStream;
                if (wrappedRequest.ContentStream == null)
                {
                    originalStream = new System.IO.MemoryStream();
                    originalStream.Write(wrappedRequest.Content, 0, wrappedRequest.Content.Length);
                    originalStream.Position = 0;
                }
                else
                {
                    originalStream = wrappedRequest.ContentStream;
                }

                httpRequest.WriteToRequestBody(requestContent, originalStream, requestContext.Request.Headers, requestContext);
            }


            //byte[] requestData = requestContext.Request.Content;
            //httpRequest.WriteToRequestBody(requestContent, requestData, requestContext.Request.Headers);
        }

        /// <summary>
        /// Creates the HttpWebRequest and configures the end point, content, user agent and proxy settings.
        /// </summary>
        /// <param name="requestContext">The async request.</param>
        /// <returns>The web request that actually makes the call.</returns>
        protected virtual IHttpRequest<HttpContent> CreateWebRequest(IRequestContext requestContext)
        {
            IRequest request = requestContext.Request;
            Uri url = AmazonServiceClient.ComposeUrl(request);
            var httpRequest = _requestFactory.CreateHttpRequest(url);
            httpRequest.ConfigureRequest(requestContext);

            httpRequest.Method = request.HttpMethod;
            if (request.MayContainRequestBody())
            {
                var content = request.Content;
                if (request.SetContentFromParameters || (content == null && request.ContentStream == null))
                {
                    // Mapping parameters to query string or body are mutually exclusive.
                    if (!request.UseQueryString)
                    {
                        request.Content = Encoding.UTF8.GetBytes(string.Empty);
                        request.SetContentFromParameters = true;
                    }
                    else
                    {
                        request.Content = Array.Empty<byte>();
                    }
                }

                if (content != null)
                {
                    request.Headers[HeaderKeys.ContentLengthHeader] =
                        content.Length.ToString(CultureInfo.InvariantCulture);
                }
                else if (request.ContentStream != null && request.ContentStream.CanSeek && !request.Headers.ContainsKey(HeaderKeys.ContentLengthHeader))
                {
                    request.Headers[HeaderKeys.ContentLengthHeader] =
                        request.ContentStream.Length.ToString(CultureInfo.InvariantCulture);
                }
            }

            return httpRequest;
        }
    }
}
