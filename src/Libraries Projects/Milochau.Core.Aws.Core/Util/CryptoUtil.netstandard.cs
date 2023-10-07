/*******************************************************************************
 *  Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *  Licensed under the Apache License, Version 2.0 (the "License"). You may not use
 *  this file except in compliance with the License. A copy of the License is located at
 *
 *  http://aws.amazon.com/apache2.0
 *
 *  or in the "license" file accompanying this file.
 *  This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
 *  CONDITIONS OF ANY KIND, either express or implied. See the License for the
 *  specific language governing permissions and limitations under the License.
 * *****************************************************************************
 *    __  _    _  ___
 *   (  )( \/\/ )/ __)
 *   /__\ \    / \__ \
 *  (_)(_) \/\/  (___/
 *
 *  AWS SDK for .NET
 */
using System;
using System.Security.Cryptography;

namespace Amazon.Util
{
    public static partial class CryptoUtilFactory
    {
        partial class CryptoUtil : ICryptoUtil
        {
            public byte[] HMACSignBinary(byte[] data, byte[] key)
            {
                if (key == null || key.Length == 0)
                    throw new ArgumentNullException("key", "Please specify a Secret Signing Key.");

                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("data", "Please specify data to sign.");

                KeyedHashAlgorithm algorithm = new HMACSHA256();

                try
                {
                    algorithm.Key = key;
                    byte[] bytes = algorithm.ComputeHash(data);
                    return bytes;
                }
                finally
                {
                    algorithm.Dispose();
                }
            }

            [ThreadStatic]
            private static HashAlgorithm _hashAlgorithm = null;
            private static HashAlgorithm SHA256HashAlgorithmInstance
            {
                get
                {
                    if (null == _hashAlgorithm)
                    {
                        _hashAlgorithm = CreateSHA256Instance();
                    }
                    return _hashAlgorithm;
                }
            }

            internal static HashAlgorithm CreateSHA256Instance()
            {
                return SHA256.Create();
            }
        }
    }
}
