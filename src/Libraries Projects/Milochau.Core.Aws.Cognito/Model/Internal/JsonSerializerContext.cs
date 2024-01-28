using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Cognito.Model.Internal
{
    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(InitiateAuthRequest))]
    [JsonSerializable(typeof(InitiateAuthResponse))]
    internal partial class InitiateAuthJsonSerializerContext : JsonSerializerContext
    {
    }
}
