using System.Text.Json;
using Milochau.Core.Aws.XRayRecorder.Core.Internal.Entities;
using Milochau.Core.Aws.XRayRecorder.Models;

namespace Milochau.Core.Aws.XRayRecorder.Core.Internal.Emitters
{
    /// <summary>
    /// Convert a segment into JSON string
    /// </summary>
    public class JsonSegmentMarshaller : ISegmentMarshaller
    {
        private const string ProtocolHeader = "{\"format\":\"json\",\"version\":1}";
        private const char ProtocolDelimiter = '\n';

        /// <summary>
        /// Marshall the segment into JSON string
        /// </summary>
        /// <param name="segment">The segment to parse</param>
        /// <returns>The JSON string parsed from given segment</returns>
        public string Marshall(Entity segment)
        {
            var serializedEntity = JsonSerializer.Serialize(segment, AwsJsonSerializerContext.Default.Entity);
            return ProtocolHeader + ProtocolDelimiter + serializedEntity;
        }
    }
}
