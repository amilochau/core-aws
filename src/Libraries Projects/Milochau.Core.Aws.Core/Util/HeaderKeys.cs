namespace Milochau.Core.Aws.Core.Util
{
    public abstract class HeaderKeys
    {
        public const string ContentTypeHeader = "Content-Type";
        public const string ContentLengthHeader = "Content-Length";
        public const string AuthorizationHeader = "Authorization";
        public const string HostHeader = "host";
        public const string UserAgentHeader = "User-Agent";
        public const string DateHeader = "Date";
        public const string RangeHeader = "Range";
        public const string TransferEncodingHeader = "transfer-encoding";

        public const string RequestIdHeader = "x-amzn-RequestId";
        public const string XAmzId2Header = "x-amz-id-2";
        public const string XAmzRequestIdHeader = "x-amz-request-id";
        public const string XAmzDateHeader = "X-Amz-Date";
        public const string XAmzErrorType = "x-amzn-ErrorType";
        public const string XAmznErrorMessage = "x-amzn-error-message";
        public const string XAmzContentSha256Header = "X-Amz-Content-SHA256";
        public const string XAmzDecodedContentLengthHeader = "X-Amz-Decoded-Content-Length";
        public const string XAmzSecurityTokenHeader = "x-amz-security-token";
        public const string XAmzApiVersion = "x-amz-api-version";
        public const string XAmzTrailerHeader = "X-Amz-Trailer";

        public const string XAmzUserAgentHeader = "x-amz-user-agent";
        public const string XAmznTraceIdHeader = "x-amzn-trace-id";
        public const string AmzSdkInvocationId = "amz-sdk-invocation-id";
        public const string AmzSdkRequest = "amz-sdk-request";
    }
}
