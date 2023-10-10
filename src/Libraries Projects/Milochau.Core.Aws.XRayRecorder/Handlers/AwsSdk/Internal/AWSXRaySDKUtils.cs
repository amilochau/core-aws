using System.Collections.Generic;

namespace Milochau.Core.Aws.XRayRecorder.Handlers.AwsSdk.Internal
{
    /// <summary>
    /// Utility class for AWS SDK handler.
    /// </summary>
    internal class AWSXRaySDKUtils
    {
        private static readonly string XRayServiceName = "XRay";
        private static readonly ISet<string> WhitelistedOperations = new HashSet<string> { "GetSamplingRules", "GetSamplingTargets" };

        // Collection to uniform service names across X-Ray SDKs.
        private static readonly Dictionary<string, string> FormattedServiceNames = new Dictionary<string, string>()
        {
            { "SimpleNotificationService" , "SNS" },
        };

        internal static bool IsBlacklistedOperation(string serviceName, string operation)
        {
            if (string.Equals(serviceName, XRayServiceName) && WhitelistedOperations.Contains(operation))
            {
                return true;
            }
            return false;
        }

        internal static string FormatServiceName(string serviceName)
        {
            if (FormattedServiceNames.TryGetValue(serviceName, out string? formattedName))
            {
                return formattedName;
            }

            return serviceName;
        }
    }
}
