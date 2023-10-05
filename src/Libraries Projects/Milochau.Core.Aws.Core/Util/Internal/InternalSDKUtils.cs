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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using Amazon.Runtime.Internal.Util;
using Amazon.Util.Internal.PlatformServices;

namespace Amazon.Util.Internal
{
    public static partial class InternalSDKUtils
    {
        #region UserAgent
        static string _customSdkUserAgent;
        static string _customData;
 
        public static string BuildUserAgentString(string serviceSdkVersion)
        {
            if (!string.IsNullOrEmpty(_customSdkUserAgent))
            {
                return _customSdkUserAgent;
            }

            var environmentInfo = EnvironmentInfo.Instance;

            return string.Format(CultureInfo.InvariantCulture, "{0}/{1} aws-sdk-dotnet-core/{2} {3} OS/{4} {5} {6}",
                _userAgentBaseName,
                serviceSdkVersion,
                CoreVersionNumber,
                environmentInfo.FrameworkUserAgent,
                environmentInfo.PlatformUserAgent,
                GetExecutionEnvironmentUserAgentString(),
                _customData).Trim();
        }


        #endregion

        internal static string EXECUTION_ENVIRONMENT_ENVVAR = "AWS_EXECUTION_ENV";
        internal static string GetExecutionEnvironment()
        {
            return Environment.GetEnvironmentVariable(EXECUTION_ENVIRONMENT_ENVVAR);
        }

        private static string GetExecutionEnvironmentUserAgentString()
        {
            string userAgentString = "";
            
            string executionEnvValue = GetExecutionEnvironment();
            if (!string.IsNullOrEmpty(executionEnvValue))
            {
                userAgentString = string.Format(CultureInfo.InvariantCulture, "exec-env/{0}", executionEnvValue);
            }

            return userAgentString;
        }
    }
}
