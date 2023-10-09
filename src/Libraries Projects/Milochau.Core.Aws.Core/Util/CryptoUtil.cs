using System.IO;

namespace Milochau.Core.Aws.Core.Util
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
