namespace Amazon.XRay.Recorder.Handlers.AwsSdk.Entities
{
    /// <summary>
    /// Response descriptor for operation of AWS services. The difference between response descriptor
    /// and parameter is descriptor represents attribute with <see cref="List"/> type, and only the count
    /// of the list get collected.
    /// </summary>
    public class AWSOperationResponseDescriptor
    {
        /// <summary>
        /// Gets or sets the new name for the field
        /// </summary>
        public string RenameTo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the filed is a map
        /// </summary>
        public bool Map { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the key should be get
        /// </summary>
        public bool GetKeys { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the filed is a list
        /// </summary>
        public bool List { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the count of the list should be get
        /// </summary>
        public bool GetCount { get; set; }
    }
}
