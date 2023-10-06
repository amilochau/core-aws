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
 * Do not modify this file. This file is generated from the sesv2-2019-09-27.normal.json service model.
 */

using Amazon.Runtime.Endpoints;

namespace Amazon.SimpleEmailV2.Internal
{
    /// <summary>
    /// Amazon SimpleEmailServiceV2 endpoint provider.
    /// Resolves endpoint for given set of SimpleEmailServiceV2EndpointParameters.
    /// Can throw AmazonClientException if endpoint resolution is unsuccessful.
    /// </summary>
    public class AmazonSimpleEmailServiceV2EndpointProvider : IEndpointProvider
    {
        /// <summary>
        /// Resolve endpoint for SimpleEmailServiceV2EndpointParameters
        /// </summary>
        public Endpoint ResolveEndpoint(EndpointParameters parameters)
        {
            string region = parameters.Region;
            var dnsSuffix = "amazonaws.com";
            return new Endpoint($"https://email.{region}.{dnsSuffix}");
        }
    }
}