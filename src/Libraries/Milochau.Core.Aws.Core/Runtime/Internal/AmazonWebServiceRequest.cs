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
        /// <remarks>
        /// See Amazon.XRay.Recorder.Handlers.AwsSdk.DefaultAWSWhitelist.json
        /// Key must be snake_case
        /// </remarks>
        public virtual Dictionary<string, object?> GetXRayRequestParameters()
        {
            return [];
        }

        /// <summary>Get request descriptors for XRay</summary>
        /// <remarks>
        /// See Amazon.XRay.Recorder.Handlers.AwsSdk.DefaultAWSWhitelist.json
        /// Key must be snake_case. Use rename_to value instead of property name as the key
        /// If map: true and get_keys: true : use propertyAsDict.Keys as the value
        /// If list: true and get_count: true : use propertyAsList.Count as the value
        /// </remarks>
        public virtual Dictionary<string, object?> GetXRayRequestDescriptors()
        {
            return [];
        }
    }
}
