using System;
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
        /// List of <see cref="ExceptionDescriptor"/>
        /// </summary>
        private readonly Lazy<List<ExceptionDescriptor>> exceptions = new();

        /// <summary>
        /// Gets the working directory
        /// </summary>
        [JsonPropertyName("working_directory")]
        public string? WorkingDirectory { get; private set; } = Directory.GetCurrentDirectory();

        /// <summary>
        /// Gets the paths
        /// </summary>
        //public IList<string>? Paths { get; private set; }

        /// <summary>
        /// Gets a read-only copy of the list of exception to the cause
        /// </summary>
        [JsonPropertyName("exceptions")]
        public List<ExceptionDescriptor>? ExceptionDescriptors = exceptionDescriptors;

        /// <summary>
        /// Gets a value indicating whether any exception is added.
        /// </summary>
        /// <value>
        /// <c>true</c> if exception has been added; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool IsExceptionAdded => exceptions.IsValueCreated && exceptions.Value.Count != 0;
    }
}
