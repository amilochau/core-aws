using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities
{
    /// <summary>
    /// Present the cause of fault and error in Segment and subsegment
    /// </summary>
    public class Cause(List<ExceptionDescriptor> exceptionDescriptors)
    {
        /// <summary>
        /// Gets the working directory
        /// </summary>
        [JsonPropertyName("working_directory")]
        public string? WorkingDirectory { get; private set; } = Directory.GetCurrentDirectory();

        /// <summary>
        /// Gets a read-only copy of the list of exception to the cause
        /// </summary>
        [JsonPropertyName("exceptions")]
        public List<ExceptionDescriptor>? ExceptionDescriptors { get; private set; } = exceptionDescriptors;
    }
}
