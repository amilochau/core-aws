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

using Amazon.Runtime.Internal.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Linq;
using System.Net;
using Amazon.Runtime.Internal;
using Amazon.Runtime;
using Amazon.Util.Internal;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace Amazon.Util
{
    /// <summary>
    /// This class defines utilities and constants that can be used by 
    /// all the client libraries of the SDK.
    /// </summary>
    public static partial class AWSSDKUtils
    {
#region Internal Constants

        internal const string DefaultRegion = "us-east-1";
        internal const string DefaultGovRegion = "us-gov-west-1";

        private const char WindowsDirectorySeparatorChar = '\\';
        private const char WindowsAltDirectorySeparatorChar = '/';
        private const char WindowsVolumeSeparatorChar = ':';

        private const char SlashChar = '/';
        private const string Slash = "/";
        private const string EncodedSlash = "%2F";

        internal const int DefaultMaxRetry = 3;
        private const int DefaultConnectionLimit = 50;

        public static readonly DateTime EPOCH_START = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public const int DefaultBufferSize = 8192;

        // Default value of progress update interval for streaming is 100KB.
        public const long DefaultProgressUpdateInterval = 102400;

        internal static Dictionary<int, string> RFCEncodingSchemes = new Dictionary<int, string>
        {
            { 3986,  ValidUrlCharacters },
            { 1738,  ValidUrlCharactersRFC1738 }
        };

        internal const string S3Accelerate = "s3-accelerate";
        internal const string S3Control = "s3-control";

        private static readonly string _userAgent = InternalSDKUtils.BuildUserAgentString(string.Empty);

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
        /// The Set of accepted and valid Url characters per RFC1738. 
        /// Characters outside of this set will be encoded.
        /// </summary>
        public const string ValidUrlCharactersRFC1738 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.";

        /// <summary>
        /// The set of accepted and valid Url path characters per RFC3986.
        /// </summary>
        private static string ValidPathCharacters = DetermineValidPathCharacters();

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

        /// <summary>
        /// The RFC822Date Format string. Used when parsing date objects
        /// </summary>
        public const string RFC822DateFormat = "ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T";

#endregion

#region Internal Methods

        /**
         * Convert request parameters to Url encoded query string
         */
        internal static string GetParametersAsString(IRequest request)
        {
            return GetParametersAsString(request.ParameterCollection);
        }

        /// <summary>
        /// Returns the request parameters in the form of a query string.
        /// </summary>
        /// <param name="request">The request instance</param>
        /// <param name="usesQueryString">Optional parameter: if true, we will return an empty string</param>
        /// <returns>Request parameters in query string byte array format</returns>
        public static byte[] GetRequestPayloadBytes(IRequest request, bool? usesQueryString = null)
        {
            if (request.Content != null)
                return request.Content;

            string content;

            if(usesQueryString.HasValue && usesQueryString.Value)
            {
                content = string.Empty;
            }
            else
            {
                content = GetParametersAsString(request);
            }

            return Encoding.UTF8.GetBytes(content);
        }

        /**
         * Convert Dictionary of parameters to Url encoded query string
         */
        internal static string GetParametersAsString(ParameterCollection parameterCollection)
        {
            var sortedParameters = parameterCollection.GetSortedParametersList();

            StringBuilder data = new StringBuilder(512);
            foreach (var kvp in sortedParameters)
            {
                var key = kvp.Key;
                var value = kvp.Value;
                if (value != null)
                {
                    data.Append(key);
                    data.Append('=');
                    data.Append(AWSSDKUtils.UrlEncode(value, false));
                    data.Append('&');
                }
            }
            string result = data.ToString();
            if (result.Length == 0)
                return string.Empty;

            return result.Remove(result.Length - 1);
        }

        /// <summary>
        /// Returns the canonicalized resource path for the service endpoint.
        /// </summary>
        /// <param name="endpoint">Endpoint URL for the request.</param>
        /// <param name="resourcePath">Resource path for the request.</param>
        /// <param name="encode">If true will URL-encode path segments including "/". "S3" is currently the only service that does not expect pre URL-encoded segments.</param>
        /// <param name="pathResources">Dictionary of key/value parameters containing the values for the ResourcePath key replacements.</param>
        /// <remarks>If resourcePath begins or ends with slash, the resulting canonicalized path will follow suit.</remarks>
        /// <returns>Canonicalized resource path for the endpoint.</returns>
        public static string CanonicalizeResourcePathV2(Uri endpoint, string resourcePath, bool encode, IDictionary<string, string> pathResources)
        {
            if (endpoint != null)
            {
                var path = endpoint.AbsolutePath;
                if (string.IsNullOrEmpty(path) || string.Equals(path, Slash, StringComparison.Ordinal))
                    path = string.Empty;

                if (!string.IsNullOrEmpty(resourcePath) && resourcePath.StartsWith(Slash, StringComparison.Ordinal))
                    resourcePath = resourcePath.Substring(1);

                if (!string.IsNullOrEmpty(resourcePath))
                    path = path + Slash + resourcePath;

                resourcePath = path;
            }

            if (string.IsNullOrEmpty(resourcePath))
                return Slash;

            IEnumerable<string> encodedSegments = AWSSDKUtils.SplitResourcePathIntoSegments(resourcePath, pathResources);

            var pathWasPreEncoded = false;
            if (encode)
            {
                if (endpoint == null)
                    throw new ArgumentNullException(nameof(endpoint), "A non-null endpoint is necessary to decide whether or not to pre URL encode.");

                encodedSegments = encodedSegments.Select(segment => UrlEncode(segment, true).Replace(Slash, EncodedSlash));
                pathWasPreEncoded = true;
            }

            var canonicalizedResourcePath = AWSSDKUtils.JoinResourcePathSegments(encodedSegments, false);

            // Get the logger each time (it's cached) because we shouldn't store it in a static variable.
            Logger.GetLogger(typeof(AWSSDKUtils)).DebugFormat("{0} encoded {1}{2} for canonicalization: {3}",
                pathWasPreEncoded ? "Double" : "Single",
                resourcePath,
                endpoint == null ? "" : " with endpoint " + endpoint.AbsoluteUri,
                canonicalizedResourcePath);

            return canonicalizedResourcePath;
        }

        /// <summary>
        /// Splits the resourcePath at / into segments then resolves any keys with the path resource values. Greedy
        /// key values will be split into multiple segments at each /.
        /// </summary>
        /// <param name="resourcePath">The patterned resourcePath</param>
        /// <param name="pathResources">The key/value lookup for the patterned resourcePath</param>
        /// <returns>A list of path segments where all keys in the resourcePath have been resolved to one or more path segment values</returns>
        public static IEnumerable<string> SplitResourcePathIntoSegments(string resourcePath, IDictionary<string, string> pathResources)
        {
            var splitChars = new char[] { SlashChar };
            var pathSegments = resourcePath.Split(splitChars, StringSplitOptions.None);
            if(pathResources == null || pathResources.Count == 0)
            {
                return pathSegments;
            }

            //Otherwise there are key/values that need to be resolved
            var resolvedSegments = new List<string>();
            foreach(var segment in pathSegments)
            {
                if (!pathResources.ContainsKey(segment))
                {
                    resolvedSegments.Add(segment);
                    continue;
                }

                //Determine if the path is greedy. If greedy the segment will be split at each / into multiple segments.
                if (segment.EndsWith("+}", StringComparison.Ordinal))
                {
                    resolvedSegments.AddRange(pathResources[segment].Split(splitChars, StringSplitOptions.None));
                }
                else
                {
                    resolvedSegments.Add(pathResources[segment]);
                }
            }

            return resolvedSegments;
        }

        /// <summary>
        /// Joins all path segments with the / character and encodes each segment before joining.
        /// </summary>
        /// <param name="pathSegments">The segments of a URL path split at each / character</param>
        /// <param name="path">If the path property is specified,
        /// the accepted path characters {/+:} are not encoded.</param>
        /// <returns>A joined URL with encoded segments</returns>
        public static string JoinResourcePathSegments(IEnumerable<string> pathSegments, bool path)
        {
            // Encode for canonicalization
            pathSegments = pathSegments.Select(segment => UrlEncode(segment, path));

            if (path)
            {
                pathSegments = pathSegments.Select(segment => segment.Replace(Slash, EncodedSlash));
            }

            // join the encoded segments with /
            return string.Join(Slash, pathSegments.ToArray());
        }

        /// <summary>
        /// Takes a patterned resource path and resolves it using the key/value path resources into
        /// a segmented encoded URL.
        /// </summary>
        /// <param name="resourcePath">The patterned resourcePath</param>
        /// <param name="pathResources">The key/value lookup for the patterned resourcePath</param>
        /// <param name="skipEncodingValidPathChars">If true valid path characters {/+:} are not encoded</param>
        /// <returns>A segmented encoded URL</returns>
        public static string ResolveResourcePath(string resourcePath, IDictionary<string, string> pathResources, bool skipEncodingValidPathChars)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                return resourcePath;
            }

            return JoinResourcePathSegments(SplitResourcePathIntoSegments(resourcePath, pathResources), skipEncodingValidPathChars);
        }

        public static string ConvertToUnixEpochSecondsString(DateTime dateTime)
        {
            return Convert.ToInt64(GetTimeSpanInTicks(dateTime).TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        public static double ConvertToUnixEpochSecondsDouble(DateTime dateTime)
        {
            return Math.Round(GetTimeSpanInTicks(dateTime).TotalMilliseconds, 0) / 1000.0;
        }

        public static TimeSpan GetTimeSpanInTicks(DateTime dateTime)
        {
            return new TimeSpan(dateTime.ToUniversalTime().Ticks - EPOCH_START.Ticks);
        }

        public static long ConvertDateTimetoMilliseconds(DateTime dateTime)
        {
            return ConvertTimeSpanToMilliseconds(GetTimeSpanInTicks(dateTime));
        }

        public static long ConvertTimeSpanToMilliseconds(TimeSpan timeSpan)
        {
            return timeSpan.Ticks / TimeSpan.TicksPerMillisecond;
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
        /// Calls a specific EventHandler in a background thread
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="args"></param>
        /// <param name="sender"></param>
        public static void InvokeInBackground<T>(EventHandler<T> handler, T args, object sender) where T : EventArgs
        {
            if (handler == null) return;


            var list = handler.GetInvocationList();
            foreach (var call in list)
            {
                var eventHandler = ((EventHandler<T>)call);
                if (eventHandler != null)
                {
                    Dispatcher.Dispatch(() => eventHandler(sender, args));
                }
            }
        }

        private static BackgroundInvoker _dispatcher;

        private static BackgroundInvoker Dispatcher
        {
            get
            {
                if (_dispatcher == null)
                {
                    _dispatcher = new BackgroundInvoker();
                }

                return _dispatcher;
            }
        }

        internal static bool AreEqual(object[] itemsA, object[] itemsB)
        {
            if (itemsA == null || itemsB == null)
                return (itemsA == itemsB);

            if (itemsA.Length != itemsB.Length)
                return false;

            var length = itemsA.Length;
            for (int i = 0; i < length; i++)
            {
                var a = itemsA[i];
                var b = itemsB[i];
                if (!AreEqual(a, b))
                    return false;
            }

            return true;
        }

        internal static bool AreEqual(object a, object b)
        {
            if (a == null || b == null)
                return (a == b);

            if (object.ReferenceEquals(a, b))
                return true;

            return (a.Equals(b));
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
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException("bufferSize");

            byte[] array = new byte[bufferSize];
            int count;
            while ((count = source.Read(array, 0, array.Length)) != 0)
            {
                destination.Write(array, 0, count);
            }
        }
#endregion

#region Public Methods and Properties

        #region The code in this region has been minimally adapted from Microsoft's PathInternal.Windows.cs class as of 11/19/2019.  The logic remains the same.
        /// <summary>
        /// Returns true if the path specified is relative to the current drive or working directory.
        /// Returns false if the path is fixed to a specific drive or UNC path.  This method does no
        /// validation of the path (URIs will be returned as relative as a result).
        /// </summary>
        /// <remarks>
        /// Handles paths that use the alternate directory separator.  It is a frequent mistake to
        /// assume that rooted paths (Path.IsPathRooted) are not relative.  This isn't the case.
        /// "C:a" is drive relative- meaning that it will be resolved against the current directory
        /// for C: (rooted, but relative). "C:\a" is rooted and not relative (the current directory
        /// will not be used to modify the path).
        /// </remarks>
        private static bool IsPartiallyQualifiedForWindows(string path)
        {
            if (path.Length < 2)
            {
                // It isn't fixed, it must be relative.  There is no way to specify a fixed
                // path with one character (or less).
                return true;
            }

            if (IsWindowsDirectorySeparator(path[0]))
            {
                // There is no valid way to specify a relative path with two initial slashes or
                // \? as ? isn't valid for drive relative paths and \??\ is equivalent to \\?\
                return !(path[1] == '?' || IsWindowsDirectorySeparator(path[1]));
            }

            // The only way to specify a fixed path that doesn't begin with two slashes
            // is the drive, colon, slash format- i.e. C:\
            return !((path.Length >= 3)
                && (path[1] == WindowsVolumeSeparatorChar)
                && IsWindowsDirectorySeparator(path[2])
                // To match old behavior we'll check the drive character for validity as the path is technically
                // not qualified if you don't have a valid drive. "=:\" is the "=" file's default data stream.
                && IsValidWindowsDriveChar(path[0]));
        }

        /// <summary>
        /// True if the given character is a directory separator.
        /// </summary>
        private static bool IsWindowsDirectorySeparator(char c)
        {
            return c == WindowsDirectorySeparatorChar || c == WindowsAltDirectorySeparatorChar;
        }

        /// <summary>
        /// Returns true if the given character is a valid drive letter
        /// </summary>
        private static bool IsValidWindowsDriveChar(char value)
        {
            return (value >= 'A' && value <= 'Z') || (value >= 'a' && value <= 'z');
        }
#endregion The code in this region has been minimally adapted from Microsoft's PathInternal.Windows.cs class as of 11/19/2019.  The logic remains the same.

        /// <summary>
        /// URL encodes a string per RFC3986. If the path property is specified,
        /// the accepted path characters {/+:} are not encoded.
        /// </summary>
        /// <param name="data">The string to encode</param>
        /// <param name="path">Whether the string is a URL path or not</param>
        /// <returns>The encoded string</returns>
        public static string UrlEncode(string data, bool path)
        {
            return UrlEncode(3986, data, path);
        }

        /// <summary>
        /// URL encodes a string per the specified RFC. If the path property is specified,
        /// the accepted path characters {/+:} are not encoded.
        /// </summary>
        /// <param name="rfcNumber">RFC number determing safe characters</param>
        /// <param name="data">The string to encode</param>
        /// <param name="path">Whether the string is a URL path or not</param>
        /// <returns>The encoded string</returns>
        /// <remarks>
        /// Currently recognised RFC versions are 1738 (Dec '94) and 3986 (Jan '05). 
        /// If the specified RFC is not recognised, 3986 is used by default.
        /// </remarks>
        public static string UrlEncode(int rfcNumber, string data, bool path)
        {
            StringBuilder encoded = new StringBuilder(data.Length * 2);
            string validUrlCharacters;
            if (!RFCEncodingSchemes.TryGetValue(rfcNumber, out validUrlCharacters))
                validUrlCharacters = ValidUrlCharacters;

            string unreservedChars = String.Concat(validUrlCharacters, (path ? ValidPathCharacters : ""));

            foreach (char symbol in System.Text.Encoding.UTF8.GetBytes(data))
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
                
        internal static string UrlEncodeSlash(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return data;
            }

            return data.Replace("/", EncodedSlash);
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
            foreach (char symbol in System.Text.Encoding.UTF8.GetBytes(value))
            {
                if (ValidTraceIdHeaderValueCharacters.IndexOf(symbol) != -1)
                {
                    encoded.Append(symbol);
                }
                else
                {
                    encoded.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)symbol));
                }
            }

            return encoded.ToString();
        }

        /// <summary>
        /// URL encodes a string per the specified RFC with the exception of preserving the encoding of previously encoded slashes.
        /// If the path property is specified, the accepted path characters {/+:} are not encoded. 
        /// </summary>
        /// <param name="data">The string to encode</param>
        /// <param name="path">Whether the string is a URL path or not</param>
        /// <returns>The encoded string with any previously encoded %2F preserved</returns>
        [Obsolete("This method is not supported in AWSSDK 3.5")]
        public static string ProtectEncodedSlashUrlEncode(string data, bool path)
        {
            if (string.IsNullOrEmpty(data))
            {
                return data;
            }

            var index = 0;
            var sb = new StringBuilder();
            var findIndex = data.IndexOf(EncodedSlash, index, StringComparison.OrdinalIgnoreCase);
            while (findIndex != -1)
            {
                sb.Append(UrlEncode(data.Substring(index, findIndex - index), path));
                sb.Append(EncodedSlash);
                index = findIndex + EncodedSlash.Length;
                findIndex = data.IndexOf(EncodedSlash, index, StringComparison.OrdinalIgnoreCase);
            }            

            //If encoded slash was not found return the original data
            if(index == 0)
            {
                return UrlEncode(data, path);
            }

            if(data.Length > index)
            {
                sb.Append(UrlEncode(data.Substring(index), path));
            }
            
            return sb.ToString();
        }

        /// <summary>
        /// Generates an MD5 Digest for the stream specified
        /// </summary>
        /// <param name="input">The Stream for which the MD5 Digest needs
        /// to be computed.</param>
        /// <returns>A string representation of the hash with base64 encoding
        /// </returns>
        public static string GenerateMD5ChecksumForStream(Stream input)
        {
            string hash = null;

            if (!input.CanSeek)
                throw new InvalidOperationException("Input stream must be seekable");

            // Use an MD5 instance to compute the hash for the stream
            byte[] hashed = CryptoUtilFactory.CryptoInstance.ComputeMD5Hash(input);

            // Convert the hash to a Base64 Encoded string and return it
            hash = Convert.ToBase64String(hashed);

            // Now that the hash has been generated, reset the stream to its origin so that the stream's data can be processed
            input.Position = 0;

            return hash;
        }

        /// <summary>
        /// Generates an MD5 Digest for the given byte array
        /// </summary>
        /// <param name="content">The content for which the MD5 Digest needs
        /// to be computed.
        /// </param>
        /// <param name="fBase64Encode">Whether the returned checksum should be
        /// base64 encoded.
        /// </param>
        /// <returns>A string representation of the hash with or w/o base64 encoding
        /// </returns>
        public static string GenerateChecksumForBytes(byte[] content, bool fBase64Encode)
        {

            var hashed = content != null ?
                CryptoUtilFactory.CryptoInstance.ComputeMD5Hash(content) :
                CryptoUtilFactory.CryptoInstance.ComputeMD5Hash(ArrayEx.Empty<byte>());

            if (fBase64Encode)
            {
                // Convert the hash to a Base64 Encoded string and return it
                return Convert.ToBase64String(hashed);
            }
            else
            {
                return BitConverter.ToString(hashed).Replace("-", String.Empty);
            }
        }

        /// <summary>
        /// Returns DateTime.UtcNow + ManualClockCorrection when
        /// <seealso cref="AWSConfigs.ManualClockCorrection"/> is set.
        /// This value should be used instead of DateTime.UtcNow to factor in manual clock correction
        /// </summary>
        [Obsolete("This property does not account for endpoint specific clock skew.  Use CorrectClockSkew.GetCorrectedUtcNowForEndpoint() instead.")]
        public static DateTime CorrectedUtcNow
        {
            get
            {
                var now = AWSConfigs.utcNowSource();
                if (AWSConfigs.ManualClockCorrection.HasValue)
                    now += AWSConfigs.ManualClockCorrection.Value;
                return now;
            }
        }

        /// <summary>
        /// Returns true if the string has any bidirectional control characters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool HasBidiControlCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            foreach (var c in input)
            {
                if (IsBidiControlChar(c))
                    return true;
            }
            return false;
        }
        private static bool IsBidiControlChar(char c)
        {
            // check general range
            if (c < '\u200E' || c > '\u202E')
                return false;

            // check specific characters
            return (
                c == '\u200E' || // LRM
                c == '\u200F' || // RLM
                c == '\u202A' || // LRE
                c == '\u202B' || // RLE
                c == '\u202C' || // PDF
                c == '\u202D' || // LRO
                c == '\u202E'    // RLO
            );
        }

        /// <summary>
        /// Executes an HTTP request and returns the response as a string. This method
        /// throws WebException and HttpRequestException. In the event HttpRequestException
        /// is thrown the StatusCode is sent as user defined data on the exception under
        /// the key "StatusCode".
        /// </summary>
        /// <param name="uri">The URI to make the request to</param>
        /// <param name="requestType">The request type: GET, PUT, POST</param>
        /// <param name="content">null or the content to send with the request</param>
        /// <param name="timeout">Timeout for the request</param>
        /// <param name="proxy">Proxy for the request</param>
        /// <param name="headers">null or any headers to send with the request</param>
        /// <returns>The response as a string.</returns>
        public static string ExecuteHttpRequest(Uri uri, string requestType, string content, TimeSpan timeout, IWebProxy proxy, IDictionary<string, string> headers)
        {
            using (var client = CreateClient(uri, timeout, proxy, headers))
            {           
                var response = AsyncHelpers.RunSync<HttpResponseMessage>(() =>
                {
                    var requestMessage = new HttpRequestMessage(new HttpMethod(requestType), uri);
                    if(!string.IsNullOrEmpty(content))
                    {
                        requestMessage.Content = new StringContent(content);         
                    }
                                
                    return client.SendAsync(requestMessage);        
                });

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch(HttpRequestException e)
                {
                    var httpRequestException = new HttpRequestException(e.Message, e);
                    httpRequestException.Data.Add(nameof(response.StatusCode), response.StatusCode);

                    response.Dispose();
                    throw httpRequestException;
                }
                            
                try
                {
                    return AsyncHelpers.RunSync<string>(() =>
                    {
                        return response.Content.ReadAsStringAsync();        
                    });
                }
                finally 
                {
                    response.Dispose();
                }
            }
        }

        private static HttpClient CreateClient(Uri uri, TimeSpan timeout, IWebProxy proxy, IDictionary<string, string> headers)
        {
            var client = new HttpClient(new System.Net.Http.HttpClientHandler() { Proxy = proxy });
            
            if (timeout > TimeSpan.Zero)
            {
                client.Timeout = timeout;
            }
                
            //DefaultRequestHeaders should not be used if we reuse the HttpClient. It is currently created for each request.
            client.DefaultRequestHeaders.TryAddWithoutValidation(UserAgentHeader, _userAgent);
            if(headers != null)
            {
                foreach(var nameValue in headers)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(nameValue.Key, nameValue.Value);
                }
            }
                                
            return client;
        }

        /// <summary>
        /// Utility method that accepts a string and replaces white spaces with a space.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string CompressSpaces(string data)
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

#region Private Methods, Static Fields and Classes

        private class IsSetMethodsCacheKey
        {
            public readonly Type Type;
            public readonly string PropertyName;
            public IsSetMethodsCacheKey(Type type, string propertyName)
            {
                Type = type;
                PropertyName = propertyName;
            }
            public override bool Equals(object other)
            {
                var otherKey = other as IsSetMethodsCacheKey;
                if (otherKey == null)
                {
                    return false;
                }
                return Type == otherKey.Type && PropertyName == otherKey.PropertyName;
            }

            public override int GetHashCode()
            {
                return Type.GetHashCode() ^ PropertyName.GetHashCode();
            }

            public override string ToString()
            {
                return Type.FullName + "." + PropertyName;
            }
        }
#endregion
    }
}
