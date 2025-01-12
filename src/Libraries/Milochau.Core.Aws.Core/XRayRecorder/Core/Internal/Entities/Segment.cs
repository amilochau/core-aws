using Milochau.Core.Aws.Core.XRayRecorder.Core.Exceptions;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Emitters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities
{
    /// <summary>
    /// A trace segment tracks a period of time associated with a computation or action, along with annotations and key / value data.
    /// A set of trace segments all of which share the same tracing ID form a trace.
    /// </summary>
    public class Segment : Entity
    {
        private long _size;           // Total number of subsegments
        private readonly Lazy<ConcurrentDictionary<string, object>> lazyService = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Segment"/> class.
        /// </summary>
        /// <param name="name">Name of the node or service component.</param>
        /// <param name="traceId">Unique id for the trace.</param>
        /// <param name="parentId">Unique id of the upstream segment.</param>
        public Segment(string name, string? traceId = null, string? parentId = null) : base(name)
        {
            if (traceId != null)
            {
                TraceId = traceId;
            }
            else
            {
                TraceId = Entities.TraceId.NewId();
            }

            if (parentId != null)
            {
                ParentId = parentId;
            }

            RootSegment = this;
        }

        /// <summary>
        /// Gets or Sets the User for the segment
        /// </summary>
        [JsonPropertyName("user")]
        public string? User { get; set; }

        /// <summary>
        /// Gets or sets the origin of the segment.
        /// </summary>
        [JsonPropertyName("origin")]
        public string? Origin { get; set; }

        /// <summary>
        /// Gets the size of subsegments.
        /// </summary>
        [JsonIgnore]
        public long Size => Interlocked.Read(ref _size);

        /*
        /// <summary>
        /// Gets the service.
        /// </summary>
        [JsonPropertyName("service")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IDictionary<string, object> Service => lazyService.Value;
        */

        /// <summary>
        /// Gets a value indicating whether any value has been added to service.
        /// </summary>
        [JsonIgnore]
        public bool IsServiceAdded => lazyService.IsValueCreated && !lazyService.Value.IsEmpty;

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
        public override long Release() => DecrementReferenceCounter();

        /// <summary>
        /// Check if this segment or the root segment that this segment belongs to is ok to emit.
        /// </summary>
        /// <returns>If the segment is ready to emit.</returns>
        public override bool IsEmittable() => Reference == 0;

        /// <summary>
        /// Checks if the segment has been streamed already
        /// </summary>
        /// <exception cref="AlreadyEmittedException">The segment has been already streamed and no further operation can be performed on it.</exception>
        private void HasAlreadyStreamed()
        {
            if (HasStreamed)
            {
                throw new AlreadyEmittedException("Segment " + Name + " has already been emitted.");
            }
        }

        /// <summary>
        /// Gets the value of the User for this segment
        /// </summary>
        public string? GetUser() => User;

        /// <summary>
        /// Sets the User for this segment
        /// </summary>
        /// <param name="user">the name of the user</param>
        /// <exception cref="System.ArgumentNullException">The value of user cannot be null.</exception>
        public void SetUser(string user)
        {
            ArgumentNullException.ThrowIfNull(user);

            HasAlreadyStreamed();
            User = user;
        }

        /// <summary>
        /// Marshall the segment into JSON string
        /// </summary>
        /// <returns>The JSON string parsed from given segment</returns>
        internal override string Marshall()
        {
            var serializedEntity = JsonSerializer.Serialize(this, XRayJsonSerializerContext.Default.Segment);
            return ProtocolHeader + ProtocolDelimiter + serializedEntity;
        }
    }
}
