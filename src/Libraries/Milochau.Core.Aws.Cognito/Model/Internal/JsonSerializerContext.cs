using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Cognito.Model.Internal
{
    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(AdminUpdateUserAttributesRequest))]
    [JsonSerializable(typeof(AdminUpdateUserAttributesResponse))]
    internal partial class AdminUpdateUserAttributesJsonSerializerContext : JsonSerializerContext
    {
    }

    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(GetUserRequest))]
    [JsonSerializable(typeof(GetUserResponse))]
    internal partial class GetUserJsonSerializerContext : JsonSerializerContext
    {
    }

    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(InitiateAuthRequest))]
    [JsonSerializable(typeof(InitiateAuthResponse))]
    internal partial class InitiateAuthJsonSerializerContext : JsonSerializerContext
    {
    }
}
