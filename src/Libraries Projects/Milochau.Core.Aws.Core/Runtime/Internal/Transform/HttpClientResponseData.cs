using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Transform
{
    public class HttpClientResponseData : IWebResponseData
    {
        Dictionary<string, string> _headers;
        HashSet<string> _headerNamesSet;

        internal HttpClientResponseData(HttpResponseMessage response)
        {
            HttpResponseMessage = response;

            CopyHeaderValues(response);
        }

        public HttpResponseMessage HttpResponseMessage { get; }

        public string GetHeaderValue(string headerName)
        {
            if (_headers.TryGetValue(headerName, out string headerValue))
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

        private static string GetFirstHeaderValue(HttpHeaders headers, string key)
        {
            if (headers.TryGetValues(key, out IEnumerable<string> headerValues))
                return headerValues.FirstOrDefault();

            return string.Empty;
        }
    }
}
