using System.Globalization;
using System.Runtime.InteropServices;

namespace Milochau.Core.Aws.Core.Util.Internal
{
    public static partial class InternalSDKUtils
    {
        private const string UnknownPlaceholder = "Unknown";
        private const string UnknownPlatform = "unknown_platform";

        private static readonly string _userAgentBaseName = "aws-sdk-dotnet-coreclr";

        private static string GetValidSubstringOrUnknown(string str, int start, int end)
        {
            if ((start != -1) && (end != -1) &&
                (0 <= start) && (end <= str.Length))
            {
                string substr = str.Substring(start, end-start);
                if (!string.IsNullOrWhiteSpace(substr))
                {
                    return substr.Trim().Replace(' ', '_');
                }

            }
            return UnknownPlaceholder;
        }

        /// <summary>
        /// Returns the type of platform and version.!-- If on a special platform, a static "0" is used as the version since
        /// we have nothing more specific that actually means anything. Otherwise, asks InteropServices RuntimeInformation for
        /// the OSDescription and trims off the OS name.
        /// </summary>
        public static string DetermineFramework()
        {
            try
            {
                var desc = RuntimeInformation.FrameworkDescription.Trim();
                return string.Format(CultureInfo.InvariantCulture, ".NET_Core/{0}", GetValidSubstringOrUnknown(desc, desc.LastIndexOf(' ') + 1, desc.Length));
            }
            catch
            {
                return UnknownPlaceholder;
            }
        }


        /// <summary>
        /// Returns the special platform information (e.g. Unity_OSXEditor, Xamarin_AndroidTablet) if
        /// on those platforms, otherwise asks InteropServices RuntimeInformation for the OSDescription
        /// and trims off the version.
        /// </summary>
        public static string DetermineOS()
        {
            try
            {
                var desc = RuntimeInformation.OSDescription.Trim();
                return GetValidSubstringOrUnknown(desc, 0, desc.LastIndexOf(' '));
            }
            catch
            {
                return UnknownPlaceholder;
            }
        }

        /// <summary>
        /// Returns the special platform information (e.g. Unity_OSXEditor, Xamarin_AndroidTablet) if
        /// on those platforms, otherwise asks InteropServices RuntimeInformation for the OSDescription,
        /// keeping the version tail.
        /// </summary>
        public static string PlatformUserAgent()
        {
            try
            {
                var desc = RuntimeInformation.OSDescription;
                if (!string.IsNullOrWhiteSpace(desc))
                {
                    return desc.Trim().Replace(' ', '_');
                }
                return UnknownPlaceholder;
            }
            catch
            {
                return UnknownPlaceholder;
            }
        }
    }
}
