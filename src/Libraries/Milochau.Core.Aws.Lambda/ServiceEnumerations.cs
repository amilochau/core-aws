using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Lambda
{
    /// <summary>
    /// Constants used for properties of type InvocationType.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<InvocationType>))]
    public enum InvocationType
    {
        /// <summary>
        /// Constant DryRun for InvocationType
        /// </summary>
        DryRun,
        /// <summary>
        /// Constant Event for InvocationType
        /// </summary>
        Event,
        /// <summary>
        /// Constant RequestResponse for InvocationType
        /// </summary>
        RequestResponse,
    }
}