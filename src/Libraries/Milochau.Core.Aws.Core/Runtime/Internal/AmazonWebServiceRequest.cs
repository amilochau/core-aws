using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Core.Runtime.Internal
{
    public abstract partial class AmazonWebServiceRequest(Guid? userId)
    {
        /// <summary>User id, used as a monitoring annotation</summary>
        [JsonIgnore]
        public Guid? UserId { get; } = userId;

        /// <summary>Get request parameters for XRay</summary>
        public virtual Dictionary<string, object?> GetXRayRequestParameters()
        {
            return [];
        }
    }
}
