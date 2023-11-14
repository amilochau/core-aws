using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal
{
    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(BatchWriteItemRequest))]
    [JsonSerializable(typeof(BatchWriteItemResponse))]
    internal partial class BatchWriteItemJsonSerializerContext : JsonSerializerContext
    {
    }

    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(DeleteItemRequest))]
    [JsonSerializable(typeof(DeleteItemResponse))]
    internal partial class DeleteItemJsonSerializerContext : JsonSerializerContext
    {
    }

    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(GetItemRequest))]
    [JsonSerializable(typeof(GetItemResponse))]
    internal partial class GetItemJsonSerializerContext : JsonSerializerContext
    {
    }

    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(PutItemRequest))]
    [JsonSerializable(typeof(PutItemResponse))]
    internal partial class PutItemJsonSerializerContext : JsonSerializerContext
    {
    }

    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(QueryRequest))]
    [JsonSerializable(typeof(QueryResponse))]
    internal partial class QueryJsonSerializerContext : JsonSerializerContext
    {
    }

    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(UpdateItemRequest))]
    [JsonSerializable(typeof(UpdateItemResponse))]
    internal partial class UpdateItemJsonSerializerContext : JsonSerializerContext
    {
    }
}
