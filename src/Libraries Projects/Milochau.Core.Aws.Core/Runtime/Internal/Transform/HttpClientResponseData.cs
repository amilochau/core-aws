using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Transform
{
    public class HttpClientResponseData : IWebResponseData
    {
        HttpResponseMessageBody _response;
        Dictionary<string, string> _headers;
        HashSet<string> _headerNamesSet;

        internal HttpClientResponseData(HttpResponseMessage response)
        {
            _response = new HttpResponseMessageBody(response);

            this.StatusCode = response.StatusCode;
            this.ContentLength = response.Content.Headers.ContentLength ?? 0;

            if (response.Content.Headers.ContentType != null)
            {
                this.ContentType = response.Content.Headers.ContentType.MediaType;
            }
            CopyHeaderValues(response);
        }

        public HttpStatusCode StatusCode { get; private set; }

        public string ContentType { get; private set; }

        public long ContentLength { get; private set; }

        public string GetHeaderValue(string headerName)
        {
            string headerValue;
            if(_headers.TryGetValue(headerName, out headerValue))
                return headerValue;

            return string.Empty;
        }

        public bool IsHeaderPresent(string headerName)
        {
            return _headerNamesSet.Contains(headerName);
        }

        private void CopyHeaderValues(HttpResponseMessage response)
        {
            List<string> headerNames = new List<string>();
            _headers = new Dictionary<string, string>(10, StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, IEnumerable<string>> kvp in response.Headers)
            {
                headerNames.Add(kvp.Key);
                var headerValue = GetFirstHeaderValue(response.Headers, kvp.Key);
                _headers.Add(kvp.Key, headerValue);
            }

            if (response.Content != null)
            {
                foreach (var kvp in response.Content.Headers)
                {
                    if (!headerNames.Contains(kvp.Key))
                    {
                        headerNames.Add(kvp.Key);
                        var headerValue = GetFirstHeaderValue(response.Content.Headers, kvp.Key);
                        _headers.Add(kvp.Key, headerValue);
                    }
                }
            }
            _headerNamesSet = new HashSet<string>(headerNames, StringComparer.OrdinalIgnoreCase);
        }

        private string GetFirstHeaderValue(HttpHeaders headers, string key)
        {
            IEnumerable<string> headerValues = null;
            if (headers.TryGetValues(key, out headerValues))
                return headerValues.FirstOrDefault();

            return string.Empty;
        }

        public IHttpResponseBody ResponseBody
        {
            get { return _response; }
        }
    }

    public class HttpResponseMessageBody : IHttpResponseBody
    {
        HttpResponseMessage _response;
        bool _disposed = false;

        public HttpResponseMessageBody(HttpResponseMessage response)
        {
            _response = response;
        }

        public Stream OpenResponse()
        {
            if (_disposed)
                throw new ObjectDisposedException("HttpWebResponseBody");

            return _response.Content.ReadAsStreamAsync().Result;
        }

        public Task<Stream> OpenResponseAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException("HttpWebResponseBody");

            return _response.Content.ReadAsStreamAsync();
        }

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
                if (_response != null)
                    _response.Dispose();

                _disposed = true;
            }
        }
    }
}
