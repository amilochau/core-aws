using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Sampling;
using Milochau.Core.Aws.Core.XRayRecorder.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Collections.Generic;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Emitters;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities
{
    /// <summary>
    /// A Facade segment tracks a period of time associated with a computation or action, along with annotations and key / value data.
    /// A set of trace segments all of which share the same tracing ID form a trace. This segment is created in AWS Lambda and only its subsegments are emitted.
    /// NOTE: This class should not be used. Its used internally by the SDK.
    /// </summary>
    /// <seealso cref="Entity" />
    public class FacadeSegment : Entity
    {
        /// <summary>Gets a readonly copy of the subsegment list.</summary>
        [JsonPropertyName("subsegments")]
        public List<Subsegment>? Subsegments { get; } = new List<Subsegment>();

        /// <summary>Constructor</summary>
        public FacadeSegment() : base("Facade")
        {
            var traceId = EnvironmentVariables.GetEnvironmentVariable(EnvironmentVariables.Key_TraceId);
            if (!TraceHeader.TryParseAll(traceId, out TraceHeader traceHeader))
            {
                traceHeader = new TraceHeader
                {
                    RootTraceId = Entities.TraceId.NewId(),
                    ParentId = null,
                    Sampled = SampleDecision.NotSampled
                };
            }

            TraceId = traceHeader.RootTraceId ?? Entities.TraceId.NewId();
            Id = traceHeader.ParentId;
            ParentId = traceHeader.ParentId;
            Sampled = traceHeader.Sampled;
        }

        /// <summary>
        /// Marshall the segment into JSON string
        /// </summary>
        /// <returns>The JSON string parsed from given segment</returns>
        public override string Marshall()
        {
            var serializedEntity = JsonSerializer.Serialize(this, XRayJsonSerializerContext.Default.FacadeSegment);
            return ProtocolHeader + ProtocolDelimiter + serializedEntity;
        }

        /// <summary>
        /// Streams subsegments of instance of <see cref="Entity"/>.
        /// </summary>
        /// <param name="emitter">Instance of <see cref="ISegmentEmitter"/>.</param>
        public void Stream(ISegmentEmitter emitter)
        {
            foreach (var subsegment in Subsegments)
            {
                subsegment.Stream(emitter);
            }

            Subsegments.RemoveAll(x => x.HasStreamed);
        }
    }
}
