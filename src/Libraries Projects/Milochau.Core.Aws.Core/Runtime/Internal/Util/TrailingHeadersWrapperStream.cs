/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 * 
 *  http://aws.amazon.com/apache2.0
 * 
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */
using Amazon.Util;
using System.Collections.Generic;
using System.Globalization;

namespace Amazon.Runtime.Internal.Util
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
        public static long CalculateLength(IDictionary<string, string> trailingHeaders, CoreChecksumAlgorithm checksumAlgorithm, long baseStreamLength)
        {
            var prefixLength = baseStreamLength.ToString("X", CultureInfo.InvariantCulture).Length;
            var trailingHeaderLength = 0;

            if (trailingHeaders != null)
            {
                foreach (var key in trailingHeaders.Keys)
                {
                    if (checksumAlgorithm != CoreChecksumAlgorithm.NONE && ChecksumUtils.GetChecksumHeaderKey(checksumAlgorithm) == key)
                    {
                        trailingHeaderLength += key.Length +
                            CryptoUtilFactory.GetChecksumBase64Length(checksumAlgorithm) + HEADER_ROW_PADDING_LENGTH;
                    }
                    else
                    {
                        trailingHeaderLength += key.Length + trailingHeaders[key].Length + HEADER_ROW_PADDING_LENGTH;
                    }
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
