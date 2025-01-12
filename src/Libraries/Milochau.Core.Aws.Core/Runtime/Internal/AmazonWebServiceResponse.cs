using System.Collections.Generic;
using System.Net;

namespace Milochau.Core.Aws.Core.Runtime.Internal
{
    /// <summary>
    /// Abstract class for Response objects, contains only metadata, 
    /// and no result information.
    /// </summary>
    public class AmazonWebServiceResponse
    {
        /// <summary>
        /// Contains additional information about the request, such as the 
        /// Request Id.
        /// </summary>
        public ResponseMetadata? ResponseMetadata { get; set; }

        /// <summary>
        /// Returns the status code of the HTTP response.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }

        /// <summary>Get response parameters for XRay</summary>
        public virtual Dictionary<string, object?> GetXRayResponseParameters()
        {
            return [];
        }

        /// <summary>Get response descriptors for XRay</summary>
        /// <remarks>
        /// See Amazon.XRay.Recorder.Handlers.AwsSdk.DefaultAWSWhitelist.json
        /// Key must be snake_case. Use rename_to value instead of property name as the key
        /// If map: true and get_keys: true : use propertyAsDict.Keys as the value
        /// If list: true and get_count: true : use propertyAsList.Count as the value
        /// </remarks>
        public virtual Dictionary<string, object?> GetXRayResponseDescriptors()
        {
            return [];
        }
    }
}
