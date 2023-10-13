using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Milochau.Core.Aws.Core.Util;
using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Runtime.Pipeline;
using System.Net.Http;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Auth
{
    /// <summary>
    /// AWS4 protocol signer for service calls that transmit authorization in the header field "Authorization".
    /// </summary>
    public class AWSSigner
    {
        public const int V4_SIGNATURE_LENGTH = 64;

        public const string Scheme = "AWS4";
        public const string Algorithm = "HMAC-SHA256";

        public const string AWS4AlgorithmTag = Scheme + "-" + Algorithm;

        public const string Terminator = "aws4_request";
        public static readonly byte[] TerminatorBytes = Encoding.UTF8.GetBytes(Terminator);

        public const string Credential = "Credential";
        public const string SignedHeaders = "SignedHeaders";
        public const string Signature = "Signature";

        public const string UnsignedPayload = "UNSIGNED-PAYLOAD";
        public const string UnsignedPayloadWithTrailer = "STREAMING-UNSIGNED-PAYLOAD-TRAILER";

        private static readonly IEnumerable<string> _headersToIgnoreWhenSigning = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            HeaderKeys.XAmznTraceIdHeader,
            HeaderKeys.TransferEncodingHeader,
            HeaderKeys.AmzSdkInvocationId,
            HeaderKeys.AmzSdkRequest
        };

        /// <summary>
        /// Calculates and signs the specified request using the AWS4 signing protocol by using the
        /// AWS account credentials given in the method parameters. The resulting signature is added
        /// to the request headers as 'Authorization'. Parameters supplied in the request, either in
        /// the resource path as a query string or in the Parameters collection must not have been
        /// uri encoded. If they have, use the SignRequest method to obtain a signature.
        /// </summary>
        /// <param name="request">
        /// The request to compute the signature for. Additional headers mandated by the AWS4 protocol 
        /// ('host' and 'x-amz-date') will be added to the request before signing.
        /// </param>
        /// <param name="clientConfig">
        /// Client configuration data encompassing the service call (notably authentication
        /// region, endpoint and service name).
        /// </param>
        public void Sign(IRequestContext requestContext)
        {
            var signingResult = SignRequest(requestContext);
            requestContext.Request.Headers[HeaderKeys.AuthorizationHeader] = signingResult.ForAuthorizationHeader;
            requestContext.HttpRequestMessage.Headers.TryAddWithoutValidation(HeaderKeys.AuthorizationHeader, signingResult.ForAuthorizationHeader);
        }

        /// <summary>
        /// Calculates and signs the specified request using the AWS4 signing protocol by using the
        /// AWS account credentials given in the method parameters.
        /// </summary>
        /// <param name="request">
        /// The request to compute the signature for. Additional headers mandated by the AWS4 protocol 
        /// ('host' and 'x-amz-date') will be added to the request before signing.
        /// </param>
        /// <param name="clientConfig">
        /// Client configuration data encompassing the service call (notably authentication
        /// region, endpoint and service name).
        /// <remarks>
        /// Parameters passed as part of the resource path should be uri-encoded prior to
        /// entry to the signer. Parameters passed in the request.Parameters collection should
        /// be not be encoded; encoding will be done for these parameters as part of the 
        /// construction of the canonical request.
        /// </remarks>
        public static AWS4SigningResult SignRequest(IRequestContext requestContext)
        {
            var signedAt = InitializeHeaders(requestContext.Request, requestContext.Request.Endpoint);
            InitializeHeaders(requestContext.HttpRequestMessage, requestContext.Request.Endpoint);

            var bodyHash = SetRequestBodyHash(requestContext);
            var sortedHeaders = SortAndPruneHeaders(requestContext.Request.Headers);

            var canonicalRequest = CanonicalizeRequest(requestContext.Request.Endpoint,
                                                       requestContext.Request.ResourcePath,
                                                       requestContext.Request.HttpMethod,
                                                       sortedHeaders,
                                                       bodyHash,
                                                       requestContext.Request.PathResources);

            return ComputeSignature(signedAt,
                                    requestContext.ClientConfig.AuthenticationServiceName,
                                    CanonicalizeHeaderNames(sortedHeaders),
                                    canonicalRequest);
        }

        #region Public Signing Helpers

        /// <summary>
        /// Sets the AWS4 mandated 'host' and 'x-amz-date' headers, returning the date/time that will
        /// be used throughout the signing process in various elements and formats.
        /// </summary>
        /// <returns>Date and time used for x-amz-date, in UTC</returns>
        public static DateTime InitializeHeaders(IRequest request, Uri requestEndpoint)
        {
            var requestDateTime = CorrectClockSkew.GetCorrectedUtcNowForEndpoint(requestEndpoint.ToString());

            // clean up any prior signature in the headers if resigning
            request.Headers.Remove(HeaderKeys.AuthorizationHeader);
            request.Headers.Remove(HeaderKeys.XAmzContentSha256Header);

            if (!request.Headers.ContainsKey(HeaderKeys.HostHeader))
            {
                var hostHeader = requestEndpoint.Host;
                if (!requestEndpoint.IsDefaultPort)
                    hostHeader += ":" + requestEndpoint.Port;
                request.Headers.Add(HeaderKeys.HostHeader, hostHeader);
            }

            request.Headers[HeaderKeys.XAmzDateHeader] = requestDateTime.ToUniversalTime().ToString(AWSSDKUtils.ISO8601BasicDateTimeFormat, CultureInfo.InvariantCulture);

            return requestDateTime;
        }

        /// <summary>
        /// Sets the AWS4 mandated 'host' and 'x-amz-date' headers, returning the date/time that will
        /// be used throughout the signing process in various elements and formats.
        /// </summary>
        /// <returns>Date and time used for x-amz-date, in UTC</returns>
        public static DateTime InitializeHeaders(HttpRequestMessage request, Uri requestEndpoint)
        {
            var requestDateTime = CorrectClockSkew.GetCorrectedUtcNowForEndpoint(requestEndpoint.ToString());

            // clean up any prior signature in the headers if resigning
            request.Headers.Remove(HeaderKeys.AuthorizationHeader);
            request.Headers.Remove(HeaderKeys.XAmzContentSha256Header);

            if (!request.Headers.Contains(HeaderKeys.HostHeader))
            {
                var hostHeader = requestEndpoint.Host;
                if (!requestEndpoint.IsDefaultPort)
                    hostHeader += ":" + requestEndpoint.Port;
                request.Headers.Add(HeaderKeys.HostHeader, hostHeader);
            }

            request.Headers.Add(HeaderKeys.XAmzDateHeader, requestDateTime.ToUniversalTime().ToString(AWSSDKUtils.ISO8601BasicDateTimeFormat, CultureInfo.InvariantCulture));

            return requestDateTime;
        }

        /// <summary>
        /// Computes and returns an AWS4 signature for the specified canonicalized request
        /// </summary>
        public static AWS4SigningResult ComputeSignature(DateTime signedAt,
                                                         string service,
                                                         string signedHeaders,
                                                         string canonicalRequest)
        {
            string region = EnvironmentVariables.RegionName;

            var dateStamp = FormatDateTime(signedAt, AWSSDKUtils.ISO8601BasicDateFormat);
            var scope = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}", dateStamp, region, service, Terminator);

            var stringToSignBuilder = new StringBuilder();
            stringToSignBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}-{1}\n{2}\n{3}\n",
                                      Scheme,
                                      Algorithm,
                                      FormatDateTime(signedAt, AWSSDKUtils.ISO8601BasicDateTimeFormat),
                                      scope);

            var canonicalRequestHashBytes = ComputeHash(canonicalRequest);
            stringToSignBuilder.Append(AWSSDKUtils.ToHex(canonicalRequestHashBytes, true));

            var key = ComposeSigningKey(EnvironmentVariables.SecretKey,
                                        region,
                                        dateStamp,
                                        service);

            var stringToSign = stringToSignBuilder.ToString();
            var signature = ComputeKeyedHash(key, stringToSign);
            return new AWS4SigningResult(signedHeaders, scope, signature);
        }

        /// <summary>
        /// Formats the supplied date and time for use in AWS4 signing, where various formats are used.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="formatString">The required format</param>
        /// <returns>The UTC date/time in the requested format</returns>
        public static string FormatDateTime(DateTime dt, string formatString)
        {
            return dt.ToUniversalTime().ToString(formatString, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Compute and return the multi-stage signing key for the request.
        /// </summary>
        /// <param name="awsSecretAccessKey">The clear-text AWS secret key, if not held in secureKey</param>
        /// <param name="region">The region in which the service request will be processed</param>
        /// <param name="date">Date of the request, in yyyyMMdd format</param>
        /// <param name="service">The name of the service being called by the request</param>
        /// <returns>Computed signing key</returns>
        public static byte[] ComposeSigningKey(string awsSecretAccessKey, string region, string date, string service)
        {
            char[]? ksecret = null;

            try
            {
                ksecret = (Scheme + awsSecretAccessKey).ToCharArray();

                var hashDate = ComputeKeyedHash(Encoding.UTF8.GetBytes(ksecret), Encoding.UTF8.GetBytes(date));
                var hashRegion = ComputeKeyedHash(hashDate, Encoding.UTF8.GetBytes(region));
                var hashService = ComputeKeyedHash(hashRegion, Encoding.UTF8.GetBytes(service));
                return ComputeKeyedHash(hashService, TerminatorBytes);
            }
            finally
            {
                // clean up all secrets, regardless of how initially seeded (for simplicity)
                if (ksecret != null)
                    Array.Clear(ksecret, 0, ksecret.Length);
            }
        }

        /// <summary>
        /// If signPayload is false set the x-amz-content-sha256 header to
        /// the UNSIGNED-PAYLOAD magic string and return it.
        /// Otherwise, if the caller has already set the x-amz-content-sha256 header with a pre-computed
        /// content hash, or it is present as ContentStreamHash on the request instance, return
        /// the value to be used in request canonicalization.
        /// If not set as a header or in the request, attempt to compute a hash based on
        /// inspection of the style of the request content.
        /// </summary>
        /// <param name="request">Request to sign</param>
        /// <param name="signPayload">Whether to sign the payload</param>
        /// <param name="chunkedBodyHash">The fixed value to set for the x-amz-content-sha256 header for chunked requests</param>
        /// <param name="signatureLength">Length of the signature for each chunk in a chuncked request, in bytes</param>
        /// <returns>
        /// The computed hash, whether already set in headers or computed here. Null
        /// if we were not able to compute a hash.
        /// </returns>
        public static string? SetRequestBodyHash(IRequestContext requestContext)
        {
            string? computedContentHash;

            // Calculate the hash and set it in the headers before returning
            if (requestContext.Request.ContentStream != null)
                computedContentHash = requestContext.Request.ComputeContentStreamHash();
            else
            {
                byte[] payloadBytes = AWSSDKUtils.GetRequestPayloadBytes(requestContext.Request);
                byte[] payloadHashBytes = CryptoUtilFactory.CryptoInstance.ComputeSHA256Hash(payloadBytes);
                computedContentHash = AWSSDKUtils.ToHex(payloadHashBytes, true);
            }

            // set the header if needed and return it
            var payloadHash = computedContentHash ?? UnsignedPayload;
            requestContext.Request.Headers.Add(HeaderKeys.XAmzContentSha256Header, payloadHash);
            requestContext.HttpRequestMessage.Headers.Add(HeaderKeys.XAmzContentSha256Header, payloadHash);
            return payloadHash;
        }

        /// <summary>
        /// Compute and return the hash of a data blob using the specified key
        /// </summary>
        /// <param name="algorithm">Algorithm to use for hashing</param>
        /// <param name="key">Hash key</param>
        /// <param name="data">Data blob</param>
        /// <returns>Hash of the data</returns>
        public static byte[] ComputeKeyedHash(byte[] key, string data)
        {
            return ComputeKeyedHash(key, Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Compute and return the hash of a data blob using the specified key
        /// </summary>
        /// <param name="algorithm">Algorithm to use for hashing</param>
        /// <param name="key">Hash key</param>
        /// <param name="data">Data blob</param>
        /// <returns>Hash of the data</returns>
        public static byte[] ComputeKeyedHash(byte[] key, byte[] data)
        {
            return CryptoUtilFactory.CryptoInstance.HMACSignBinary(data, key);
        }

        /// <summary>
        /// Computes the non-keyed hash of the supplied data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeHash(string data)
        {
            return ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Computes the non-keyed hash of the supplied data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeHash(byte[] data)
        {
            return CryptoUtilFactory.CryptoInstance.ComputeSHA256Hash(data);
        }

        #endregion

        #region Private Signing Helpers

        /// <summary>
        /// Computes and returns the canonical request
        /// </summary>
        /// <param name="endpoint">The endpoint URL</param>
        /// <param name="resourcePath">the path of the resource being operated on</param>
        /// <param name="httpMethod">The http method used for the request</param>
        /// <param name="sortedHeaders">The full request headers, sorted into canonical order</param>
        /// <param name="precomputedBodyHash">
        /// <param name="pathResources">The path resource values lookup to use to replace the keys within resourcePath</param>
        /// The hash of the binary request body if present. If not supplied, the routine
        /// will look for the hash as a header on the request.
        /// </param>
        /// <returns>Canonicalised request as a string</returns>
        protected static string CanonicalizeRequest(Uri endpoint,
                                                    string resourcePath,
                                                    string httpMethod,
                                                    IDictionary<string, string> sortedHeaders,
                                                    string? precomputedBodyHash,
                                                    IDictionary<string, string> pathResources)
        {
            return CanonicalizeRequestHelper(endpoint,
                resourcePath,
                httpMethod,
                sortedHeaders,
                precomputedBodyHash,
                pathResources);
        }

        private static string CanonicalizeRequestHelper(Uri endpoint,
                                                    string resourcePath,
                                                    string httpMethod,
                                                    IDictionary<string, string> sortedHeaders,
                                                    string? precomputedBodyHash,
                                                    IDictionary<string, string> pathResources)
        {
            var canonicalRequest = new StringBuilder();
            canonicalRequest.AppendFormat("{0}\n", httpMethod);
            canonicalRequest.AppendFormat("{0}\n", AWSSDKUtils.CanonicalizeResourcePathV2(endpoint, resourcePath, pathResources));
            canonicalRequest.AppendFormat("{0}\n", "");

            canonicalRequest.AppendFormat("{0}\n", CanonicalizeHeaders(sortedHeaders));
            canonicalRequest.AppendFormat("{0}\n", CanonicalizeHeaderNames(sortedHeaders));

            if (precomputedBodyHash != null)
            {
                canonicalRequest.Append(precomputedBodyHash);
            }
            else
            {
                if (sortedHeaders.TryGetValue(HeaderKeys.XAmzContentSha256Header, out string? contentHash))
                    canonicalRequest.Append(contentHash);
            }

            return canonicalRequest.ToString();
        }

        /// <summary>
        /// Reorders the headers for the request for canonicalization.
        /// </summary>
        /// <param name="requestHeaders">The set of proposed headers for the request</param>
        /// <returns>List of headers that must be included in the signature</returns>
        /// <remarks>For AWS4 signing, all headers are considered viable for inclusion</remarks>
        protected internal static IDictionary<string, string> SortAndPruneHeaders(IEnumerable<KeyValuePair<string, string>> requestHeaders)
        {
            // Refer https://docs.aws.amazon.com/general/latest/gr/sigv4-create-canonical-request.html. (Step #4: "Build the canonical headers list by sorting the (lowercase) headers by character code"). StringComparer.OrdinalIgnoreCase incorrectly places '_' after lowercase chracters.
            var sortedHeaders = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var header in requestHeaders)
            {
                if (_headersToIgnoreWhenSigning.Contains(header.Key))
                {
                    continue;
                }
                sortedHeaders.Add(header.Key.ToLowerInvariant(), header.Value);
            }
            
            return sortedHeaders;
        }

        /// <summary>
        /// Computes the canonical headers with values for the request. Only headers included in the signature
        /// are included in the canonicalization process.
        /// </summary>
        /// <param name="sortedHeaders">All request headers, sorted into canonical order</param>
        /// <returns>Canonicalized string of headers, with the header names in lower case.</returns>
        protected internal static string CanonicalizeHeaders(IEnumerable<KeyValuePair<string, string>> sortedHeaders)
        {
            if (sortedHeaders == null || !sortedHeaders.Any())
                return string.Empty;

            var builder = new StringBuilder();
            
            foreach (var entry in sortedHeaders)
            {
                // Refer https://docs.aws.amazon.com/general/latest/gr/sigv4-create-canonical-request.html. (Step #4: "To create the canonical headers list, convert all header names to lowercase and remove leading spaces and trailing spaces. Convert sequential spaces in the header value to a single space.").
                builder.Append(entry.Key.ToLowerInvariant());
                builder.Append(':');
                builder.Append(AWSSDKUtils.CompressSpaces(entry.Value)?.Trim());
                builder.Append('\n');
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns the set of headers included in the signature as a flattened, ;-delimited string
        /// </summary>
        /// <param name="sortedHeaders">The headers included in the signature</param>
        /// <returns>Formatted string of header names</returns>
        protected static string CanonicalizeHeaderNames(IEnumerable<KeyValuePair<string, string>> sortedHeaders)
        {
            var builder = new StringBuilder();
            
            foreach (var header in sortedHeaders)
            {
                if (builder.Length > 0)
                    builder.Append(';');
                builder.Append(header.Key.ToLowerInvariant());
            }
            
            return builder.ToString();
        }

        #endregion
    }
}
