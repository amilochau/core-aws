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

using Amazon.Runtime.Internal.Transform;
using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Amazon.Runtime
{
    /// <summary>
    /// A factory which creates HTTP requests which uses System.Net.Http.HttpClient.
    /// </summary>
    public class HttpRequestMessageFactory : IHttpRequestFactory<HttpContent>
    {
        static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Creates an HTTP request for the given URI.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <returns>An HTTP request.</returns>
        public IHttpRequest<HttpContent> CreateHttpRequest(Uri requestUri)
        {
            return new HttpWebRequestMessage(_httpClient, requestUri);
        }
    }

    /// <summary>
    /// HTTP request wrapper for System.Net.Http.HttpRequestMessage.
    /// </summary>
    public class HttpWebRequestMessage : IHttpRequest<HttpContent>
    {
        /// <summary>
        /// Set of content header names.
        /// </summary>
        private static HashSet<string> ContentHeaderNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            HeaderKeys.ContentLengthHeader,
            HeaderKeys.ContentTypeHeader,
            HeaderKeys.ContentRangeHeader,
            HeaderKeys.ContentMD5Header,
            HeaderKeys.ContentEncodingHeader,
            HeaderKeys.ContentDispositionHeader,
            HeaderKeys.Expires
        };

        private bool _disposed;
        private HttpRequestMessage _request;
        private HttpClient _httpClient;

        /// <summary>
        /// The constructor for HttpWebRequestMessage.
        /// </summary>
        /// <param name="httpClient">The HttpClient used to make the request.</param>
        /// <param name="requestUri">The request URI.</param>
        public HttpWebRequestMessage(HttpClient httpClient, Uri requestUri)
        {
            _httpClient = httpClient;

            _request = new HttpRequestMessage();
            _request.RequestUri = requestUri;
        }

        /// <summary>
        /// The HTTP method or verb.
        /// </summary>
        public string Method
        {
            get { return _request.Method.Method; }
            set { _request.Method = new HttpMethod(value); }
        }

        /// <summary>
        /// Configures a request as per the request context.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        public void ConfigureRequest(IRequestContext requestContext)
        {
            // Configure the Expect 100-continue header
            if (requestContext != null && requestContext.OriginalRequest != null)
            {
                _request.Headers.ExpectContinue = false;
            }
        }

        /// <summary>
        /// Sets the headers on the request.
        /// </summary>
        /// <param name="headers">A dictionary of header names and values.</param>
        public void SetRequestHeaders(IDictionary<string, string> headers)
        {
            foreach (var kvp in headers)
            {
                if (ContentHeaderNames.Contains(kvp.Key))
                    continue;

                _request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Gets a handle to the request content.
        /// </summary>
        /// <returns>The request content.</returns>
#pragma warning disable CA1024
        public HttpContent GetRequestContent()
        {
            return _request.Content;
        }
#pragma warning restore CA1024

        /// <summary>
        /// Returns the HTTP response.
        /// </summary>
        /// <returns>The HTTP response.</returns>
        public IWebResponseData GetResponse()
        {
            try
            {
                return this.GetResponseAsync(System.Threading.CancellationToken.None).Result;
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }

        /// <summary>
        /// Returns the HTTP response.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IWebResponseData> GetResponseAsync(System.Threading.CancellationToken cancellationToken)
        {
            try
            {
                var responseMessage = await _httpClient.SendAsync(_request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    // For all responses other than HTTP 2xx, return an exception.
                    throw new Amazon.Runtime.Internal.HttpErrorResponseException(
                        new HttpClientResponseData(responseMessage));
                }

                return new HttpClientResponseData(responseMessage);
            }
            catch (HttpRequestException httpException)
            {
                if (httpException.InnerException != null)
                {
                    if (httpException.InnerException is IOException)
                    {
                        throw httpException.InnerException;
                    }
                }

                throw;
            }
            catch (OperationCanceledException canceledException)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    //OperationCanceledException thrown by HttpClient not the CancellationToken supplied by the user.
                    //This exception can wrap at least IOExceptions, ObjectDisposedExceptions and should be retried.
                    //Throw the underlying exception if it exists.
                    if(canceledException.InnerException != null)
                    {
                        throw canceledException.InnerException;
                    }
                }

                throw;
            }
        }

        /// <summary>
        /// Writes a byte array to the request body.
        /// </summary>
        /// <param name="requestContent">The destination where the content stream is written.</param>
        /// <param name="content">The content stream to be written.</param>
        /// <param name="contentHeaders">HTTP content headers.</param>
        public void WriteToRequestBody(HttpContent requestContent,
            byte[] content, IDictionary<string, string> contentHeaders)
        {
            _request.Content = new ByteArrayContent(content);
            _request.Content.Headers.ContentLength = content.Length;
            WriteContentHeaders(contentHeaders);
        }

        /// <summary>
        /// Gets a handle to the request content.
        /// </summary>
        /// <returns></returns>
        public System.Threading.Tasks.Task<HttpContent> GetRequestContentAsync()
        {
            return System.Threading.Tasks.Task.FromResult(_request.Content);
        }


        private void WriteContentHeaders(IDictionary<string, string> contentHeaders)
        {
            _request.Content.Headers.ContentType =
                MediaTypeHeaderValue.Parse(contentHeaders[HeaderKeys.ContentTypeHeader]);

            if (contentHeaders.TryGetValue(HeaderKeys.ContentRangeHeader, out var contentRangeHeader))
                _request.Content.Headers.TryAddWithoutValidation(HeaderKeys.ContentRangeHeader,
                    contentRangeHeader);

            if (contentHeaders.TryGetValue(HeaderKeys.ContentMD5Header, out var contentMd5Header))
                _request.Content.Headers.TryAddWithoutValidation(HeaderKeys.ContentMD5Header,
                    contentMd5Header);

            if (contentHeaders.TryGetValue(HeaderKeys.ContentEncodingHeader, out var contentEncodingHeader))
                _request.Content.Headers.TryAddWithoutValidation(HeaderKeys.ContentEncodingHeader,
                    contentEncodingHeader);

            if (contentHeaders.TryGetValue(HeaderKeys.ContentDispositionHeader, out var contentDispositionHeader))
                _request.Content.Headers.TryAddWithoutValidation(HeaderKeys.ContentDispositionHeader,
                    contentDispositionHeader);

            if (contentHeaders.TryGetValue(HeaderKeys.Expires, out var expiresHeaderValue) &&
                DateTime.TryParse(expiresHeaderValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var expires))
                _request.Content.Headers.Expires = expires;
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
                if (_request != null)
                    _request.Dispose();

                _disposed = true;
            }
        }
    }
}
