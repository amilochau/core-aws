using Milochau.Core.Aws.Core.Util.Internal.PlatformServices;
using System;
using System.Globalization;

namespace Milochau.Core.Aws.Core.Util.Internal
{
    public static partial class InternalSDKUtils
    {
        #region UserAgent
 
        public static string BuildUserAgentString(string serviceSdkVersion)
        {
            var environmentInfo = EnvironmentInfo.Instance;

            return string.Format(CultureInfo.InvariantCulture, "{0}/{1} aws-sdk-dotnet-core/{2} {3} OS/{4} {5}",
                _userAgentBaseName,
                serviceSdkVersion,
                CoreVersionNumber,
                environmentInfo.FrameworkUserAgent,
                environmentInfo.PlatformUserAgent,
                GetExecutionEnvironmentUserAgentString()).Trim();
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
