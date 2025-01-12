using System.Collections.Generic;

namespace Milochau.Core.Aws.Core.XRayRecorder.Handlers.AwsSdk.Internal
{
    /// <summary>
    /// Utility class for AWS SDK handler.
    /// </summary>
    internal class AWSXRaySDKUtils
    {
        // Collection to uniform service names across X-Ray SDKs.
        private static readonly Dictionary<string, string> FormattedServiceNames = new()
        {
            { "SimpleNotificationService" , "SNS" },
        };

        internal static string FormatServiceName(string serviceName)
        {
            return FormattedServiceNames.TryGetValue(serviceName, out string? formattedName) ? formattedName : serviceName;
        }
    }
}
