using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Core.XRayRecorder.Models
{
    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(FacadeSegment))]
    [JsonSerializable(typeof(Subsegment))]
    internal partial class XRayJsonSerializerContext : JsonSerializerContext
    {
    }
}
