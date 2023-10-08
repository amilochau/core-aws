//-----------------------------------------------------------------------------
// <copyright file="StringExtension.cs" company="Amazon.com">
//      Copyright 2016 Amazon.com, Inc. or its affiliates. All Rights Reserved.
//
//      Licensed under the Apache License, Version 2.0 (the "License").
//      You may not use this file except in compliance with the License.
//      A copy of the License is located at
//
//      http://aws.amazon.com/apache2.0
//
//      or in the "license" file accompanying this file. This file is distributed
//      on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
//      express or implied. See the License for the specific language governing
//      permissions and limitations under the License.
// </copyright>
//-----------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Amazon.XRay.Recorder.Core.Internal.Utils
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
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Camel case start with lower case letter")]
        public static string FromCamelCaseToSnakeCase(this string camelCaseStr)
        {
            camelCaseStr = char.ToLower(camelCaseStr[0], CultureInfo.InvariantCulture) + camelCaseStr.Substring(1);
            string snakeCaseString = Regex.Replace(camelCaseStr, "(?<char>[A-Z])", match => '_' + match.Groups["char"].Value.ToLowerInvariant());
            return snakeCaseString;
        }
    }
}
