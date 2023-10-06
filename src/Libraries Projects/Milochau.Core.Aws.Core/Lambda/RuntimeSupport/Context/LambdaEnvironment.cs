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

using System;
using System.Linq;
using System.Reflection;

namespace Amazon.Lambda.RuntimeSupport
{
    /// <summary>
    /// Provides access to Environment Variables set by the Lambda runtime environment.
    /// </summary>
    public class LambdaEnvironment
    {
        internal const string EnvVarServerHostAndPort = "AWS_LAMBDA_RUNTIME_API";
        internal const string EnvVarTraceId = "_X_AMZN_TRACE_ID";

        internal LambdaEnvironment()
        {
            RuntimeServerHostAndPort = Environment.GetEnvironmentVariable(EnvVarServerHostAndPort);
        }

        internal void SetXAmznTraceId(string xAmznTraceId)
        {
            Environment.SetEnvironmentVariable(EnvVarTraceId, xAmznTraceId);
        }

        public string RuntimeServerHostAndPort { get; private set; }
    }
}
