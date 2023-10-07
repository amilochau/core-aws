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
        static CryptoUtil util = new CryptoUtil();

        public static ICryptoUtil CryptoInstance
        {
            get { return util; }
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
