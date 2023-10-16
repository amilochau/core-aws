using Milochau.Core.Aws.Core.XRayRecorder.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities
{
    /// <summary>
    /// A trace segment tracks a period of time associated with a computation or action, along with annotations and key / value data.
    /// A set of trace segments all of which share the same tracing ID form a trace.
    /// </summary>
    /// <seealso cref="Entity" />
    public class Segment : Entity
    {
        private long _size;           // Total number of subsegments

        /// <summary>
        /// Gets the size of subsegments.
        /// </summary>
        [JsonIgnore]
        public long Size => Interlocked.Read(ref _size);

        /// <summary>
        /// Initializes a new instance of the <see cref="Segment"/> class.
        /// </summary>
        /// <param name="name">Name of the node or service component.</param>
        /// <param name="traceId">Unique id for the trace.</param>
        /// <param name="parentId">Unique id of the upstream segment.</param>
        public Segment(string name, string? traceId = null, string? parentId = null) : base(name)
        {
            TraceId = traceId ?? Entities.TraceId.NewId();

            if (parentId != null)
            {
                ParentId = parentId;
            }

            RootSegment = this;
        }

        /// <summary>
        /// Increment the size count.
        /// </summary>
        public void IncrementSize()
        {
            Interlocked.Increment(ref _size);
        }

        /// <summary>
        /// Decrement the size count.
        /// </summary>
        public void DecrementSize()
        {
            Interlocked.Decrement(ref _size);
        }

        /// <summary>
        /// Release reference to this instance of segment.
        /// </summary>
        /// <returns>Reference count after release.</returns>
        public override long Release()
        {
            return DecrementReferenceCounter();
        }

        /// <summary>
        /// Check if this segment or the root segment that this segment belongs to is ok to emit.
        /// </summary>
        /// <returns>If the segment is ready to emit.</returns>
        public override bool IsEmittable()
        {
            return Reference == 0;
        }

        /// <summary>
        /// Marshall the segment into JSON string
        /// </summary>
        /// <returns>The JSON string parsed from given segment</returns>
        public override string? Marshall()
        {
            var serializedEntity = JsonSerializer.Serialize(this, XRayJsonSerializerContext.Default.Segment);
            return ProtocolHeader + ProtocolDelimiter + serializedEntity;
        }
    }
}
