using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Cognito.Events
{
    /// <summary>
    /// https://docs.aws.amazon.com/cognito/latest/developerguide/user-pool-lambda-pre-sign-up.html
    /// </summary>
    public class CognitoPreSignupEvent : CognitoTriggerEvent<CognitoPreSignupRequest, CognitoPreSignupResponse>
    {
    }
}
