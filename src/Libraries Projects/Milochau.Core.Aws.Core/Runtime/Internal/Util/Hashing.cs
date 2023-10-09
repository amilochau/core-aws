using System.Linq;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Util
{
    public static class Hashing
    {
        /// <summary>
        /// Hashes a set of objects.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Hash(params object[] value)
        {
            var hashes = value
                .Select(v => v == null ? 0 : v.GetHashCode())
                .ToArray();
            var result = CombineHashes(hashes);
            return result;
        }

        /// <summary>
        /// Combines a set of hashses.
        /// </summary>
        /// <param name="hashes"></param>
        /// <returns></returns>
        public static int CombineHashes(params int[] hashes)
        {
            int result = 0;
            foreach (int hash in hashes)
            {
                result = CombineHashesInternal(result, hash);
            }
            return result;
        }

        /// <summary>
        /// Combines two hashes.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static int CombineHashesInternal(int a, int b)
        {
            return unchecked(((a << 5) + a) ^ b);
        }
    }
}
