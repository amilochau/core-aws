using System.Collections.Generic;

namespace Milochau.Core.Aws.Core.XRayRecorder.Handlers.AwsSdk.Internal
{
    /// <summary>
    /// Utility class for AWS SDK handler.
    /// </summary>
    internal class AWSXRaySDKUtils
    {
        // Collection to uniform service names across X-Ray SDKs.
        private static readonly Dictionary<string, string> FormattedServiceNames = new Dictionary<string, string>()
        {
            { "SimpleNotificationService" , "SNS" },
        };

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
