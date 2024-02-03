using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Milochau.Core.Aws.Core.Util;
using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Runtime.Pipeline;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Milochau.Core.Aws.Core.Runtime.Credentials;

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
        public async Task SignAsync(IRequestContext requestContext, ImmutableCredentials immutableCredentials)
        {
            var signingResult = await SignRequestAsync(requestContext, immutableCredentials.AccessKey, immutableCredentials.SecretKey);
            requestContext.HttpRequestMessage.Headers.TryAddWithoutValidation(HeaderKeys.AuthorizationHeader, signingResult.ForAuthorizationHeader);
        }

        /// <summary>
        /// Calculates and signs the specified request using the AWS4 signing protocol by using the
        /// AWS account credentials given in the method parameters.
        /// </summary>
        /// <remarks>
        /// Parameters passed as part of the resource path should be uri-encoded prior to
        /// entry to the signer. Parameters passed in the request.Parameters collection should
        /// be not be encoded; encoding will be done for these parameters as part of the 
        /// construction of the canonical request.
        /// </remarks>
        private static async Task<AWS4SigningResult> SignRequestAsync(IRequestContext requestContext,
                                             string awsAccessKeyId,
                                             string awsSecretAccessKey)
        {
            var signedAt = InitializeHeaders(requestContext.HttpRequestMessage);

            var bodyHash = await SetRequestBodyHashAsync(requestContext);
            var sortedHeaders = SortAndPruneHeaders(requestContext.HttpRequestMessage.Headers);

            var canonicalRequest = CanonicalizeRequest(sortedHeaders,
                                                       bodyHash,
                                                       requestContext.HttpRequestMessage);

            return ComputeSignature(awsAccessKeyId,
                                    awsSecretAccessKey,
                                    signedAt,
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
        private static DateTime InitializeHeaders(HttpRequestMessage request)
        {
            var requestDateTime = DateTime.UtcNow;

            // clean up any prior signature in the headers if resigning
            request.Headers.Remove(HeaderKeys.AuthorizationHeader);
            request.Headers.Remove(HeaderKeys.XAmzContentSha256Header);

            if (!request.Headers.Contains(HeaderKeys.HostHeader))
            {
                request.Headers.Add(HeaderKeys.HostHeader, request.RequestUri!.Authority);
            }

            request.Headers.Add(HeaderKeys.XAmzDateHeader, requestDateTime.ToUniversalTime().ToString(AWSSDKUtils.ISO8601BasicDateTimeFormat, CultureInfo.InvariantCulture));

            return requestDateTime;
        }

        /// <summary>
        /// Computes and returns an AWS4 signature for the specified canonicalized request
        /// </summary>
        private static AWS4SigningResult ComputeSignature(string awsAccessKey,
                                                         string awsSecretAccessKey,
                                                         DateTime signedAt,
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

            var key = ComposeSigningKey(awsSecretAccessKey,
                                        region,
                                        dateStamp,
                                        service);

            var stringToSign = stringToSignBuilder.ToString();
            var signature = ComputeKeyedHash(key, stringToSign);
            return new AWS4SigningResult(awsAccessKey, signedHeaders, scope, signature);
        }

        /// <summary>
        /// Formats the supplied date and time for use in AWS4 signing, where various formats are used.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="formatString">The required format</param>
        /// <returns>The UTC date/time in the requested format</returns>
        private static string FormatDateTime(DateTime dt, string formatString)
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
        private static byte[] ComposeSigningKey(string awsSecretAccessKey, string region, string date, string service)
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
        /// <returns>
        /// The computed hash, whether already set in headers or computed here. Null
        /// if we were not able to compute a hash.
        /// </returns>
        private static async Task<string> SetRequestBodyHashAsync(IRequestContext requestContext)
        {
            // Calculate the hash and set it in the headers before returning
            byte[] payloadBytes = await requestContext.HttpRequestMessage.Content!.ReadAsByteArrayAsync();
            byte[] payloadHashBytes = CryptoUtilFactory.CryptoInstance.ComputeSHA256Hash(payloadBytes);
            string? computedContentHash = AWSSDKUtils.ToHex(payloadHashBytes, true);

            // set the header if needed and return it
            var payloadHash = computedContentHash ?? UnsignedPayload;
            requestContext.HttpRequestMessage.Headers.Add(HeaderKeys.XAmzContentSha256Header, payloadHash);
            return payloadHash;
        }

        /// <summary>
        /// Compute and return the hash of a data blob using the specified key
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="data">Data blob</param>
        /// <returns>Hash of the data</returns>
        private static byte[] ComputeKeyedHash(byte[] key, string data)
        {
            return ComputeKeyedHash(key, Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Compute and return the hash of a data blob using the specified key
        /// </summary>
        /// <param name="key">Hash key</param>
        /// <param name="data">Data blob</param>
        /// <returns>Hash of the data</returns>
        private static byte[] ComputeKeyedHash(byte[] key, byte[] data)
        {
            return CryptoUtilFactory.CryptoInstance.HMACSignBinary(data, key);
        }

        /// <summary>
        /// Computes the non-keyed hash of the supplied data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static byte[] ComputeHash(string data)
        {
            return CryptoUtilFactory.CryptoInstance.ComputeSHA256Hash(Encoding.UTF8.GetBytes(data));
        }

        #endregion

        #region Private Signing Helpers

        /// <summary>
        /// Computes and returns the canonical request
        /// </summary>
        /// <returns>Canonicalised request as a string</returns>
        private static string CanonicalizeRequest(IDictionary<string, string> sortedHeaders,
                                                    string? precomputedBodyHash,
                                                    HttpRequestMessage httpRequestMessage)
        {
            var canonicalRequest = new StringBuilder();
            canonicalRequest.AppendFormat("{0}\n", httpRequestMessage.Method.Method);
            canonicalRequest.AppendFormat("{0}\n", AWSSDKUtils.CanonicalizeResourcePathV2(new Uri(httpRequestMessage.RequestUri!.GetLeftPart(UriPartial.Authority)), httpRequestMessage.RequestUri.LocalPath));
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
        private static IDictionary<string, string> SortAndPruneHeaders(HttpRequestHeaders requestHeaders)
        {
            // Refer https://docs.aws.amazon.com/general/latest/gr/sigv4-create-canonical-request.html. (Step #4: "Build the canonical headers list by sorting the (lowercase) headers by character code"). StringComparer.OrdinalIgnoreCase incorrectly places '_' after lowercase chracters.
            var sortedHeaders = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var header in requestHeaders)
            {
                if (_headersToIgnoreWhenSigning.Contains(header.Key))
                {
                    continue;
                }
                sortedHeaders.Add(header.Key.ToLowerInvariant(), header.Value.First());
            }

            return sortedHeaders;
        }

        /// <summary>
        /// Computes the canonical headers with values for the request. Only headers included in the signature
        /// are included in the canonicalization process.
        /// </summary>
        /// <param name="sortedHeaders">All request headers, sorted into canonical order</param>
        /// <returns>Canonicalized string of headers, with the header names in lower case.</returns>
        private static string CanonicalizeHeaders(IEnumerable<KeyValuePair<string, string>> sortedHeaders)
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
        private static string CanonicalizeHeaderNames(IEnumerable<KeyValuePair<string, string>> sortedHeaders)
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
