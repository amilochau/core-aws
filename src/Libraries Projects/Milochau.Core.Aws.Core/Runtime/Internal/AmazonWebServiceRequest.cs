using System.Collections.Generic;

namespace Milochau.Core.Aws.Core.Runtime.Internal
{
    public abstract partial class AmazonWebServiceRequest
    {
        /// <summary>Get request parameters for XRay</summary>
        public virtual Dictionary<string, string?> GetXRayRequestParameters()
        {
            return new Dictionary<string, string?>();
        }
    }
}
