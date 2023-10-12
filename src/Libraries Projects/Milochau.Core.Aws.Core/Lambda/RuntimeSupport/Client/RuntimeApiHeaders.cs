using System.Collections.Generic;
using System.Linq;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client
{
    internal class RuntimeApiHeaders
    {
        internal const string HeaderAwsRequestId = "Lambda-Runtime-Aws-Request-Id";
        internal const string HeaderTraceId = "Lambda-Runtime-Trace-Id";

        public RuntimeApiHeaders(Dictionary<string, IEnumerable<string>> headers)
        {
            AwsRequestId = GetHeaderValueRequired(headers, HeaderAwsRequestId);
            TraceId = GetHeaderValueOrNull(headers, HeaderTraceId);
        }

        public string AwsRequestId { get; private set; }
        public string TraceId { get; private set; }

        private static string GetHeaderValueRequired(Dictionary<string, IEnumerable<string>> headers, string header)
        {
            return headers[header].FirstOrDefault();
        }

        private static string GetHeaderValueOrNull(Dictionary<string, IEnumerable<string>> headers, string header)
        {
            if (headers.TryGetValue(header, out var values))
            {
                return values.FirstOrDefault();
            }

            return null;
        }
    }
}
