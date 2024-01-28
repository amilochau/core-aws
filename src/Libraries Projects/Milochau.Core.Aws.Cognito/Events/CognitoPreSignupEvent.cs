namespace Milochau.Core.Aws.Cognito.Events
{
    /// <summary>
    /// https://docs.aws.amazon.com/cognito/latest/developerguide/user-pool-lambda-pre-sign-up.html
    /// </summary>
    public class CognitoPreSignupEvent : CognitoTriggerEvent<CognitoPreSignupRequest, CognitoPreSignupResponse>
    {
    }
}
