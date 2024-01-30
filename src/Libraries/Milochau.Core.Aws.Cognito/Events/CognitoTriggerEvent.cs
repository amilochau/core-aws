using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Cognito.Events
{
    /// <summary>
    /// AWS Cognito Trigger Common Parameters
    /// https://docs.aws.amazon.com/cognito/latest/developerguide/cognito-user-identity-pools-working-with-aws-lambda-triggers.html#cognito-user-pools-lambda-trigger-syntax-shared
    /// </summary>
    public abstract class CognitoTriggerEvent<TRequest, TResponse>
            where TRequest : CognitoTriggerRequest
            where TResponse : CognitoTriggerResponse
    {
        /// <summary>
        /// The version number of your Lambda function.
        /// </summary>
        [JsonPropertyName("version")]
        public required string Version { get; set; }

        /// <summary>
        /// The AWS Region, as an AWSRegion instance.
        /// </summary>
        [JsonPropertyName("region")]
        public required string Region { get; set; }

        /// <summary>
        /// The user pool ID for the user pool.
        /// </summary>
        [JsonPropertyName("userPoolId")]
        public required string UserPoolId { get; set; }

        /// <summary>
        /// The username of the current user.
        /// </summary>
        [JsonPropertyName("userName")]
        public required string UserName { get; set; }

        /// <summary>
        /// The caller context
        /// </summary>
        [JsonPropertyName("callerContext")]
        public required CognitoTriggerCallerContext CallerContext { get; set; }

        /// <summary>
        /// The name of the event that triggered the Lambda function.For a description of each triggerSource see User pool Lambda trigger sources.
        /// </summary>
        [JsonPropertyName("triggerSource")]
        public required string TriggerSource { get; set; }

        /// <summary>
        /// The request from the Amazon Cognito service
        /// </summary>
        [JsonPropertyName("request")]
        public TRequest? Request { get; set; }

        /// <summary>
        /// The response from your Lambda trigger.The return parameters in the response depend on the triggering event.
        /// </summary>
        [JsonPropertyName("response")]
        public TResponse? Response { get; set; }
    }
}
