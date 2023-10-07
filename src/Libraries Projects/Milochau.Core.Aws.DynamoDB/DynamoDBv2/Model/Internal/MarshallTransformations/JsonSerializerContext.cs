using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(GetItemRequest))]
    internal partial class AwsJsonSerializerContext : JsonSerializerContext
    {
    }
}
