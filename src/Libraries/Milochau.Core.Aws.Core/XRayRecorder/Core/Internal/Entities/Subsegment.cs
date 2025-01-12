using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Emitters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities
{
    /// <summary>
    /// A trace subsegment tracks unit of computation within a trace segment (e.g. a method or function) or a downstream call.
    /// </summary>
    public class Subsegment(string name) : Entity(name)
    {
        //private readonly Lazy<HashSet<string>> lazyPrecursorIds = new();

        /// <summary>
        /// Gets or sets the namespace of the subsegment
        /// </summary>
        [JsonPropertyName("namespace")]
        public string? Namespace { get; set; }

        /// <summary>
        /// Gets or sets parent segment
        /// </summary>
        [JsonIgnore]
        public Entity? Parent { get; set; }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /*
        /// <summary>
        /// Gets the precursor ids
        /// </summary>
        [JsonPropertyName("precursor_ids")]
        public IEnumerable<string> PrecursorIds => lazyPrecursorIds.Value;

        /// <summary>
        /// Gets a value indicating whether precursor is has been added.
        /// </summary>
        /// <value>
        /// <c>true</c> if precursor id has been added; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool IsPrecursorIdAdded => lazyPrecursorIds.IsValueCreated && lazyPrecursorIds.Value.Any();

        /// <summary>
        /// Add the given precursor id to a set
        /// </summary>
        /// <param name="precursorId">The precursor id to add to the set</param>
        /// <returns>true if the id is added; false if the id is already present.</returns>
        /// <exception cref="ArgumentException">The given precursor id is not a valid segment id.</exception>
        public bool AddPrecursorId(string precursorId)
        {
            if (!IsIdValid(precursorId))
            {
                throw new ArgumentException("The precursor id is not a valid segment id: ", precursorId);
            }

            bool ret;
            lock (lazyPrecursorIds.Value)
            {
                ret = lazyPrecursorIds.Value.Add(precursorId);
            }

            return ret;
        }
        */

        /// <summary>
        /// Check if this segment or the root segment that this segment belongs to is ok to emit
        /// </summary>
        /// <returns>If the segment is ready to emit</returns>
        public override bool IsEmittable()
        {
            return Reference == 0 && Parent != null && Parent.IsEmittable();
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

        /// <summary>
        /// Marshall the segment into JSON string
        /// </summary>
        /// <returns>The JSON string parsed from given segment</returns>
        internal override string Marshall()
        {
            var serializedEntity = JsonSerializer.Serialize(this, XRayJsonSerializerContext.Default.Subsegment);
            return ProtocolHeader + ProtocolDelimiter + serializedEntity;
        }
    }
}
