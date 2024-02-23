using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace Milochau.Core.Aws.Core.JsonConverters
{
    public class GuidConverter : JsonConverter<Guid>
    {
        public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return value == null ? default : Guid.Parse(value);
        }

        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("N"));
        }

        public override Guid ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return value == null ? default : Guid.Parse(value);
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, [DisallowNull] Guid value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(value.ToString("N"));
        }
    }
}
