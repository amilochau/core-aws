using Milochau.Core.Aws.XRayRecorder.Core.Internal.Entities;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.XRayRecorder.Models
{
    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(Entity))]
    [JsonSerializable(typeof(Segment))]
    [JsonSerializable(typeof(Subsegment))]
    internal partial class AwsJsonSerializerContext : JsonSerializerContext
    {
    }
}
