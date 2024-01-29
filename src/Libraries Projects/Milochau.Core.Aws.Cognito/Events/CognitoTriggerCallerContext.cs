using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Cognito.Events
{
    /// <summary>
    /// https://docs.aws.amazon.com/cognito/latest/developerguide/cognito-user-identity-pools-working-with-aws-lambda-triggers.html#cognito-user-pools-lambda-trigger-syntax-shared
    /// </summary>
    public class CognitoTriggerCallerContext
    {
        /// <summary>
        /// The AWS SDK version number.
        /// </summary>
        [JsonPropertyName("awsSdkVersion")]
        public string AwsSdkVersion { get; set; } = null!;

        /// <summary>
        /// The ID of the client associated with the user pool.
        /// </summary>
        [JsonPropertyName("clientId")]
        public string ClientId { get; set; } = null!;
    }
}
