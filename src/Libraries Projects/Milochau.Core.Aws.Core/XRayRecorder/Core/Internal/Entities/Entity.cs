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
    public abstract class Entity
    {
        private const int SegmentIdHexDigits = 16;  // Number of hex digits in segment id

        /// <summary>Protocol header</summary>
        protected const string ProtocolHeader = "{\"format\":\"json\",\"version\":1}";
        /// <summary>Protocol delimiter</summary>
        protected const char ProtocolDelimiter = '\n';

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Entity(string name)
        {
            Id = ThreadSafeRandom.GenerateHexNumber(SegmentIdHexDigits);
            Name = name;
        }

        /// <summary>
        /// Gets or sets the unique id of segment.
        /// </summary>
        /// <value>
        /// The unique for Entity.
        /// </value>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

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
        public string Name { get; }

        /// <summary>
        /// Gets aws information
        /// </summary>
        [JsonPropertyName("aws")]
        public IDictionary<string, object?>? Aws { get; set; }

        /// <summary>
        /// Gets the http attribute
        /// </summary>
        [JsonPropertyName("http")]
        public IDictionary<string, Dictionary<string, long>>? Http { get; set; }

        /// <summary>
        /// Gets or sets the sample decision
        /// </summary>
        [JsonIgnore]
        public SampleDecision Sampled { get; set; }

        internal void AddToAws(string key, object? value)
        {
            Aws ??= new Dictionary<string, object?>();
            Aws[key] = value;
        }

        internal void AddToHttp(string key, Dictionary<string, long> value)
        {
            Http ??= new Dictionary<string, Dictionary<string, long>>();
            Http[key] = value;
        }

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
