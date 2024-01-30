using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Cognito.Events
{
    /// <summary>
    /// https://docs.aws.amazon.com/cognito/latest/developerguide/user-pool-lambda-migrate-user.html
    /// </summary>
    public class CognitoMigrateUserResponse : CognitoTriggerResponse
    {
        /// <summary>
        /// It must contain one or more name-value pairs representing user attributes to be stored in the user profile in your user pool. You can include both standard and custom user attributes. Custom attributes require the custom: prefix to distinguish them from standard attributes.
        /// </summary>
        [JsonPropertyName("userAttributes")]
        public Dictionary<string, string>? UserAttributes { get; set; }

        /// <summary>
        /// During sign-in, this attribute can be set to CONFIRMED, or not set, to auto-confirm your users and allow them to sign-in with their previous passwords. This is the simplest experience for the user.
        /// </summary>
        [JsonPropertyName("finalUserStatus")]
        public string? FinalUserStatus { get; set; }

        /// <summary>
        /// This attribute can be set to "SUPPRESS" to suppress the welcome message usually sent by Amazon Cognito to new users. If this attribute is not returned, the welcome message will be sent.
        /// </summary>
        [JsonPropertyName("messageAction")]
        public string? MessageAction { get; set; }

        /// <summary>
        /// This attribute can be set to "EMAIL" to send the welcome message by email, or "SMS" to send the welcome message by SMS. If this attribute is not returned, the welcome message will be sent by SMS.
        /// </summary>
        [JsonPropertyName("desiredDeliveryMediums")]
        public List<string>? DesiredDeliveryMediums { get; set; }

        /// <summary>
        /// If this parameter is set to "true" and the phone number or email address specified in the UserAttributes parameter already exists as an alias with a different user, the API call will migrate the alias from the previous user to the newly created user. The previous user will no longer be able to log in using that alias.
        /// </summary>
        [JsonPropertyName("forceAliasCreation")]
        public bool? ForceAliasCreation { get; set; }
    }
}
