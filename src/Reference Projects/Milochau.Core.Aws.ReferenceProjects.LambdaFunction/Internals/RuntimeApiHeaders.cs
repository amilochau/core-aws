using System.Collections.Generic;
using System.Linq;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals
{
    public class RuntimeApiHeaders
    {
        internal const string HeaderAwsRequestId = "Lambda-Runtime-Aws-Request-Id";
        internal const string HeaderTraceId = "Lambda-Runtime-Trace-Id";
        internal const string HeaderClientContext = "Lambda-Runtime-Client-Context";
        internal const string HeaderCognitoIdentity = "Lambda-Runtime-Cognito-Identity";
        internal const string HeaderDeadlineMs = "Lambda-Runtime-Deadline-Ms";
        internal const string HeaderInvokedFunctionArn = "Lambda-Runtime-Invoked-Function-Arn";

        public RuntimeApiHeaders(Dictionary<string, IEnumerable<string>> headers)
        {
            DeadlineMs = GetHeaderValueOrNull(headers, HeaderDeadlineMs);
            AwsRequestId = GetHeaderValueRequired(headers, HeaderAwsRequestId);
            ClientContextJson = GetHeaderValueOrNull(headers, HeaderClientContext);
            CognitoIdentityJson = GetHeaderValueOrNull(headers, HeaderCognitoIdentity);
            InvokedFunctionArn = GetHeaderValueOrNull(headers, HeaderInvokedFunctionArn);
            TraceId = GetHeaderValueOrNull(headers, HeaderTraceId);
        }

        public string AwsRequestId { get; private set; }
        public string? InvokedFunctionArn { get; private set; }
        public string? TraceId { get; private set; }
        public string? ClientContextJson { get; private set; }
        public string? CognitoIdentityJson { get; private set; }
        public string? DeadlineMs { get; private set; }

        private string GetHeaderValueRequired(Dictionary<string, IEnumerable<string>> headers, string header)
        {
            return headers[header].FirstOrDefault() ?? string.Empty;
        }

        private string? GetHeaderValueOrNull(Dictionary<string, IEnumerable<string>> headers, string header)
        {
            if (headers.TryGetValue(header, out var values))
            {
                return values.FirstOrDefault();
            }

            return null;
        }
    }
}
