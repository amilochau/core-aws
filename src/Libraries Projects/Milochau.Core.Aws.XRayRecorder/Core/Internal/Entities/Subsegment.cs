﻿using System;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.XRayRecorder.Core.Internal.Entities
{
    /// <summary>
    /// A trace subsegment tracks unit of computation within a trace segment (e.g. a method or function) or a downstream call.
    /// </summary>
    /// <seealso cref="Entity" />
    public class Subsegment : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subsegment"/> class.
        /// </summary>
        public Subsegment(string name, Entity parent) : base(name)
        {
            Parent = parent;
        }

        /// <summary>
        /// Gets or sets the unique id for the trace.
        /// </summary>
        /// <exception cref="ArgumentException">Trace id is invalid. - value</exception>
        [JsonPropertyName("trace_id")]
        public string? TraceId { get; set; }

        /// <summary>
        /// Gets or sets the namespace of the subsegment
        /// </summary>
        [JsonPropertyName("namespace")]
        public string? Namespace { get; set; }

        /// <summary>
        /// Gets or sets parent segment
        /// </summary>
        [JsonIgnore]
        public Entity Parent { get; set; }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Check if this segment or the root segment that this segment belongs to is ok to emit
        /// </summary>
        /// <returns>If the segment is ready to emit</returns>
        public override bool IsEmittable()
        {
            return Reference == 0 && Parent.IsEmittable();
        }

        /// <summary>
        /// Release reference to this instance of segment
        /// </summary>
        /// <returns>Reference count after release</returns>
        public override long Release()
        {
            long count = DecrementReferenceCounter();
            if (count == 0 && Parent != null)
            {
                Parent.Release();
            }

            return count;
        }
    }
}
