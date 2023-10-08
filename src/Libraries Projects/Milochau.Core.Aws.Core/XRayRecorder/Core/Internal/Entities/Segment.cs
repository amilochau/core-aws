﻿using System;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using System.Threading;

namespace Amazon.XRay.Recorder.Core.Internal.Entities
{
    /// <summary>
    /// A trace segment tracks a period of time associated with a computation or action, along with annotations and key / value data.
    /// A set of trace segments all of which share the same tracing ID form a trace.
    /// </summary>
    /// <seealso cref="Entity" />
    public class Segment : Entity
    {
        private long _size;           // Total number of subsegments
        private readonly Lazy<ConcurrentDictionary<string, object>> _lazyService = new Lazy<ConcurrentDictionary<string, object>>();

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
        public Segment(string name, string traceId = null, string parentId = null) : base(name)
        {
            this.TraceId = traceId ?? Entities.TraceId.NewId();

            if (parentId != null)
            {
                this.ParentId = parentId;
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
    }
}
