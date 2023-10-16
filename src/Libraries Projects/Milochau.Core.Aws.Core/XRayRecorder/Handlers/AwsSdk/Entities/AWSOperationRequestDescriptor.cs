namespace Milochau.Core.Aws.Core.XRayRecorder.Handlers.AwsSdk.Entities
{
    /// <summary>
    /// Request descriptor for operation of AWS service. The difference between request descriptor
    /// and parameter is descriptor represents attribute with Dictionary type, and only keys of the
    /// dictionary are collected. 
    /// </summary>
    public class AWSOperationRequestDescriptor
    {
        /// <summary>
        /// Gets or sets the new name of the field
        /// </summary>
        public string? RenameTo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the filed is a map
        /// </summary>
        public bool Map { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the key should be get
        /// </summary>
        public bool GetKeys { get; set; }
    }
}
