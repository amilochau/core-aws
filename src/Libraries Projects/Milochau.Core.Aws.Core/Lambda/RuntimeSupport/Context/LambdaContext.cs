/*
 * Copyright 2019 Amazon.com, Inc. or its affiliates. All Rights Reserved.
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

using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport.Helpers;

namespace Amazon.Lambda.RuntimeSupport
{
    internal class LambdaContext : ILambdaContext
    {
        private LambdaEnvironment _lambdaEnvironment;
        private RuntimeApiHeaders _runtimeApiHeaders;
        private IConsoleLoggerWriter _consoleLogger;

        public LambdaContext(RuntimeApiHeaders runtimeApiHeaders, LambdaEnvironment lambdaEnvironment, IConsoleLoggerWriter consoleLogger)
        {

            _lambdaEnvironment = lambdaEnvironment;
            _runtimeApiHeaders = runtimeApiHeaders;
            _consoleLogger = consoleLogger;

            // set environment variable so that if the function uses the XRay client it will work correctly
            _lambdaEnvironment.SetXAmznTraceId(_runtimeApiHeaders.TraceId);
        }

        public string AwsRequestId => _runtimeApiHeaders.AwsRequestId;

        public ILambdaLogger Logger => new LambdaConsoleLogger(_consoleLogger);
    }
}
