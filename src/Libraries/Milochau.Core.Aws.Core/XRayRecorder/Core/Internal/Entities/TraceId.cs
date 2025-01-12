using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Utils;
using System;
using System.Globalization;
using System.Numerics;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities
{
    /// <summary>
    /// Provides utilities to manipulate trace id.
    /// </summary>
    public static class TraceId
    {
        // Trace id contains three elements in a dash separated hex encoded string
        //      1. 4 bit version. Initially the only supported value is 1.
        //      2. Time as an integer number of seconds since the Epoch.
        //      3. 96 bit random number.
        //
        // Example:
        //      1-5759e988-bd862e3fe1be46a994272793
        //      | |        |
        //      | |        random number
        //      | epoch
        //      |
        //      version
        private const int Version = 1;
        private const int ElementsCount = 3;
        private const int RandomNumberHexDigits = 24; // 96 bits
        private const int EpochHexDigits = 8; // 32 bits
        private const int VersionDigits = 1;
        private const int TotalLength = RandomNumberHexDigits + EpochHexDigits + VersionDigits + ElementsCount - 1;
        private const char Delimiter = '-';

        /// <summary>
        /// Randomly generate a new trace id
        /// </summary>
        /// <returns>A new random trace id</returns>
        public static string NewId()
        {
            // Get epoch second as 32bit integer
            int epoch = (int)DateTime.UtcNow.ToUnixTimeSeconds();

            // Get a 96 bit random number
            string randomNumber = ThreadSafeRandom.GenerateHexNumber(RandomNumberHexDigits);

            string[] arr = [Version.ToString(CultureInfo.InvariantCulture), epoch.ToString("x", CultureInfo.InvariantCulture), randomNumber];

            // Concatenate elements with dash
            return string.Join(Delimiter.ToString(), arr);
        }

        /// <summary>
        /// Check whether the trace id is valid
        /// </summary>
        /// <param name="traceId">The trace id</param>
        /// <returns>True if the trace id is valid</returns>
        public static bool IsIdValid(string? traceId)
        {
            // Is the input valid?
            // Is the total length valid?
            if (string.IsNullOrWhiteSpace(traceId)
                || traceId.Length != TotalLength)
            {
                return false;
            }

            string[] elements = traceId.Split(Delimiter);
            var idEpoch = elements[1];
            var idRand = elements[2];

            // Is the number of elements valid?
            // Is the version a valid integer?
            // Is the version supported?
            // Is the size of epoch and random number valid?
            // Is the epoch a valid 32bit hex number?
            // Is the random number a valid hex number?
            return elements.Length == ElementsCount
                && int.TryParse(elements[0], out int idVersion)
                && Version == idVersion
                && idEpoch.Length == EpochHexDigits && idRand.Length == RandomNumberHexDigits
                && int.TryParse(idEpoch, NumberStyles.HexNumber, null, out _)
                && BigInteger.TryParse(idRand, NumberStyles.HexNumber, null, out _);
        }
    }
}
