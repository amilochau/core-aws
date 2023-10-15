using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities
{
    /// <summary>
    /// Present the cause of fault and error in Segment and subsegment
    /// </summary>
    public class Cause
    {
        /// <summary>
        /// Gets the working directory
        /// </summary>
        [JsonPropertyName("working_directory")]
        public string? WorkingDirectory { get;  private set; }

        /// <summary>
        /// Gets a read-only copy of the list of exception to the cause
        /// </summary>
        [JsonPropertyName("exceptions")]
        public List<ExceptionDescriptor>? ExceptionDescriptors { get; set; }

        private readonly object exceptionDescriptorsLock = new();
        /// <summary>
        /// Add list of <see cref="ExceptionDescriptor"/> to cause instance.
        /// </summary>
        /// <param name="exceptionDescriptors">List of <see cref="ExceptionDescriptor"/>.</param>
        public void AddException(List<ExceptionDescriptor> exceptionDescriptors)
        {
            WorkingDirectory = Directory.GetCurrentDirectory();
            lock (exceptionDescriptorsLock)
            {
                ExceptionDescriptors ??= new List<ExceptionDescriptor>();
                ExceptionDescriptors.AddRange(exceptionDescriptors);
            }
        }
    }
}
