/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 * 
 *  http://aws.amazon.com/apache2.0
 * 
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

using Amazon.Util;
using System;

namespace Amazon.Runtime.Internal.Auth
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
        /// <param name="awsAccessKeyId">The access key that was included in the signature</param>
        /// <param name="signedAt">Date/time (UTC) that the signature was computed</param>
        /// <param name="signedHeaders">The collection of headers names that were included in the signature</param>
        /// <param name="scope">Formatted 'scope' value for signing (YYYYMMDD/region/service/aws4_request)</param>
        public AWSSigningResultBase(string awsAccessKeyId,
                                    string signedHeaders,
                                    string scope)
        {
            AccessKeyId = awsAccessKeyId;
            SignedHeaders = signedHeaders;
            Scope = scope;
        }

        /// <summary>
        /// The access key that was used in signature computation.
        /// </summary>
        public string AccessKeyId { get; }

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
