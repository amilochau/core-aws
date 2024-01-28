using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Cognito.Events
{
    /// <summary>
    /// AWS Cognito Trigger Common Parameters
    /// https://docs.aws.amazon.com/cognito/latest/developerguide/cognito-user-identity-pools-working-with-aws-lambda-triggers.html#cognito-user-pools-lambda-trigger-syntax-shared
    /// </summary>
    public abstract class CognitoTriggerEvent<TRequest, TResponse>
            where TRequest : CognitoTriggerRequest, new()
            where TResponse : CognitoTriggerResponse, new()
    {
        /// <summary>
        /// The version number of your Lambda function.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; } = null!; // @todo Change that with .NET 8

        /// <summary>
        /// The AWS Region, as an AWSRegion instance.
        /// </summary>
        [JsonPropertyName("region")]
        public string Region { get; set; } = null!;

        /// <summary>
        /// The user pool ID for the user pool.
        /// </summary>
        [JsonPropertyName("userPoolId")]
        public string UserPoolId { get; set; } = null!;

        /// <summary>
        /// The username of the current user.
        /// </summary>
        [JsonPropertyName("userName")]
        public string UserName { get; set; } = null!;

        /// <summary>
        /// The caller context
        /// </summary>
        [JsonPropertyName("callerContext")]
        public CognitoTriggerCallerContext CallerContext { get; set; } = new CognitoTriggerCallerContext();

        /// <summary>
        /// The name of the event that triggered the Lambda function.For a description of each triggerSource see User pool Lambda trigger sources.
        /// </summary>
        [JsonPropertyName("triggerSource")]
        public string TriggerSource { get; set; } = null!;

        /// <summary>
        /// The request from the Amazon Cognito service
        /// </summary>
        [JsonPropertyName("request")]
        public TRequest Request { get; set; } = new TRequest();

        /// <summary>
        /// The response from your Lambda trigger.The return parameters in the response depend on the triggering event.
        /// </summary>
        [JsonPropertyName("response")]
        public TResponse Response { get; set; } = new TResponse();
    }
}
