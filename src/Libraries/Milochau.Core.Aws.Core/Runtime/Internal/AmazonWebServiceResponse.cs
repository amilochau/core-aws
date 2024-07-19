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
    }
}
