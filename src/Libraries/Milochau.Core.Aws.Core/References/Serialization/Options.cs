using System.Text.Json.Serialization;
using System.Text.Json;

namespace Milochau.Core.Aws.Core.References.Serialization
{
    public class Options
    {
        /// <summary>JSON serializer options for AWS APIs</summary>
        public static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new DateTimeConverter(),
            }
        };
    }
}
