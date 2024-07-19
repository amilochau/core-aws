using Milochau.Core.Aws.Core.Util;
using System.Text;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Auth
{
    /// <summary>
    /// Encapsulates the various fields and eventual signing value that makes up 
    /// an AWS4 signature. This can be used to retrieve the required authorization string
    /// or authorization query parameters for the final request as well as hold ongoing
    /// signature computations for subsequent calls related to the initial signing.
    /// </summary>
    /// <remarks>
    /// Constructs a new signing result instance for a computed signature
    /// </remarks>
    /// <param name="awsAccessKeyId">The access key that was included in the signature</param>
    /// <param name="signedHeaders">The collection of headers names that were included in the signature</param>
    /// <param name="scope">Formatted 'scope' value for signing (YYYYMMDD/region/service/aws4_request)</param>
    /// <param name="signature">Computed signature</param>
    public class AWS4SigningResult(string awsAccessKeyId,
                             string signedHeaders,
                             string scope,
                             byte[] signature)
    {

        /// <summary>
        /// The ;-delimited collection of header names that were included in the signature computation
        /// </summary>
        public string SignedHeaders { get; } = signedHeaders;

        /// <summary>
        /// Formatted 'scope' value for signing (YYYYMMDD/region/service/aws4_request)
        /// </summary>
        public string Scope { get; } = scope;

        /// <summary>
        /// Returns the hex string representing the signature
        /// </summary>
        public string Signature => AWSSDKUtils.ToHex(signature, true);

        /// <summary>
        /// Returns the signature in a form usable as an 'Authorization' header value.
        /// </summary>
        public string ForAuthorizationHeader => new StringBuilder()
            .Append(AWSSigner.AWS4AlgorithmTag)
            .AppendFormat(" {0}={1}/{2},", AWSSigner.Credential, awsAccessKeyId, Scope)
            .AppendFormat(" {0}={1},", AWSSigner.SignedHeaders, SignedHeaders)
            .AppendFormat(" {0}={1}", AWSSigner.Signature, Signature)
            .ToString();
    }
}
