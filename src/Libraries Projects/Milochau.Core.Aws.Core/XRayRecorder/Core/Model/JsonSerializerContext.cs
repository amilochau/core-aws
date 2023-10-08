using Amazon.XRay.Recorder.Core.Internal.Entities;
using System.Text.Json.Serialization;

namespace Amazon.XRay.Recorder.Models
{
    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(Entity))]
    internal partial class AwsJsonSerializerContext : JsonSerializerContext
    {
    }
}
