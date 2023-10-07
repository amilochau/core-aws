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

using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Endpoints;

#pragma warning disable 1591

namespace Amazon.Lambda.Internal
{
    /// <summary>
    /// Amazon Lambda endpoint resolver.
    /// Custom PipelineHandler responsible for resolving endpoint and setting authentication parameters for Lambda service requests.
    /// Collects values for LambdaEndpointParameters and then tries to resolve endpoint by calling 
    /// ResolveEndpoint method on GlobalEndpoints.Provider if present, otherwise uses LambdaEndpointProvider.
    /// Responsible for setting authentication and http headers provided by resolved endpoint.
    /// </summary>
    public class AmazonLambdaEndpointResolver : BaseEndpointResolver
    {
        protected override EndpointParameters MapEndpointsParameters(IRequestContext requestContext)
        {
            var config = (AmazonLambdaConfig)requestContext.ClientConfig;
            return new EndpointParameters
            {
                Region = config.RegionEndpoint?.SystemName
            };
        }
    }
}