using System.Linq;
using System.Net.Http.Headers;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client
{
    internal class RuntimeApiHeaders(HttpHeaders headers)
    {
        internal const string HeaderAwsRequestId = "Lambda-Runtime-Aws-Request-Id";
        internal const string HeaderTraceId = "Lambda-Runtime-Trace-Id";

        public string AwsRequestId { get; } = GetHeaderValueRequired(headers, HeaderAwsRequestId);
        public string? TraceId { get; } = GetHeaderValueOrNull(headers, HeaderTraceId);

        private static string GetHeaderValueRequired(HttpHeaders headers, string header)
        {
            return headers.GetValues(header).First();
        }

        private static string? GetHeaderValueOrNull(HttpHeaders headers, string header)
        {
            if (headers.TryGetValues(header, out var values))
            {
                return values.FirstOrDefault();
            }

            return null;
        }
    }
}
