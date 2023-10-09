using System.Collections.Generic;
using System.Globalization;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Util
{
    /// <summary>
    /// Stream wrapper to append trailing headers, including an optional
    /// rolling checksum for a request with an unsigned payload.
    /// </summary>
    public class TrailingHeadersWrapperStream
    {
        private const int NEWLINE_LENGTH = 2;            // additional length for any new lines
        private const int EMPTY_CHUNK_LENGTH = 3;        // additional length for an empty chunk "0CRLF"
        private const int HEADER_ROW_PADDING_LENGTH = 3; // additional length for each row of a trailing header, 1 for ':' between the key and value, plus 2 for CRLF

        /// <summary>
        /// Calculates the length in bytes of a TrailingChecksumWrapperStream initialized
        /// with the given trailing headers and optional checksum
        /// </summary>
        /// <param name="trailingHeaders">Dictionary of trailing headers</param>
        /// <param name="checksumAlgorithm">Trailing checksum</param>
        /// <param name="baseStreamLength">Length of the base stream in bytes</param>
        /// <returns>Length of a TrailingChecksumWrapperStream with given parameters, in bytes</returns>
        public static long CalculateLength(IDictionary<string, string> trailingHeaders, long baseStreamLength)
        {
            var prefixLength = baseStreamLength.ToString("X", CultureInfo.InvariantCulture).Length;
            var trailingHeaderLength = 0;

            if (trailingHeaders != null)
            {
                foreach (var key in trailingHeaders.Keys)
                {
                    trailingHeaderLength += key.Length + trailingHeaders[key].Length + HEADER_ROW_PADDING_LENGTH;
                }
            }

            return prefixLength +
                   NEWLINE_LENGTH +
                   baseStreamLength +
                   NEWLINE_LENGTH +
                   EMPTY_CHUNK_LENGTH +
                   trailingHeaderLength +
                   NEWLINE_LENGTH;
        }
    }
}
