using System;
using System.Collections.Generic;

namespace Amazon.XRay.Recorder.Handlers.AwsSdk.Internal
{
    /// <summary>
    /// Utility class for AWS SDK handler.
    /// </summary>
    internal class AWSXRaySDKUtils
    {
        private static readonly String XRayServiceName = "XRay";
        private static readonly ISet<String> WhitelistedOperations = new HashSet<String> { "GetSamplingRules", "GetSamplingTargets" };

        // Collection to uniform service names across X-Ray SDKs.
        private static readonly Dictionary<string, string> FormattedServiceNames = new Dictionary<string, string>()
        {
            { "SimpleNotificationService" , "SNS" },
        };

        internal static bool IsBlacklistedOperation(String serviceName, string operation)
        {
            if (string.Equals(serviceName, XRayServiceName) && WhitelistedOperations.Contains(operation))
            {
                return true;
            }
            return false;
        }

        internal static string FormatServiceName(string serviceName)
        {
            if (FormattedServiceNames.TryGetValue(serviceName, out string formattedName))
            {
                return formattedName;
            }

            return serviceName;
        }
    }
}
