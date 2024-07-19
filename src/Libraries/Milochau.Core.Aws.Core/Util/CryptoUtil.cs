using System;
using System.Security.Cryptography;

namespace Milochau.Core.Aws.Core.Util
{
    public static partial class CryptoUtilFactory
    {
        public static ICryptoUtil CryptoInstance { get; } = new CryptoUtil();

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

            public byte[] HMACSignBinary(byte[] data, byte[] key)
            {
                if (key == null || key.Length == 0)
                    throw new ArgumentNullException(nameof(key), "Please specify a Secret Signing Key.");

                if (data == null || data.Length == 0)
                    throw new ArgumentNullException(nameof(data), "Please specify data to sign.");

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
            private static HashAlgorithm? _hashAlgorithm;
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
