using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal
{
    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(BatchWriteItemRequest))]
    [JsonSerializable(typeof(BatchWriteItemResponse))]
    [JsonSerializable(typeof(DeleteItemRequest))]
    [JsonSerializable(typeof(DeleteItemResponse))]
    [JsonSerializable(typeof(GetItemRequest))]
    [JsonSerializable(typeof(GetItemResponse))]
    [JsonSerializable(typeof(PutItemRequest))]
    [JsonSerializable(typeof(PutItemResponse))]
    [JsonSerializable(typeof(QueryRequest))]
    [JsonSerializable(typeof(QueryResponse))]
    [JsonSerializable(typeof(UpdateItemRequest))]
    [JsonSerializable(typeof(UpdateItemResponse))]
    internal partial class AwsJsonSerializerContext : JsonSerializerContext
    {
    }
}
