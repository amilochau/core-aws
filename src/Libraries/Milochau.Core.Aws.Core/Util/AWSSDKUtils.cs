using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Linq;

namespace Milochau.Core.Aws.Core.Util
{
    /// <summary>
    /// This class defines utilities and constants that can be used by 
    /// all the client libraries of the SDK.
    /// </summary>
    public static partial class AWSSDKUtils
    {
        private const char SlashChar = '/';
        private const string Slash = "/";
        private const int DefaultBufferSize = 8192;

        /// <summary>
        /// The user agent string header
        /// </summary>
        public const string UserAgentHeader = "User-Agent";

        /// <summary>
        /// The Set of accepted and valid Url characters per RFC3986. 
        /// Characters outside of this set will be encoded.
        /// </summary>
        public const string ValidUrlCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        /// <summary>
        /// The set of characters which are not to be encoded as part of the X-Amzn-Trace-Id header values
        /// </summary>
        public const string ValidTraceIdHeaderValueCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-=;:+&[]{}\"',";

        /// <summary>
        /// The ISO8601 Basic date/time format string. Used when parsing date objects
        /// </summary>
        public const string ISO8601BasicDateTimeFormat = "yyyyMMddTHHmmssZ";

        /// <summary>
        /// The ISO8601 basic date format. Used during AWS4 signature computation.
        /// </summary>
        public const string ISO8601BasicDateFormat = "yyyyMMdd";

        /// <summary>
        /// Returns the canonicalized resource path for the service endpoint.
        /// </summary>
        /// <param name="endpoint">Endpoint URL for the request.</param>
        /// <param name="resourcePath">Resource path for the request.</param>
        /// <remarks>If resourcePath begins or ends with slash, the resulting canonicalized path will follow suit.</remarks>
        /// <returns>Canonicalized resource path for the endpoint.</returns>
        public static string CanonicalizeResourcePathV2(Uri endpoint, string resourcePath)
        {
            var path = endpoint.AbsolutePath;
            if (string.IsNullOrEmpty(path) || string.Equals(path, Slash, StringComparison.Ordinal))
                path = string.Empty;

            if (!string.IsNullOrEmpty(resourcePath) && resourcePath.StartsWith(Slash, StringComparison.Ordinal))
                resourcePath = resourcePath.Substring(1);

            if (!string.IsNullOrEmpty(resourcePath))
                path = path + Slash + resourcePath;

            resourcePath = path;

            if (string.IsNullOrEmpty(resourcePath))
                return Slash;

            IEnumerable<string> encodedSegments = resourcePath.Split(SlashChar, StringSplitOptions.None);

            encodedSegments = encodedSegments.Select(UrlEncode);

            return string.Join(Slash, encodedSegments.ToArray());
        }

        /// <summary>
        /// Helper function to format a byte array into string
        /// </summary>
        /// <param name="data">The data blob to process</param>
        /// <param name="lowercase">If true, returns hex digits in lower case form</param>
        /// <returns>String version of the data</returns>
        public static string ToHex(byte[] data, bool lowercase)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString(lowercase ? "x2" : "X2", CultureInfo.InvariantCulture));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Utility method for converting a string to a MemoryStream.
        /// </summary>
        public static MemoryStream GenerateMemoryStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Utility method for copy the contents of the source stream to the destination stream.
        /// </summary>
        public static void CopyStream(Stream source, Stream destination)
        {
            CopyStream(source, destination, DefaultBufferSize);
        }

        /// <summary>
        /// Utility method for copy the contents of the source stream to the destination stream.
        /// </summary>
        public static void CopyStream(Stream source, Stream destination, int bufferSize)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(destination);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bufferSize);

            byte[] array = new byte[bufferSize];
            int count;
            while ((count = source.Read(array, 0, array.Length)) != 0)
            {
                destination.Write(array, 0, count);
            }
        }

        /// <summary>
        /// URL encodes a string per RFC3986. If the path property is specified,
        /// the accepted path characters {/+:} are not encoded.
        /// </summary>
        /// <param name="data">The string to encode</param>
        /// <returns>The encoded string</returns>
        private static string UrlEncode(string data)
        {
            var encoded = new StringBuilder(data.Length * 2);

            var validUrlCharacters = ValidUrlCharacters;
            string unreservedChars = string.Concat(validUrlCharacters, "");

            foreach (char symbol in Encoding.UTF8.GetBytes(data).Select(v => (char)v))
            {
                if (unreservedChars.Contains(symbol))
                {
                    encoded.Append(symbol);
                }
                else
                {
                    encoded.Append('%');

                    // Break apart the byte into two four-bit components and
                    // then convert each into their hexadecimal equivalent.
                    byte b = (byte)symbol;
                    int hiNibble = b >> 4;
                    int loNibble = b & 0xF;
                    encoded.Append(ToUpperHex(hiNibble));
                    encoded.Append(ToUpperHex(loNibble));
                }
            }

            return encoded.ToString();
        }

        private static char ToUpperHex(int value)
        {
            // Maps 0-9 to the Unicode range of '0' - '9' (0x30 - 0x39).
            if (value <= 9)
            {
                return (char)(value + '0');
            }
            // Maps 10-15 to the Unicode range of 'A' - 'F' (0x41 - 0x46).
            return (char)(value - 10 + 'A');
        }

        /// <summary>
        /// Percent encodes the X-Amzn-Trace-Id header value skipping any characters within the
        /// ValidTraceIdHeaderValueCharacters character set.
        /// </summary>
        /// <param name="value">The X-Amzn-Trace-Id header value to encode.</param>
        /// <returns>An encoded X-Amzn-Trace-Id header value.</returns>
        internal static string EncodeTraceIdHeaderValue(string value)
        {
            var encoded = new StringBuilder(value.Length * 2);
            foreach (char symbol in Encoding.UTF8.GetBytes(value).Select(v => (char)v))
            {
                if (ValidTraceIdHeaderValueCharacters.Contains(symbol))
                {
                    encoded.Append(symbol);
                }
                else
                {
                    encoded.Append('%').Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)symbol));
                }
            }

            return encoded.ToString();
        }

        /// <summary>
        /// Utility method that accepts a string and replaces white spaces with a space.
        /// </summary>
        public static string? CompressSpaces(string? data)
        {
            if (data == null)
            {
                return null;
            }

            if (data.Length == 0)
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();
            var isWhiteSpace = false;
            foreach (var character in data)
            {
                if (!isWhiteSpace | !(isWhiteSpace = char.IsWhiteSpace(character)))
                {
                    stringBuilder.Append(isWhiteSpace ? ' ' : character);
                }
            }
            return stringBuilder.ToString();
        }
    }
}
