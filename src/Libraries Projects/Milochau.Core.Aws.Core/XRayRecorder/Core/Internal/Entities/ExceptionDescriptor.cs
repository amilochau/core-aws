using System;
using System.Text.Json.Serialization;
using Amazon.XRay.Recorder.Core.Internal.Utils;

namespace Amazon.XRay.Recorder.Core.Internal.Entities
{
    public class InternalStackFrame
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("line")]
        public int Line { get; set; }
    }

    /// <summary>
    /// AWS X-Ray Descriptor of Exception
    /// </summary>
    public class ExceptionDescriptor
    {
        /// <summary>
        /// The exception descriptor identifier length
        /// </summary>
        public const int ExceptionDescriptorIdLength = 16;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionDescriptor"/> class.
        /// </summary>
        public ExceptionDescriptor()
        {
            Id = ThreadSafeRandom.GenerateHexNumber(ExceptionDescriptor.ExceptionDescriptorIdLength);
        }

        /// <summary>
        /// Gets or sets the id of the descriptor.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the stack.
        /// </summary>
        [JsonPropertyName("stack")]
        public InternalStackFrame[]? Stack { get; set; }

        /// <summary>
        /// Gets or sets the truncated.
        /// </summary>
        [JsonPropertyName("truncated")]
        public int? Truncated { get; set; }

        /// <summary>
        /// Gets or sets the cause.
        /// </summary>
        [JsonPropertyName("cause")]
        public string? Cause { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        [JsonIgnore]
        public Exception Exception { get; set; }

        /// <summary>
        /// The exception's "remote" attribute should be set to true if the exception on a "remote" subsegment is caused by or originated from a downstream service.
        /// </summary>
        [JsonPropertyName("remote")]
        public bool Remote { get; set;}
    }
}
