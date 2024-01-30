using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Contains details about the type of identity that made the request.
    /// </summary>
    public partial class Identity
    {
        /// <summary>
        /// Gets and sets the property PrincipalId. 
        /// <para>
        /// A unique identifier for the entity that made the call. For Time To Live, the principalId
        /// is "dynamodb.amazonaws.com".
        /// </para>
        /// </summary>
        [JsonPropertyName("principalId")]
        public required string PrincipalId { get; set; }

        /// <summary>
        /// Gets and sets the property Type. 
        /// <para>
        /// The type of the identity. For Time To Live, the type is "Service".
        /// </para>
        /// </summary>
        [JsonPropertyName("type")]
        public required string Type { get; set; }
    }
}
