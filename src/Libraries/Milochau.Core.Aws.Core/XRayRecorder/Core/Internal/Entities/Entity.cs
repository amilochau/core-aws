using Milochau.Core.Aws.Core.XRayRecorder.Core.Sampling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Utils;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Exceptions;
using System.Text.Json.Serialization;

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

        private readonly Lazy<List<Subsegment>> _lazySubsegments = new Lazy<List<Subsegment>>();
        private readonly Lazy<Dictionary<string, string>> _lazyAnnotations = new Lazy<Dictionary<string, string>>();
        private readonly Lazy<Dictionary<string, object>> _lazyHttp = new Lazy<Dictionary<string, object>>();
        private readonly Lazy<Dictionary<string, object?>> _lazyAws = new Lazy<Dictionary<string, object?>>();

        private string? _traceId;
        private string? _id;
        private string? _parentId;
        private long _referenceCounter;      // Reference count

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Entity(string name)
        {
            Id = ThreadSafeRandom.GenerateHexNumber(SegmentIdHexDigits);
            IsInProgress = true;
            Name = name;
            IncrementReferenceCounter();
        }

        /// <summary>
        /// Gets or sets the unique id for the trace.
        /// </summary>
        /// <exception cref="System.ArgumentException">Trace id is invalid. - value</exception>
        [JsonPropertyName("trace_id")]
        public string? TraceId
        {
            get
            {
                return _traceId;
            }
            set
            {
                if (!Entities.TraceId.IsIdValid(value))
                {
                    throw new ArgumentException("Trace id is invalid.", nameof(value));
                }

                _traceId = value;
            }
        }

        /// <summary>
        /// Gets or sets the unique id of segment.
        /// </summary>
        /// <value>
        /// The unique for Entity.
        /// </value>
        /// <exception cref="System.ArgumentException">The id is invalid. - value</exception>
        [JsonPropertyName("id")]
        public string? Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != null && !IsIdValid(value))
                {
                    throw new ArgumentException("The id is invalid.", nameof(value));
                }

                _id = value;
            }
        }

        /// <summary>
        /// Gets or sets the start time of this segment with Unix time in seconds.
        /// </summary>
        [JsonPropertyName("start_time")]
        public decimal StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of this segment with Unix time in seconds.
        /// </summary>
        [JsonPropertyName("end_time")]
        public decimal EndTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the service component.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets a readonly copy of the subsegment list.
        /// </summary>
        [JsonPropertyName("subsegments")]
        public List<Subsegment> Subsegments => _lazySubsegments.Value;

        /// <summary>
        /// Gets a value indicating whether any subsegments have been added.
        /// </summary>
        /// <value>
        /// <c>true</c> if there are subsegments added; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool IsSubsegmentsAdded => _lazySubsegments.IsValueCreated && _lazySubsegments.Value.Count != 0;

        /// <summary>
        /// Gets or sets the unique id of upstream segment
        /// </summary>
        /// <value>
        /// The unique id for parent Entity.
        /// </value>
        /// <exception cref="System.ArgumentException">The parent id is invalid. - value</exception>
        [JsonPropertyName("parent_id")]
        public string? ParentId
        {
            get
            {
                return _parentId;
            }
            set
            {
                if (value != null && !IsIdValid(value))
                {
                    throw new ArgumentException("The parent id is invalid.", nameof(value));
                }

                _parentId = value;
            }
        }

        /// <summary>
        /// Gets the annotations of the segment
        /// </summary>
        [JsonPropertyName("annotations")]
        public Dictionary<string, string> Annotations => _lazyAnnotations.Value;

        /// <summary>
        /// Gets or sets a value indicating whether the segment has faulted or failed
        /// </summary>
        [JsonPropertyName("fault")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool HasFault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the segment has errored
        /// </summary>
        [JsonPropertyName("error")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool HasError { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the remote segment is throttled
        /// </summary>
        [JsonPropertyName("throttle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsThrottled { get; set; }

        /// <summary>
        /// Gets the cause of fault or error
        /// </summary>
        [JsonPropertyName("cause")]
        public Cause? Cause { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the segment is in progress
        /// </summary>
        [JsonIgnore]
        public bool IsInProgress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been streamed 
        /// </summary>
        [JsonIgnore]
        public bool HasStreamed { get; set; }

        /// <summary>
        /// Gets reference of this instance of segment
        /// </summary>
        [JsonIgnore]
        public long Reference => Interlocked.Read(ref _referenceCounter);

        /// <summary>
        /// Gets the http attribute
        /// </summary>
        [JsonPropertyName("http")]
        public Dictionary<string, object> Http => _lazyHttp.Value;

        /// <summary>
        /// Gets or sets the sample decision
        /// </summary>
        [JsonIgnore]
        public SampleDecision Sampled { get; set; }

        /// <summary>
        /// Gets or sets the root segment
        /// </summary>
        [JsonIgnore]
        public Segment? RootSegment { get; set; }

        /// <summary>
        /// Gets aws information
        /// </summary>
        [JsonPropertyName("aws")]
        public Dictionary<string, object?> Aws => _lazyAws.Value;

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
        /// Set start time of the entity to current time
        /// </summary>
        public void SetStartTimeToNow()
        {
            StartTime = DateTime.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Set end time of the entity to current time
        /// </summary>
        public void SetEndTimeToNow()
        {
            EndTime = DateTime.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Adds the specified key and value as annotation to current segment.
        /// The type of value is restricted. Only <see cref="string" />, <see cref="int" />, <see cref="long" />,
        /// <see cref="double" /> and <see cref="bool" /> are supported.
        /// </summary>
        /// <param name="key">The key of the annotation to add</param>
        /// <param name="value">The value of the annotation to add</param>
        /// <exception cref="System.ArgumentException">Key cannot be null or empty - key</exception>
        /// <exception cref="System.ArgumentNullException">value</exception>
        /// <exception cref="InvalidAnnotationException">The annotation to be added is invalid.</exception>
        public void AddAnnotation(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            }

            _lazyAnnotations.Value.Add(key, value);
            return;
        }

        /// <summary>
        /// Add a subsegment
        /// </summary>
        /// <param name="subsegment">The subsegment to add</param>
        /// <exception cref="EntityNotAvailableException">Cannot add subsegment to a completed segment.</exception>
        public void AddSubsegment(Subsegment subsegment)
        {
            if (!IsInProgress)
            {
                throw new EntityNotAvailableException("Cannot add subsegment to a completed segment.");
            }

            lock (_lazySubsegments.Value)
            {
                _lazySubsegments.Value.Add(subsegment);
            }

            IncrementReferenceCounter();
            subsegment.Parent = this;
            RootSegment?.IncrementSize();
        }

        /// <summary>
        /// Adds the exception to cause and set this segment to has fault.
        /// </summary>
        /// <param name="e">The exception to be added.</param>
        public void AddException(Exception e)
        {
            HasFault = true;
            Cause = new Cause(AWSXRayRecorder.Instance.ExceptionSerializationStrategy.DescribeException(e, Subsegments));
        }

        /// <summary>
        /// Check if this segment or the root segment that this segment belongs to is ok to emit
        /// </summary>
        /// <returns>If the segment is ready to emit</returns>
        public abstract bool IsEmittable();

        /// <summary>
        /// Release reference to this instance of segment
        /// </summary>
        /// <returns>Reference count after release</returns>
        public abstract long Release();

        /// <summary>
        /// Release reference to this instance of segment
        /// </summary>
        /// <returns>Reference count after release</returns>
        protected long DecrementReferenceCounter()
        {
            return Interlocked.Decrement(ref _referenceCounter);
        }

        /// <summary>
        /// Add reference to this instance of segment
        /// </summary>
        /// <returns>Reference count after add</returns>
        protected long IncrementReferenceCounter()
        {
            return Interlocked.Increment(ref _referenceCounter);
        }

        /// <summary>
        /// Marshall the segment into JSON string
        /// </summary>
        /// <returns>The JSON string parsed from given segment</returns>
        internal abstract string Marshall();
    }
}
