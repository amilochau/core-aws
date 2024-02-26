using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Emitters;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Utils;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Sampling;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Strategies;
using Milochau.Core.Aws.Core.XRayRecorder.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities
{
    /// <summary>
    /// A trace subsegment tracks unit of computation within a trace segment (e.g. a method or function) or a downstream call.
    /// </summary>
    /// <seealso cref="Entity" />
    public class Subsegment : Entity
    {
        /// <summary>Defines exception serialization stategy to process recorded exceptions.</summary>
        private static readonly IExceptionSerializationStrategy exceptionSerializationStrategy = new DefaultExceptionSerializationStrategy();

        /// <summary>Emitter used to send Traces.</summary>
        private static readonly ISegmentEmitter emitter = new UdpSegmentEmitter();

        /// <summary>Initializes a new instance of the <see cref="Subsegment"/> class.</summary>
        public Subsegment(string name, FacadeSegment parent) : base(name)
        {
            Parent = parent;
            RootSegment = parent;
            Sampled = parent.Sampled;
            IsInProgress = true;
            StartTime = DateTime.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>Gets or sets the namespace of the subsegment</summary>
        [JsonPropertyName("namespace")]
        public string? Namespace { get; set; }

        /// <summary>Gets or sets the type</summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = "subsegment";

        /// <summary>Gets or sets a value indicating whether the segment has errored</summary>
        [JsonPropertyName("error")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool HasError { get; set; }

        /// <summary>Gets or sets a value indicating whether the remote segment is throttled</summary>
        [JsonPropertyName("throttle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsThrottled { get; set; }

        /// <summary>Gets the cause of fault or error</summary>
        [JsonPropertyName("cause")]
        public Cause? Cause { get; private set; }

        /// <summary>Gets or sets a value indicating whether the segment has faulted or failed</summary>
        [JsonPropertyName("fault")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool HasFault { get; set; }

        /// <summary>Gets or sets the start time of this segment with Unix time in seconds.</summary>
        [JsonPropertyName("start_time")]
        public decimal StartTime { get; set; }

        /// <summary>Gets or sets the end time of this segment with Unix time in seconds.</summary>
        [JsonPropertyName("end_time")]
        public decimal EndTime { get; set; }


        /// <summary>Gets or sets parent segment</summary>
        [JsonIgnore]
        public FacadeSegment Parent { get; }

        /// <summary>Gets or sets a value indicating whether the entity has been streamed</summary>
        [JsonIgnore]
        public bool HasStreamed { get; set; }

        /// <summary>Gets or sets a value indicating whether the segment is in progress</summary>
        [JsonIgnore]
        public bool IsInProgress { get; set; }

        /// <summary>Gets or sets the root segment</summary>
        [JsonIgnore]
        public FacadeSegment RootSegment { get; }

        /// <summary>
        /// Marshall the segment into JSON string
        /// </summary>
        /// <returns>The JSON string parsed from given segment</returns>
        public override string Marshall()
        {
            var serializedEntity = JsonSerializer.Serialize(this, XRayJsonSerializerContext.Default.Subsegment);
            return ProtocolHeader + ProtocolDelimiter + serializedEntity;
        }

        /// <summary>
        /// Adds the exception to cause and set this segment to has fault.
        /// </summary>
        /// <param name="e">The exception to be added.</param>
        public void AddException(Exception e)
        {
            HasFault = true;
            Cause = new Cause(exceptionSerializationStrategy.DescribeException(e));
        }

        /// <remarks>Not used for AWS services, but used for external calls</remarks>
        public void AddHttpInformation(HttpRequestMessage httpRequest)
        {
            if (httpRequest.RequestUri != null)
            {
                var requestAttributes = new Dictionary<string, object>
                {
                    ["method"] = httpRequest.Method.Method,
                    ["url"] = httpRequest.RequestUri.GetLeftPart(UriPartial.Path),
                };
                Http["request"] = requestAttributes;
            }
        }

        public void AddHttpInformation(HttpResponseMessage httpResponse)
        {
            int statusCode = (int)httpResponse.StatusCode;
            if (statusCode == 429)
            {
                HasError = false;
                HasFault = true;
                IsThrottled = true;
            }
            else if (statusCode >= 400 && statusCode <= 499)
            {
                HasError = true;
                HasFault = false;
            }
            else if (statusCode >= 500 && statusCode <= 599)
            {
                HasError = false;
                HasFault = true;
            }

            var responseAttributes = new Dictionary<string, object>
            {
                ["status"] = statusCode,
                ["content_length"] = httpResponse.Content.Headers.ContentLength ?? 0,
            };
            Http["response"] = responseAttributes;
        }

        /// <summary>
        /// Streams subsegments of instance of <see cref="Entity"/>.
        /// </summary>
        /// <param name="emitter">Instance of <see cref="ISegmentEmitter"/>.</param>
        public void Stream(ISegmentEmitter emitter)
        {
            if (Sampled != SampleDecision.Sampled || IsInProgress)
            {
                return;
            }

            TraceId = RootSegment?.TraceId;
            ParentId = Parent.Id;
            emitter.Send(this);
            HasStreamed = true;
        }

        /// <summary>End a subsegment.</summary>
        public void End()
        {
            IsInProgress = false;
            EndTime = DateTime.UtcNow.ToUnixTimeSeconds();

            Parent.Stream(emitter); // Facade segment is not emitted, all its subsegments, if emmittable, are emitted
        }
    }
}
