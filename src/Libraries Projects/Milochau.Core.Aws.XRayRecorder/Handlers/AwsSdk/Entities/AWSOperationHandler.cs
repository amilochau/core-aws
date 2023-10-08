using System.Collections.Generic;

namespace Milochau.Core.Aws.XRayRecorder.Handlers.AwsSdk.Entities
{
    /// <summary>
    /// Handler for AWS services operation. It lists the information to be collected
    /// for the operation from request and response.
    /// </summary>
    public class AWSOperationHandler
    {
        /// <summary>
        /// Gets or sets the request parameters
        /// </summary>
        public List<string>? RequestParameters { get; set; }

        /// <summary>
        /// Gets or sets the response parameters
        /// </summary>
        public List<string>? ResponseParameters { get; set; }

        /// <summary>
        /// Gets or sets the request descriptors
        /// </summary>
        public Dictionary<string, AWSOperationRequestDescriptor>? RequestDescriptors { get; set; }
    }
}
