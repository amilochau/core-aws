﻿using Milochau.Core.Aws.Core.Runtime.Internal;
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
        /// before it gets tokens, <code>ChallengeName</code>, <code>ChallengeParameters</code>,
        /// and <code>Session</code> are returned.
        /// </para>
        /// </summary>
        public AuthenticationResultType? AuthenticationResult { get; set; }

        /// <summary>
        /// Gets and sets the property ChallengeName. 
        /// <para>
        /// The name of the challenge that you're responding to with this call. This name is returned
        /// in the <code>AdminInitiateAuth</code> response if you must pass another challenge.
        /// </para>
        ///  
        /// <para>
        /// Valid values include the following:
        /// </para>
        ///  <note> 
        /// <para>
        /// All of the following challenges require <code>USERNAME</code> and <code>SECRET_HASH</code>
        /// (if applicable) in the parameters.
        /// </para>
        ///  </note> <ul> <li> 
        /// <para>
        ///  <code>SMS_MFA</code>: Next challenge is to supply an <code>SMS_MFA_CODE</code>, delivered
        /// via SMS.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>PASSWORD_VERIFIER</code>: Next challenge is to supply <code>PASSWORD_CLAIM_SIGNATURE</code>,
        /// <code>PASSWORD_CLAIM_SECRET_BLOCK</code>, and <code>TIMESTAMP</code> after the client-side
        /// SRP calculations.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>CUSTOM_CHALLENGE</code>: This is returned if your custom authentication flow
        /// determines that the user should pass another challenge before tokens are issued.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>DEVICE_SRP_AUTH</code>: If device tracking was activated on your user pool
        /// and the previous challenges were passed, this challenge is returned so that Amazon
        /// Cognito can start tracking this device.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>DEVICE_PASSWORD_VERIFIER</code>: Similar to <code>PASSWORD_VERIFIER</code>,
        /// but for devices only.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>NEW_PASSWORD_REQUIRED</code>: For users who are required to change their passwords
        /// after successful first login. 
        /// </para>
        ///  
        /// <para>
        /// Respond to this challenge with <code>NEW_PASSWORD</code> and any required attributes
        /// that Amazon Cognito returned in the <code>requiredAttributes</code> parameter. You
        /// can also set values for attributes that aren't required by your user pool and that
        /// your app client can write. For more information, see <a href="https://docs.aws.amazon.com/cognito-user-identity-pools/latest/APIReference/API_RespondToAuthChallenge.html">RespondToAuthChallenge</a>.
        /// </para>
        ///  <note> 
        /// <para>
        /// In a <code>NEW_PASSWORD_REQUIRED</code> challenge response, you can't modify a required
        /// attribute that already has a value. In <code>RespondToAuthChallenge</code>, set a
        /// value for any keys that Amazon Cognito returned in the <code>requiredAttributes</code>
        /// parameter, then use the <code>UpdateUserAttributes</code> API operation to modify
        /// the value of any additional attributes.
        /// </para>
        ///  </note> </li> <li> 
        /// <para>
        ///  <code>MFA_SETUP</code>: For users who are required to setup an MFA factor before
        /// they can sign in. The MFA types activated for the user pool will be listed in the
        /// challenge parameters <code>MFA_CAN_SETUP</code> value. 
        /// </para>
        ///  
        /// <para>
        ///  To set up software token MFA, use the session returned here from <code>InitiateAuth</code>
        /// as an input to <code>AssociateSoftwareToken</code>. Use the session returned by <code>VerifySoftwareToken</code>
        /// as an input to <code>RespondToAuthChallenge</code> with challenge name <code>MFA_SETUP</code>
        /// to complete sign-in. To set up SMS MFA, an administrator should help the user to add
        /// a phone number to their account, and then the user should call <code>InitiateAuth</code>
        /// again to restart sign-in.
        /// </para>
        ///  </li> </ul>
        /// </summary>
        public ChallengeNameType? ChallengeName { get; set; }

        /// <summary>
        /// Gets and sets the property ChallengeParameters. 
        /// <para>
        /// The challenge parameters. These are returned in the <code>InitiateAuth</code> response
        /// if you must pass another challenge. The responses in this parameter should be used
        /// to compute inputs to the next call (<code>RespondToAuthChallenge</code>). 
        /// </para>
        ///  
        /// <para>
        /// All challenges require <code>USERNAME</code> and <code>SECRET_HASH</code> (if applicable).
        /// </para>
        /// </summary>
        public Dictionary<string, string>? ChallengeParameters { get; set; }

        /// <summary>
        /// Gets and sets the property Session. 
        /// <para>
        /// The session that should pass both ways in challenge-response calls to the service.
        /// If the caller must pass another challenge, they return a session with other challenge
        /// parameters. This session should be passed as it is to the next <code>RespondToAuthChallenge</code>
        /// API call.
        /// </para>
        /// </summary>
        public string? Session { get; set; }
    }
}
