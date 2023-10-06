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
using System;
using System.Globalization;
using Amazon.Util;
using System.Collections.Generic;

namespace Amazon.Runtime.Internal.Util
{
    /// <summary>
    /// Stream wrapper that double-buffers from a wrapped stream and
    /// returns the buffered content as a series of signed 'chunks'
    /// for the AWS4 ('Signature V4') protocol or the asymmetric Sigv4 (Sigv4a) protocol.
    /// </summary>
    public class ChunkedUploadWrapperStream
    {
        public static readonly int DefaultChunkSize = 81920;

        private const int NEWLINE_LENGTH = 2;
        private const int HEADER_ROW_PADDING_LENGTH = 3; // additional length for each row of a trailing header, 1 for ':' between the key and value, plus 2 for CRLF

        private const string CHUNK_SIGNATURE_HEADER = ";chunk-signature=";
        public const int V4_SIGNATURE_LENGTH = 64;
        private const string TRAILING_HEADER_SIGNATURE_KEY = "x-amz-trailer-signature";

        /// <summary>
        /// Computes the total size of the data payload, including the chunk metadata 
        /// and optional trailing headers. Called externally so as to be able to set 
        /// the correct Content-Length header value.
        /// </summary>
        /// <param name="originalLength">Length of the wrapped stream</param>
        /// <param name="signatureLength">Length of the signature for each chunk, in bytes</param>
        /// <param name="trailingHeaders">Optional trailing headers</param>
        /// <param name="trailingChecksum">Optional checksum algorithm for a trailing header</param>
        /// <returns>Total size of the wrapped payload, in bytes</returns>
        public static long ComputeChunkedContentLength(long originalLength, int signatureLength, IDictionary<string, string> trailingHeaders, CoreChecksumAlgorithm trailingChecksum)
        {
            if (originalLength < 0)
            {
                throw new ArgumentOutOfRangeException("originalLength", "Expected 0 or greater value for originalLength.");
            }

            int trailingHeaderLength = 0;
            long chunkedContentLength;

            // Calculate the size of the chunked content, before trailing headers/checksum
            if (originalLength == 0)
            {
                chunkedContentLength = CalculateChunkHeaderLength(0, signatureLength);
            }
            else
            {
                var maxSizeChunks = originalLength / DefaultChunkSize;
                var remainingBytes = originalLength % DefaultChunkSize;

                chunkedContentLength = maxSizeChunks * CalculateChunkHeaderLength(DefaultChunkSize, signatureLength)
                       + (remainingBytes > 0 ? CalculateChunkHeaderLength(remainingBytes, signatureLength) : 0)
                       + CalculateChunkHeaderLength(0, signatureLength);
            }

            if (trailingHeaders?.Count > 0)
            {
                foreach (var key in trailingHeaders.Keys)
                {
                    // If the trailing checksum key is already in dictionary, use the
                    // expected length since the checksum value may not be set yet.
                    if (trailingChecksum != CoreChecksumAlgorithm.NONE && ChecksumUtils.GetChecksumHeaderKey(trailingChecksum) == key)
                    {
                        trailingHeaderLength += key.Length +
                            CryptoUtilFactory.GetChecksumBase64Length(trailingChecksum) + HEADER_ROW_PADDING_LENGTH;
                    }
                    else
                    {
                        trailingHeaderLength += key.Length + trailingHeaders[key].Length + HEADER_ROW_PADDING_LENGTH;
                    }
                }

                trailingHeaderLength += TRAILING_HEADER_SIGNATURE_KEY.Length + signatureLength + HEADER_ROW_PADDING_LENGTH;
            }

            return chunkedContentLength + trailingHeaderLength;
        }

        /// <summary>
        /// Computes the size of the header data for each chunk.
        /// </summary>
        /// <param name="chunkDataSize">Payload size of each chunk, in bytes</param>
        /// <param name="signatureLength">Length of the signature for each chunk, in bytes</param>
        /// <returns></returns>
        private static long CalculateChunkHeaderLength(long chunkDataSize, int signatureLength)
        {
            return chunkDataSize.ToString("X", CultureInfo.InvariantCulture).Length
                   + CHUNK_SIGNATURE_HEADER.Length
                   + signatureLength
                   + NEWLINE_LENGTH
                   + chunkDataSize
                   + NEWLINE_LENGTH;
        }
    }
}
