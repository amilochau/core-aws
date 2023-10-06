/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 * 
 *  http://aws.amazon.com/apache2.0
 * 
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using Amazon.Util.Internal;
using System;
using System.Globalization;
using System.Net;
using System.Text;

namespace Amazon.Runtime.Internal
{
    /// <summary>
    /// The HTTP handler contains common logic for issuing an HTTP request that is 
    /// independent of the underlying HTTP infrastructure.
    /// </summary>
    /// <typeparam name="TRequestContent"></typeparam>
    public class HttpHandler<TRequestContent> : PipelineHandler, IDisposable
    {
        private bool _disposed;
        private IHttpRequestFactory<TRequestContent> _requestFactory;

        /// <summary>
        /// The sender parameter used in any events raised by this handler.
        /// </summary>
        public object CallbackSender { get; private set; }

        /// <summary>
        /// The constructor for HttpHandler.
        /// </summary>
        /// <param name="requestFactory">The request factory used to create HTTP Requests.</param>
        /// <param name="callbackSender">The sender parameter used in any events raised by this handler.</param>
        public HttpHandler(IHttpRequestFactory<TRequestContent> requestFactory, object callbackSender)
        {
            _requestFactory = requestFactory;
            this.CallbackSender = callbackSender;
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
            IHttpRequest<TRequestContent> httpRequest = null;
            try
            {
                SetMetrics(executionContext.RequestContext);
                IRequest wrappedRequest = executionContext.RequestContext.Request;
                httpRequest = CreateWebRequest(executionContext.RequestContext);
                httpRequest.SetRequestHeaders(wrappedRequest.Headers);
                
                using(executionContext.RequestContext.Metrics.StartEvent(Metric.HttpRequestTime))
                {
                    // Send request body if present.
                    if (wrappedRequest.HasRequestBody())
                    {
                        System.Runtime.ExceptionServices.ExceptionDispatchInfo edi = null;
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
                }
                // The response is not unmarshalled yet.
                return null;
            }            
            finally
            {
                if (httpRequest != null)
                    httpRequest.Dispose();
            }
        }

        private static async System.Threading.Tasks.Task CompleteFailedRequest(IExecutionContext executionContext, IHttpRequest<TRequestContent> httpRequest)
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

        private static void SetMetrics(IRequestContext requestContext)
        {
            requestContext.Metrics.AddProperty(Metric.ServiceName, requestContext.Request.ServiceName);
            requestContext.Metrics.AddProperty(Metric.ServiceEndpoint, requestContext.Request.Endpoint);
            requestContext.Metrics.AddProperty(Metric.MethodName, requestContext.Request.RequestName);
        }

        /// <summary>
        /// Determines the content for request body and uses the HTTP request
        /// to write the content to the HTTP request body.
        /// </summary>
        /// <param name="requestContent">Content to be written.</param>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <param name="requestContext">The request context.</param>
        private void WriteContentToRequestBody(TRequestContent requestContent,
            IHttpRequest<TRequestContent> httpRequest,
            IRequestContext requestContext)
        {
            byte[] requestData = requestContext.Request.Content;
            requestContext.Metrics.AddProperty(Metric.RequestSize, requestData.Length);
            httpRequest.WriteToRequestBody(requestContent, requestData, requestContext.Request.Headers);
        }

        /// <summary>
        /// Creates the HttpWebRequest and configures the end point, content, user agent and proxy settings.
        /// </summary>
        /// <param name="requestContext">The async request.</param>
        /// <returns>The web request that actually makes the call.</returns>
        protected virtual IHttpRequest<TRequestContent> CreateWebRequest(IRequestContext requestContext)
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
                        string queryString = AWSSDKUtils.GetParametersAsString(request);
                        content = Encoding.UTF8.GetBytes(queryString);
                        request.Content = content;
                        request.SetContentFromParameters = true;
                    }
                    else
                    {
                        request.Content = ArrayEx.Empty<byte>();
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

        /// <summary>
        /// Disposes the HTTP handler.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_requestFactory != null)
                    _requestFactory.Dispose();

                _disposed = true;
            }
        }
    }
}
