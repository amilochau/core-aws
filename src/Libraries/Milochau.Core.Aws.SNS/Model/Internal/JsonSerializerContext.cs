using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.SNS.Model.Internal
{
    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(PublishRequest))]
    [JsonSerializable(typeof(PublishResponse))]
    internal partial class PublishJsonSerializerContext : JsonSerializerContext
    {
    }
}
