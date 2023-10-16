using System;
using System.Collections.Generic;

namespace Milochau.Core.Aws.Core.XRayRecorder.Handlers.AwsSdk.Entities
{
    /// <summary>
    /// Manifest of AWS Service Handler.
    /// </summary>
    public class AWSServiceHandlerManifest
    {
        /// <summary>
        /// Gets or sets the map of service name to AwsServiceInfo. The key of map ignores case.
        /// </summary>
        public Dictionary<string, AWSServiceHandler> Services { get; set; } = new Dictionary<string, AWSServiceHandler>(StringComparer.OrdinalIgnoreCase);
    }
}
