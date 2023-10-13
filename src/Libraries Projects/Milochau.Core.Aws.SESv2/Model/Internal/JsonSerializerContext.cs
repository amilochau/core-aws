using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.SESv2.Model.Internal
{
    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(SendEmailRequest))]
    [JsonSerializable(typeof(SendEmailResponse))]
    internal partial class SendEmailJsonSerializerContext : JsonSerializerContext
    {
    }
}
