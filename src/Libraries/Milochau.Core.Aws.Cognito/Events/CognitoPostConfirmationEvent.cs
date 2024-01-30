namespace Milochau.Core.Aws.Cognito.Events
{
    /// <summary>
    /// https://docs.aws.amazon.com/cognito/latest/developerguide/user-pool-lambda-post-confirmation.html
    /// </summary>
    public class CognitoPostConfirmationEvent : CognitoTriggerEvent<CognitoPostConfirmationRequest, CognitoPostConfirmationResponse>
    {
    }
}
