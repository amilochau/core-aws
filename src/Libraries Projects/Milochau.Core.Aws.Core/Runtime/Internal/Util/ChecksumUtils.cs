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

using Amazon.Runtime.Internal.Transform;
using Amazon.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Amazon.Runtime.Internal.Util
{
    /// <summary>
    /// Utilities for working with the checksums used to validate request/response integrity
    /// </summary>
    public static class ChecksumUtils
    {
        /// <summary>
        /// Generates the name of the header key to use for a given checksum algorithm
        /// </summary>
        /// <param name="checksumAlgorithm">Checksum algorithm</param>
        /// <returns>Name of the HTTP header key for the given algorithm</returns>
        internal static string GetChecksumHeaderKey(CoreChecksumAlgorithm checksumAlgorithm)
        {
            return $"x-amz-checksum-{checksumAlgorithm.ToString().ToLower()}";
        }

        /// <summary>
        /// Selects which checksum to use to validate the integrity of a response
        /// </summary>
        /// <param name="operationSupportedChecksums">List of checksums supported by the service operation</param>
        /// <param name="responseData">Response from the service, which potentially contains x-amz-checksum-* headers</param>
        /// <returns>Single checksum algorithm to use for validating the response, or NONE if checksum validation should be skipped</returns>
        public static CoreChecksumAlgorithm SelectChecksumForResponseValidation(ICollection<CoreChecksumAlgorithm> operationSupportedChecksums, IWebResponseData responseData)
        {
            if (operationSupportedChecksums == null || operationSupportedChecksums.Count == 0 || responseData == null)
            {
                return CoreChecksumAlgorithm.NONE;
            }

            // Checksums to use for validation in order of speed (via CRT profiling)
            CoreChecksumAlgorithm[] checksumsInPriorityOrder =
            {
                CoreChecksumAlgorithm.CRC32C,
                CoreChecksumAlgorithm.CRC32,
                CoreChecksumAlgorithm.SHA1,
                CoreChecksumAlgorithm.SHA256
            };

            foreach (var algorithm in checksumsInPriorityOrder)
            {
                if (operationSupportedChecksums.Contains(algorithm))
                {
                    var headerKey = GetChecksumHeaderKey(algorithm);
                    if (responseData.IsHeaderPresent(headerKey) && !IsChecksumValueMultipartGet(responseData.GetHeaderValue(headerKey)))
                    {
                        return algorithm;
                    }
                }
            }

            return CoreChecksumAlgorithm.NONE;
        }

        /// <summary>
        /// Determines if a checksum value is for a whole S3 multipart object, which must skip validation.
        /// These checksums end with `-#`, where # is an integer between 1 and 10000
        /// </summary>
        /// <param name="checksumValue">Base 64 checksum value</param>
        /// <returns>True if the checksum is for an S3 multipart object, false otherwise</returns>
        private static bool IsChecksumValueMultipartGet(string checksumValue)
        {
            if (string.IsNullOrEmpty(checksumValue))
            {
                return false;
            }

            var lastDashIndex = checksumValue.LastIndexOf('-');
            if (lastDashIndex == -1)
            {
                return false;
            }

            int partNumber;
            var isInteger = int.TryParse(checksumValue.Substring(lastDashIndex + 1), out partNumber);

            if (!isInteger)
            {
                return false;
            }

            if (partNumber >= 1 && partNumber <= 10000)
            {
                return true;
            }

            return false;
        }
    }
}
