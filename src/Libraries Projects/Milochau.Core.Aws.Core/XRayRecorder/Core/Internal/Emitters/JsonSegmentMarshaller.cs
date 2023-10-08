using System.Text.Json;
using Amazon.XRay.Recorder.Core.Internal.Entities;
using Amazon.XRay.Recorder.Models;

namespace Amazon.XRay.Recorder.Core.Internal.Emitters
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
