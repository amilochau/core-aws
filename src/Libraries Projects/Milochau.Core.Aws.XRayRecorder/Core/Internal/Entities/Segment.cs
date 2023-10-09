﻿using System;
using System.Text.Json.Serialization;
using System.Threading;

namespace Milochau.Core.Aws.XRayRecorder.Core.Internal.Entities
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
        /// Gets or sets the unique id for the trace.
        /// </summary>
        /// <exception cref="ArgumentException">Trace id is invalid. - value</exception>
        [JsonPropertyName("trace_id")]
        public string TraceId { get; set; }

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
    }
}
