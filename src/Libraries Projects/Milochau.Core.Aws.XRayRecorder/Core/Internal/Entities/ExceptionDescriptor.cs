using System;
using System.Text.Json.Serialization;
using Milochau.Core.Aws.XRayRecorder.Core.Internal.Utils;

namespace Milochau.Core.Aws.XRayRecorder.Core.Internal.Entities
{
    /// <summary>Internal stack frame</summary>
    public class InternalStackFrame
    {
        /// <summary>Path</summary>
        [JsonPropertyName("path")]
        public string? Path { get; set; }

        /// <summary>Line</summary>
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
        public ExceptionDescriptor(string message, string type)
        {
            Id = ThreadSafeRandom.GenerateHexNumber(ExceptionDescriptorIdLength);
            Message = message;
            Type = type;
        }

        /// <summary>
        /// Gets or sets the id of the descriptor.
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

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
        /// The exception's "remote" attribute should be set to true if the exception on a "remote" subsegment is caused by or originated from a downstream service.
        /// </summary>
        [JsonPropertyName("remote")]
        public bool Remote { get; set;}
    }
}
