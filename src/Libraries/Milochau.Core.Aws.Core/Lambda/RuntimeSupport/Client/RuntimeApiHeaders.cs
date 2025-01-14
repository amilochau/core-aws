using System.Linq;
using System.Net.Http.Headers;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client
{
    internal class RuntimeApiHeaders
    {
        internal const string HeaderAwsRequestId = "Lambda-Runtime-Aws-Request-Id";
        internal const string HeaderTraceId = "Lambda-Runtime-Trace-Id";

        public RuntimeApiHeaders(HttpHeaders headers)
        {
            AwsRequestId = GetHeaderValueRequired(headers, HeaderAwsRequestId);
            TraceId = GetHeaderValueOrNull(headers, HeaderTraceId);
        }

        public string AwsRequestId { get; }
        public string? TraceId { get; }

        private static string GetHeaderValueRequired(HttpHeaders headers, string header)
        {
            return headers.GetValues(header).First();
        }

        private static string? GetHeaderValueOrNull(HttpHeaders headers, string header)
        {
            return headers.TryGetValues(header, out var values) ? values.FirstOrDefault() : null;
        }
    }
}
