using System;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Utils
{
    /// <summary>
    /// An thread safe wrapper for System.Random
    /// </summary>
    public static class ThreadSafeRandom
    {
        private static readonly Random _global = new Random();
        private static readonly ThreadLocal<Random> _local = new ThreadLocal<Random>(() =>
        {
            int seed;
            lock (_global)
            {
                seed = _global.Next();
            }

            return new Random(seed);
        });

        /// <summary>
        /// Generate a random hex number
        /// </summary>
        /// <param name="digits">Digits of the hex number</param>
        /// <returns>The generated hex number</returns>
        public static string GenerateHexNumber(int digits)
        {
            byte[] bytes = new byte[digits / 2];
            NextBytes(bytes);
            string hexNumber = string.Concat(bytes.Select(x => x.ToString("x2", CultureInfo.InvariantCulture)).ToArray());
            if (digits % 2 != 0)
            {
                hexNumber += Next(16).ToString("x", CultureInfo.InvariantCulture);
            }

            return hexNumber;
        }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers
        /// </summary>
        /// <param name="buffer">An array of bytes to contain random numbers</param>
        private static void NextBytes(byte[] buffer)
        {
            _local.Value!.NextBytes(buffer);
        }

        /// <summary>
        /// Returns a non-negative random integer that is less than the specified maximum
        /// </summary>
        /// <param name="maxValue">Max value of the random integer</param>
        /// <returns>A 32-bit signed integer that is greater than or equal to 0, and less than maxValue</returns>
        private static int Next(int maxValue)
        {
            return _local.Value!.Next(maxValue);
        }
    }
}
