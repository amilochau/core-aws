using System.Collections.Generic;

namespace Milochau.Core.Aws.Core.Runtime.Internal
{
    public abstract partial class AmazonWebServiceRequest
    {
        /// <summary>Get request parameters for XRay</summary>
        public virtual Dictionary<string, object?> GetXRayRequestParameters()
        {
            return new Dictionary<string, object?>();
        }

        /// <summary>Get request descriptors for XRay</summary>
        public virtual Dictionary<string, object?> GetXRayRequestDescriptors()
        {
            return new Dictionary<string, object?>();
        }
    }
}
