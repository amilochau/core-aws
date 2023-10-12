namespace Milochau.Core.Aws.Core.Runtime.Internal.Auth
{
    /// <summary>
    /// Base class for the various fields and eventual signing value
    /// that make up an AWS request signature.
    /// </summary>
    public abstract class AWSSigningResultBase
    {
        /// <summary>
        /// Constructs a new signing result instance for a computed signature
        /// </summary>
        /// <param name="signedHeaders">The collection of headers names that were included in the signature</param>
        /// <param name="scope">Formatted 'scope' value for signing (YYYYMMDD/region/service/aws4_request)</param>
        public AWSSigningResultBase(string signedHeaders,
                                    string scope)
        {
            SignedHeaders = signedHeaders;
            Scope = scope;
        }

        /// <summary>
        /// The ;-delimited collection of header names that were included in the signature computation
        /// </summary>
        public string SignedHeaders { get; }

        /// <summary>
        /// Formatted 'scope' value for signing (YYYYMMDD/region/service/aws4_request)
        /// </summary>
        public string Scope { get; }

        public abstract string Signature { get; }

        public abstract string ForAuthorizationHeader { get; }
    }
}
