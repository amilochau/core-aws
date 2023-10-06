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
using Amazon.Runtime;
using Amazon.Runtime.Internal.Util;
using System.IO;
using System.Security.Cryptography;

namespace Amazon.Util
{
    public static partial class CryptoUtilFactory
    {
        private const int SHA1_BASE64_LENGTH = 28;
        private const int SHA56_BASE64_LENGTH = 44;
        private const int CRC32_BASE64_LENGTH = 8;

        static CryptoUtil util = new CryptoUtil();

        public static ICryptoUtil CryptoInstance
        {
            get { return util; }
        }

        /// <summary>
        /// Returns a new instance of the specified hashing algorithm
        /// </summary>
        /// <param name="algorithm">Hashing algorithm to instantiate</param>
        /// <returns>New instance of the given algorithm</returns>
        public static HashAlgorithm GetChecksumInstance(CoreChecksumAlgorithm algorithm)
        {
            return algorithm switch
            {
                CoreChecksumAlgorithm.SHA1 => new SHA1Managed(),
                CoreChecksumAlgorithm.SHA256 => CryptoUtil.CreateSHA256Instance(),
                CoreChecksumAlgorithm.CRC32 => new CrtCrc32(),
                CoreChecksumAlgorithm.CRC32C => new CrtCrc32c(),
                _ => throw new AmazonClientException($"Unable to instantiate checksum algorithm {algorithm}"),
            };
        }

        /// <summary>
        /// Returns the length of the base64 encoded checksum of the specifed hashing algorithm
        /// </summary>
        /// <param name="algorithm">Hashing algorithm </param>
        /// <returns>Length of the base64 encoded checksum</returns>
        public static int GetChecksumBase64Length(CoreChecksumAlgorithm algorithm)
        {
            return algorithm switch
            {
                CoreChecksumAlgorithm.SHA1 => SHA1_BASE64_LENGTH,
                CoreChecksumAlgorithm.SHA256 => SHA56_BASE64_LENGTH,
                CoreChecksumAlgorithm.CRC32 or CoreChecksumAlgorithm.CRC32C => CRC32_BASE64_LENGTH,
                _ => throw new AmazonClientException($"Unable to determine the base64-encoded length of {algorithm}"),
            };
        }

        partial class CryptoUtil : ICryptoUtil
        {
            internal CryptoUtil()
            {
            }

            /// <summary>
            /// Computes a SHA256 hash
            /// </summary>
            /// <param name="data">Input to compute the hash code for</param>
            /// <returns>Computed hash code</returns>
            public byte[] ComputeSHA256Hash(byte[] data)
            {
                return SHA256HashAlgorithmInstance.ComputeHash(data);
            }

            /// <summary>
            /// Computes a SHA256 hash
            /// </summary>
            /// <param name="steam">Input to compute the hash code for</param>
            /// <returns>Computed hash code</returns>
            public byte[] ComputeSHA256Hash(Stream steam)
            {
                return SHA256HashAlgorithmInstance.ComputeHash(steam);
            }
        }
    }
}
