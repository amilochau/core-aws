using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal
{
    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(GetItemRequest))]
    internal partial class AwsJsonSerializerContext : JsonSerializerContext
    {
    }
}
