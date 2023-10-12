﻿using Milochau.Core.Aws.Core.References;
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
    public class AWS4SigningResult : AWSSigningResultBase
    {
        private readonly byte[] _signature;

        /// <summary>
        /// Constructs a new signing result instance for a computed signature
        /// </summary>
        /// <param name="signedHeaders">The collection of headers names that were included in the signature</param>
        /// <param name="scope">Formatted 'scope' value for signing (YYYYMMDD/region/service/aws4_request)</param>
        /// <param name="signature">Computed signature</param>
        public AWS4SigningResult(string signedHeaders,
                                 string scope,
                                 byte[] signature) :
            base(signedHeaders, scope)
        {
            _signature = signature;
        }

        /// <summary>
        /// Returns the hex string representing the signature
        /// </summary>
        public override string Signature
        {
            get { return AWSSDKUtils.ToHex(_signature, true); }
        }

        /// <summary>
        /// Returns the signature in a form usable as an 'Authorization' header value.
        /// </summary>
        public override string ForAuthorizationHeader
        {
            get
            {
                var authorizationHeader = new StringBuilder()
                    .Append(AWSSigner.AWS4AlgorithmTag)
                    .AppendFormat(" {0}={1}/{2},", AWSSigner.Credential, EnvironmentVariables.AccessKey, Scope)
                    .AppendFormat(" {0}={1},", AWSSigner.SignedHeaders, SignedHeaders)
                    .AppendFormat(" {0}={1}", AWSSigner.Signature, Signature);

                return authorizationHeader.ToString();
            }
        }
    }
}
