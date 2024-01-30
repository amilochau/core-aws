﻿namespace Milochau.Core.Aws.Cognito.Events
{
    /// <summary>
    /// https://docs.aws.amazon.com/cognito/latest/developerguide/user-pool-lambda-migrate-user.html
    /// </summary>
    public class CognitoMigrateUserEvent : CognitoTriggerEvent<CognitoMigrateUserRequest, CognitoMigrateUserResponse>
    {
    }
}
