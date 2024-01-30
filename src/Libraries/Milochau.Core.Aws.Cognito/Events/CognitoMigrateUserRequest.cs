using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Cognito.Events
{
    /// <summary>
    /// https://docs.aws.amazon.com/cognito/latest/developerguide/user-pool-lambda-migrate-user.html
    /// </summary>
    public class CognitoMigrateUserRequest : CognitoTriggerRequest
    {
        /// <summary>
        /// The username entered by the user.
        /// </summary>
        [JsonPropertyName("userName")]
        public required string UserName { get; set; }

        /// <summary>
        /// The password entered by the user for sign-in. It is not set in the forgot-password flow.
        /// </summary>
        [JsonPropertyName("password")]
        public required string Password { get; set; }

        /// <summary>
        /// One or more name-value pairs containing the validation data in the request to register a user. The validation data is set and then passed from the client in the request to register a user. You can pass this data to your Lambda function by using the ClientMetadata parameter in the InitiateAuth and AdminInitiateAuth API actions.
        /// </summary>
        [JsonPropertyName("validationData")]
        public Dictionary<string, string>? ValidationData { get; set; }

        /// <summary>
        /// One or more key-value pairs that you can provide as custom input to the Lambda function that you specify for the pre sign-up trigger. You can pass this data to your Lambda function by using the ClientMetadata parameter in the following API actions: AdminCreateUser, AdminRespondToAuthChallenge, ForgotPassword, and SignUp.
        /// </summary>
        [JsonPropertyName("clientMetadata")]
        public Dictionary<string, string>? ClientMetadata { get; set; }
    }
}
