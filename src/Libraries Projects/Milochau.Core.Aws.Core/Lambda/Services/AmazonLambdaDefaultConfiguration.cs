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

/*
 * Do not modify this file. This file is generated from the lambda-2015-03-31.normal.json service model.
 */


using System;

using Amazon.Runtime;

namespace Amazon.Lambda
{
    /// <summary>
    /// Configuration for accessing Amazon Lambda service
    /// </summary>
    public static class AmazonLambdaDefaultConfiguration
    {
        /// <summary>
        /// <p>The STANDARD mode provides the latest recommended default values that should be safe to run in most scenarios</p><p>Note that the default values vended from this mode might change as best practices may evolve. As a result, it is encouraged to perform tests when upgrading the SDK</p>
        /// </summary>
        public static IDefaultConfiguration Standard { get; } = new DefaultConfiguration
        {
            StsRegionalEndpoints = StsRegionalEndpointsValue.Regional,
            S3UsEast1RegionalEndpoint = S3UsEast1RegionalEndpointValue.Regional,
            // 0:00:03.1
            ConnectTimeout = TimeSpan.FromMilliseconds(3100L),
            // 0:00:03.1
            TlsNegotiationTimeout = TimeSpan.FromMilliseconds(3100L),
            TimeToFirstByteTimeout = null,
            HttpRequestTimeout = null
        };
    }
}