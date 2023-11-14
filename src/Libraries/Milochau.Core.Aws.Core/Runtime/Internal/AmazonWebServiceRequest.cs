using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Core.Runtime.Internal
{
    public abstract partial class AmazonWebServiceRequest
    {
        /// <summary>User id, used as a monitoring annotation</summary>
        [JsonIgnore]
        public string? UserId { get; set; }

        /// <summary>Get request parameters for XRay</summary>
        public virtual Dictionary<string, object?> GetXRayRequestParameters()
        {
            return new Dictionary<string, object?>();
        }
    }
}
