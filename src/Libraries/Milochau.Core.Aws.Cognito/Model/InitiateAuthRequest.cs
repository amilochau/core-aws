﻿using Milochau.Core.Aws.Core.Runtime.Internal;
using System;
using System.Collections.Generic;

namespace Milochau.Core.Aws.Cognito.Model
{
    /// <summary>
    /// Container for the parameters to the InitiateAuth operation.
    /// Initiates sign-in for a user in the Amazon Cognito user directory. You can't sign
    /// in a user with a federated IdP with <c>InitiateAuth</c>. For more information,
    /// see <a href="https://docs.aws.amazon.com/cognito/latest/developerguide/cognito-user-pools-identity-federation.html">
    /// Adding user pool sign-in through a third party</a>.
    /// 
    ///  <note> 
    /// <para>
    /// Amazon Cognito doesn't evaluate Identity and Access Management (IAM) policies in requests
    /// for this API operation. For this operation, you can't use IAM credentials to authorize
    /// requests, and you can't grant IAM permissions in policies. For more information about
    /// authorization models in Amazon Cognito, see <a href="https://docs.aws.amazon.com/cognito/latest/developerguide/user-pools-API-operations.html">Using
    /// the Amazon Cognito native and OIDC APIs</a>.
    /// </para>
    ///  </note> <note> 
    /// <para>
    /// This action might generate an SMS text message. Starting June 1, 2021, US telecom
    /// carriers require you to register an origination phone number before you can send SMS
    /// messages to US phone numbers. If you use SMS text messages in Amazon Cognito, you
    /// must register a phone number with <a href="https://console.aws.amazon.com/pinpoint/home/">Amazon
    /// Pinpoint</a>. Amazon Cognito uses the registered number automatically. Otherwise,
    /// Amazon Cognito users who must receive SMS messages might not be able to sign up, activate
    /// their accounts, or sign in.
    /// </para>
    ///  
    /// <para>
    /// If you have never used SMS text messages with Amazon Cognito or any other Amazon Web
    /// Service, Amazon Simple Notification Service might place your account in the SMS sandbox.
    /// In <i> <a href="https://docs.aws.amazon.com/sns/latest/dg/sns-sms-sandbox.html">sandbox
    /// mode</a> </i>, you can send messages only to verified phone numbers. After you test
    /// your app while in the sandbox environment, you can move out of the sandbox and into
    /// production. For more information, see <a href="https://docs.aws.amazon.com/cognito/latest/developerguide/user-pool-sms-settings.html">
    /// SMS message settings for Amazon Cognito user pools</a> in the <i>Amazon Cognito Developer
    /// Guide</i>.
    /// </para>
    ///  </note>
    /// </summary>
    public partial class InitiateAuthRequest(Guid? userId) : AmazonWebServiceRequest(userId)
    {
        ///// <summary>
        ///// Gets and sets the property AnalyticsMetadata. 
        ///// <para>
        ///// The Amazon Pinpoint analytics metadata that contributes to your metrics for <c>InitiateAuth</c>
        ///// calls.
        ///// </para>
        ///// </summary>
        //public AnalyticsMetadataType? AnalyticsMetadata { get; set; }

        /// <summary>
        /// Gets and sets the property AuthFlow. 
        /// <para>
        /// The authentication flow for this call to run. The API action will depend on this value.
        /// For example:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>REFRESH_TOKEN_AUTH</c> takes in a valid refresh token and returns new tokens.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>USER_SRP_AUTH</c> takes in <c>USERNAME</c> and <c>SRP_A</c>
        /// and returns the SRP variables to be used for next challenge execution.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>USER_PASSWORD_AUTH</c> takes in <c>USERNAME</c> and <c>PASSWORD</c>
        /// and returns the next challenge or tokens.
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// Valid values include:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>USER_SRP_AUTH</c>: Authentication flow for the Secure Remote Password (SRP)
        /// protocol.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>REFRESH_TOKEN_AUTH</c>/<c>REFRESH_TOKEN</c>: Authentication flow for
        /// refreshing the access token and ID token by supplying a valid refresh token.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>CUSTOM_AUTH</c>: Custom authentication flow.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>USER_PASSWORD_AUTH</c>: Non-SRP authentication flow; user name and password
        /// are passed directly. If a user migration Lambda trigger is set, this flow will invoke
        /// the user migration Lambda if it doesn't find the user name in the user pool. 
        /// </para>
        ///  </li> </ul> 
        /// <para>
        ///  <c>ADMIN_NO_SRP_AUTH</c> isn't a valid value.
        /// </para>
        /// </summary>
        public required AuthFlowType AuthFlow { get; set; }

        /// <summary>
        /// Gets and sets the property AuthParameters. 
        /// <para>
        /// The authentication parameters. These are inputs corresponding to the <c>AuthFlow</c>
        /// that you're invoking. The required values depend on the value of <c>AuthFlow</c>:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// For <c>USER_SRP_AUTH</c>: <c>USERNAME</c> (required), <c>SRP_A</c>
        /// (required), <c>SECRET_HASH</c> (required if the app client is configured with
        /// a client secret), <c>DEVICE_KEY</c>.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// For <c>USER_PASSWORD_AUTH</c>: <c>USERNAME</c> (required), <c>PASSWORD</c>
        /// (required), <c>SECRET_HASH</c> (required if the app client is configured with
        /// a client secret), <c>DEVICE_KEY</c>.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// For <c>REFRESH_TOKEN_AUTH/REFRESH_TOKEN</c>: <c>REFRESH_TOKEN</c> (required),
        /// <c>SECRET_HASH</c> (required if the app client is configured with a client secret),
        /// <c>DEVICE_KEY</c>.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// For <c>CUSTOM_AUTH</c>: <c>USERNAME</c> (required), <c>SECRET_HASH</c>
        /// (if app client is configured with client secret), <c>DEVICE_KEY</c>. To start
        /// the authentication flow with password verification, include <c>ChallengeName: SRP_A</c>
        /// and <c>SRP_A: (The SRP_A Value)</c>.
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// For more information about <c>SECRET_HASH</c>, see <a href="https://docs.aws.amazon.com/cognito/latest/developerguide/signing-up-users-in-your-app.html#cognito-user-pools-computing-secret-hash">Computing
        /// secret hash values</a>. For information about <c>DEVICE_KEY</c>, see <a href="https://docs.aws.amazon.com/cognito/latest/developerguide/amazon-cognito-user-pools-device-tracking.html">Working
        /// with user devices in your user pool</a>.
        /// </para>
        /// </summary>
        public Dictionary<string, string>? AuthParameters { get; set; }

        /// <summary>
        /// Gets and sets the property ClientId. 
        /// <para>
        /// The app client ID.
        /// </para>
        /// </summary>
        public required string ClientId { get; set; }

        /// <summary>
        /// Gets and sets the property ClientMetadata. 
        /// <para>
        /// A map of custom key-value pairs that you can provide as input for certain custom workflows
        /// that this action triggers.
        /// </para>
        ///  
        /// <para>
        /// You create custom workflows by assigning Lambda functions to user pool triggers. When
        /// you use the InitiateAuth API action, Amazon Cognito invokes the Lambda functions that
        /// are specified for various triggers. The ClientMetadata value is passed as input to
        /// the functions for only the following triggers:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// Pre signup
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Pre authentication
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// User migration
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// When Amazon Cognito invokes the functions for these triggers, it passes a JSON payload,
        /// which the function receives as input. This payload contains a <c>validationData</c>
        /// attribute, which provides the data that you assigned to the ClientMetadata parameter
        /// in your InitiateAuth request. In your function code in Lambda, you can process the
        /// <c>validationData</c> value to enhance your workflow for your specific needs.
        /// </para>
        ///  
        /// <para>
        /// When you use the InitiateAuth API action, Amazon Cognito also invokes the functions
        /// for the following triggers, but it doesn't provide the ClientMetadata value as input:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// Post authentication
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Custom message
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Pre token generation
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Create auth challenge
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Define auth challenge
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Verify auth challenge
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// For more information, see <a href="https://docs.aws.amazon.com/cognito/latest/developerguide/cognito-user-identity-pools-working-with-aws-lambda-triggers.html">
        /// Customizing user pool Workflows with Lambda Triggers</a> in the <i>Amazon Cognito
        /// Developer Guide</i>.
        /// </para>
        ///  <note> 
        /// <para>
        /// When you use the ClientMetadata parameter, remember that Amazon Cognito won't do the
        /// following:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// Store the ClientMetadata value. This data is available only to Lambda triggers that
        /// are assigned to a user pool to support custom workflows. If your user pool configuration
        /// doesn't include triggers, the ClientMetadata parameter serves no purpose.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Validate the ClientMetadata value.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Encrypt the ClientMetadata value. Don't use Amazon Cognito to provide sensitive information.
        /// </para>
        ///  </li> </ul> </note>
        /// </summary>
        public Dictionary<string, string>? ClientMetadata { get; set; }

        ///// <summary>
        ///// Gets and sets the property UserContextData. 
        ///// <para>
        ///// Contextual data about your user session, such as the device fingerprint, IP address,
        ///// or location. Amazon Cognito advanced security evaluates the risk of an authentication
        ///// event based on the context that your app generates and passes to Amazon Cognito when
        ///// it makes API requests.
        ///// </para>
        ///// </summary>
        //public UserContextDataType? UserContextData { get; set; }
    }
}
