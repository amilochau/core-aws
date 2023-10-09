using System;
using System.Collections.Generic;

namespace Milochau.Core.Aws.XRayRecorder.Handlers.AwsSdk.Entities
{
    /// <summary>
    /// Handler for an AWS service. It contains a map of operation and its handler.
    /// </summary>
    public class AWSServiceHandler
    {
        /// <summary>
        /// Gets or sets the operations for the services
        /// </summary>
        public Dictionary<string, AWSOperationHandler> Operations { get; set; } = new Dictionary<string, AWSOperationHandler>(StringComparer.OrdinalIgnoreCase);
    }
}
