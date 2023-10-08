using System.Globalization;
using System.Text.RegularExpressions;

namespace Milochau.Core.Aws.XRayRecorder.Core.Internal.Utils
{
    /// <summary>
    /// Perform string matching using standard wildcards (globbing pattern).
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Convert a string from the camel case to snake case.
        /// </summary>
        /// <param name="camelCaseStr">The camel case string.</param>
        /// <returns>The converted snake case string.</returns>
        public static string FromCamelCaseToSnakeCase(this string camelCaseStr)
        {
            camelCaseStr = char.ToLower(camelCaseStr[0], CultureInfo.InvariantCulture) + camelCaseStr.Substring(1);
            string snakeCaseString = Regex.Replace(camelCaseStr, "(?<char>[A-Z])", match => '_' + match.Groups["char"].Value.ToLowerInvariant());
            return snakeCaseString;
        }
    }
}
