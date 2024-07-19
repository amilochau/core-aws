using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Serialization;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Utils;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Sampling;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities
{
    /// <summary>
    /// Represents the common part for both Segment and Subsegment.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="Entity"/> class.
    /// </remarks>
    /// <param name="name">The name.</param>
    public abstract class Entity(string name)
    {
        private const int SegmentIdHexDigits = 16;  // Number of hex digits in segment id

        /// <summary>Protocol header</summary>
        protected const string ProtocolHeader = "{\"format\":\"json\",\"version\":1}";
        /// <summary>Protocol delimiter</summary>
        protected const char ProtocolDelimiter = '\n';

        /// <summary>
        /// Gets or sets the unique id of segment.
        /// </summary>
        /// <value>
        /// The unique for Entity.
        /// </value>
        [JsonPropertyName("id")]
        public string? Id { get; set; } = ThreadSafeRandom.GenerateHexNumber(SegmentIdHexDigits);

        /// <summary>
        /// Gets or sets the unique id for the trace.
        /// </summary>
        [JsonPropertyName("trace_id")]
        public string? TraceId { get; set; }

        /// <summary>
        /// Gets or sets the unique id of upstream segment
        /// </summary>
        /// <value>
        /// The unique id for parent Entity.
        /// </value>
        [JsonPropertyName("parent_id")]
        public string? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the service component.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonPropertyName("name")]
        public string Name { get; } = name;

        /// <summary>
        /// Gets aws information
        /// </summary>
        [JsonPropertyName("aws")]
        public Dictionary<string, object?> Aws { get; set; } = [];

        /// <summary>
        /// Gets the http attribute
        /// </summary>
        [JsonPropertyName("http")]
        public Dictionary<string, Dictionary<string, object>> Http { get; set; } = [];

        /// <summary>
        /// Gets annotations, indexed
        /// </summary>
        [JsonPropertyName("annotations")]
        public Dictionary<string, object> Annotations { get; set; } = [];

        /// <summary>
        /// Gets metadata, not indexed
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; } = [];

        /// <summary>
        /// Gets or sets the sample decision
        /// </summary>
        [JsonIgnore]
        public SampleDecision Sampled { get; set; }

        /// <summary>
        /// Validate the segment id
        /// </summary>
        /// <param name="id">The segment id to be validate</param>
        /// <returns>A value indicates if the id is valid</returns>
        public static bool IsIdValid(string id)
        {
            return id.Length == SegmentIdHexDigits && long.TryParse(id, NumberStyles.HexNumber, null, out _);
        }

        /// <summary>
        /// Marshall the segment into JSON string
        /// </summary>
        /// <returns>The JSON string parsed from given segment</returns>
        public abstract string Marshall();
    }
}
