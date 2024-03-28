using Milochau.Core.Aws.Core.Runtime.Internal;
using System.Collections.Generic;

namespace Milochau.Core.Aws.Cognito.Model
{
    /// <summary>
    /// Initiates the authentication response.
    /// </summary>
    public partial class InitiateAuthResponse : AmazonWebServiceResponse
    {
        /// <summary>
        /// Gets and sets the property AuthenticationResult. 
        /// <para>
        /// The result of the authentication response. This result is only returned if the caller
        /// doesn't need to pass another challenge. If the caller does need to pass another challenge
        /// before it gets tokens, <c>ChallengeName</c>, <c>ChallengeParameters</c>,
        /// and <c>Session</c> are returned.
        /// </para>
        /// </summary>
        public AuthenticationResultType? AuthenticationResult { get; set; }

        /// <summary>
        /// Gets and sets the property ChallengeName. 
        /// <para>
        /// The name of the challenge that you're responding to with this call. This name is returned
        /// in the <c>AdminInitiateAuth</c> response if you must pass another challenge.
        /// </para>
        ///  
        /// <para>
        /// Valid values include the following:
        /// </para>
        ///  <note> 
        /// <para>
        /// All of the following challenges require <c>USERNAME</c> and <c>SECRET_HASH</c>
        /// (if applicable) in the parameters.
        /// </para>
        ///  </note> <ul> <li> 
        /// <para>
        ///  <c>SMS_MFA</c>: Next challenge is to supply an <c>SMS_MFA_CODE</c>, delivered
        /// via SMS.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>PASSWORD_VERIFIER</c>: Next challenge is to supply <c>PASSWORD_CLAIM_SIGNATURE</c>,
        /// <c>PASSWORD_CLAIM_SECRET_BLOCK</c>, and <c>TIMESTAMP</c> after the client-side
        /// SRP calculations.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>CUSTOM_CHALLENGE</c>: This is returned if your custom authentication flow
        /// determines that the user should pass another challenge before tokens are issued.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>DEVICE_SRP_AUTH</c>: If device tracking was activated on your user pool
        /// and the previous challenges were passed, this challenge is returned so that Amazon
        /// Cognito can start tracking this device.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>DEVICE_PASSWORD_VERIFIER</c>: Similar to <c>PASSWORD_VERIFIER</c>,
        /// but for devices only.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>NEW_PASSWORD_REQUIRED</c>: For users who are required to change their passwords
        /// after successful first login. 
        /// </para>
        ///  
        /// <para>
        /// Respond to this challenge with <c>NEW_PASSWORD</c> and any required attributes
        /// that Amazon Cognito returned in the <c>requiredAttributes</c> parameter. You
        /// can also set values for attributes that aren't required by your user pool and that
        /// your app client can write. For more information, see <a href="https://docs.aws.amazon.com/cognito-user-identity-pools/latest/APIReference/API_RespondToAuthChallenge.html">RespondToAuthChallenge</a>.
        /// </para>
        ///  <note> 
        /// <para>
        /// In a <c>NEW_PASSWORD_REQUIRED</c> challenge response, you can't modify a required
        /// attribute that already has a value. In <c>RespondToAuthChallenge</c>, set a
        /// value for any keys that Amazon Cognito returned in the <c>requiredAttributes</c>
        /// parameter, then use the <c>UpdateUserAttributes</c> API operation to modify
        /// the value of any additional attributes.
        /// </para>
        ///  </note> </li> <li> 
        /// <para>
        ///  <c>MFA_SETUP</c>: For users who are required to setup an MFA factor before
        /// they can sign in. The MFA types activated for the user pool will be listed in the
        /// challenge parameters <c>MFA_CAN_SETUP</c> value. 
        /// </para>
        ///  
        /// <para>
        ///  To set up software token MFA, use the session returned here from <c>InitiateAuth</c>
        /// as an input to <c>AssociateSoftwareToken</c>. Use the session returned by <c>VerifySoftwareToken</c>
        /// as an input to <c>RespondToAuthChallenge</c> with challenge name <c>MFA_SETUP</c>
        /// to complete sign-in. To set up SMS MFA, an administrator should help the user to add
        /// a phone number to their account, and then the user should call <c>InitiateAuth</c>
        /// again to restart sign-in.
        /// </para>
        ///  </li> </ul>
        /// </summary>
        public ChallengeNameType? ChallengeName { get; set; }

        /// <summary>
        /// Gets and sets the property ChallengeParameters. 
        /// <para>
        /// The challenge parameters. These are returned in the <c>InitiateAuth</c> response
        /// if you must pass another challenge. The responses in this parameter should be used
        /// to compute inputs to the next call (<c>RespondToAuthChallenge</c>). 
        /// </para>
        ///  
        /// <para>
        /// All challenges require <c>USERNAME</c> and <c>SECRET_HASH</c> (if applicable).
        /// </para>
        /// </summary>
        public Dictionary<string, string>? ChallengeParameters { get; set; }

        /// <summary>
        /// Gets and sets the property Session. 
        /// <para>
        /// The session that should pass both ways in challenge-response calls to the service.
        /// If the caller must pass another challenge, they return a session with other challenge
        /// parameters. This session should be passed as it is to the next <c>RespondToAuthChallenge</c>
        /// API call.
        /// </para>
        /// </summary>
        public string? Session { get; set; }
    }
}
