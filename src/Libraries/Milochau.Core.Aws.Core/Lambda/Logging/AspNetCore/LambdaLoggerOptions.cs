namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Options that can be used to configure Lambda logging.
    /// </summary>
    public class LambdaLoggerOptions
    {
        /// <summary>
        /// Flag to indicate if Category should be part of logged message.
        /// </summary>
        public bool IncludeCategory { get; set; } = true;

        /// <summary>
        /// Flag to indicate if Exception should be part of logged message.
        /// </summary>
        public bool IncludeException { get; set; } = true;

        /// <summary>
        /// Flag to indicate if EventId should be part of logged message.
        /// </summary>
        public bool IncludeEventId { get; set; } = true;

        /// <summary>
        /// Whether scopes should be included in the message.
        /// </summary>
        public bool IncludeScopes { get; set; } = true;
    }
}
