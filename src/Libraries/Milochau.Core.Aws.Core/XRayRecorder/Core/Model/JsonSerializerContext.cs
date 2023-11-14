using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Core.XRayRecorder.Models
{
    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(FacadeSegment))]
    [JsonSerializable(typeof(Subsegment))]
    [JsonSerializable(typeof(Dictionary<string, long>))]
    [JsonSerializable(typeof(Dictionary<string, string>))]
    internal partial class XRayJsonSerializerContext : JsonSerializerContext
    {
    }
}
