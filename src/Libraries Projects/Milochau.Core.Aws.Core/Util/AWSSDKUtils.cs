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
#region Internal Constants

        private const char SlashChar = '/';
        private const string Slash = "/";
        private const string EncodedSlash = "%2F";

        public static readonly DateTime EPOCH_START = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public const int DefaultBufferSize = 8192;

#endregion

#region Public Constants

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
        /// The set of accepted and valid Url path characters per RFC3986.
        /// </summary>
        private static readonly string ValidPathCharacters = DetermineValidPathCharacters();

        /// <summary>
        /// The set of characters which are not to be encoded as part of the X-Amzn-Trace-Id header values
        /// </summary>
        public const string ValidTraceIdHeaderValueCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-=;:+&[]{}\"',";

        // Checks which path characters should not be encoded
        // This set will be different for .NET 4 and .NET 4.5, as
        // per http://msdn.microsoft.com/en-us/library/hh367887%28v=vs.110%29.aspx
        private static string DetermineValidPathCharacters()
        {
            const string basePathCharacters = "/:'()!*[]$";

            var sb = new StringBuilder();
            foreach (var c in basePathCharacters)
            {
                var escaped = Uri.EscapeDataString(c.ToString());
                if (escaped.Length == 1 && escaped[0] == c)
                    sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// The string representing Url Encoded Content in HTTP requests
        /// </summary>
        public const string UrlEncodedContent = "application/x-www-form-urlencoded; charset=utf-8";

        /// <summary>
        /// The GMT Date Format string. Used when parsing date objects
        /// </summary>
        public const string GMTDateFormat = "ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T";

        /// <summary>
        /// The ISO8601Date Format string. Used when parsing date objects
        /// </summary>
        public const string ISO8601DateFormat = "yyyy-MM-dd\\THH:mm:ss.fff\\Z";

        /// <summary>
        /// The ISO8601Date Format string. Used when parsing date objects
        /// </summary>
        public const string ISO8601DateFormatNoMS = "yyyy-MM-dd\\THH:mm:ss\\Z";

        /// <summary>
        /// The ISO8601 Basic date/time format string. Used when parsing date objects
        /// </summary>
        public const string ISO8601BasicDateTimeFormat = "yyyyMMddTHHmmssZ";

        /// <summary>
        /// The ISO8601 basic date format. Used during AWS4 signature computation.
        /// </summary>
        public const string ISO8601BasicDateFormat = "yyyyMMdd";

#endregion

#region Internal Methods

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
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString(lowercase ? "x2" : "X2", CultureInfo.InvariantCulture));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Utility method for converting a string to a MemoryStream.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static MemoryStream GenerateMemoryStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Utility method for copy the contents of the source stream to the destination stream.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void CopyStream(Stream source, Stream destination)
        {
            CopyStream(source, destination, DefaultBufferSize);
        }

        /// <summary>
        /// Utility method for copy the contents of the source stream to the destination stream.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="bufferSize"></param>
        public static void CopyStream(Stream source, Stream destination, int bufferSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            byte[] array = new byte[bufferSize];
            int count;
            while ((count = source.Read(array, 0, array.Length)) != 0)
            {
                destination.Write(array, 0, count);
            }
        }
#endregion

#region Public Methods and Properties

        /// <summary>
        /// URL encodes a string per RFC3986. If the path property is specified,
        /// the accepted path characters {/+:} are not encoded.
        /// </summary>
        /// <param name="data">The string to encode</param>
        /// <returns>The encoded string</returns>
        public static string UrlEncode(string data)
        {
            StringBuilder encoded = new StringBuilder(data.Length * 2);

            var validUrlCharacters = ValidUrlCharacters;
            string unreservedChars = string.Concat(validUrlCharacters, "");

            foreach (char symbol in Encoding.UTF8.GetBytes(data).Select(v => (char)v))
            {
                if (unreservedChars.IndexOf(symbol) != -1)
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
                if (ValidTraceIdHeaderValueCharacters.IndexOf(symbol) != -1)
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
        /// <param name="data"></param>
        /// <returns></returns>
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

#endregion
    }
}
