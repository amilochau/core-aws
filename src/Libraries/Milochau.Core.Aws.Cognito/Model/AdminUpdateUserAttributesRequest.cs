﻿using Milochau.Core.Aws.Core.Runtime.Internal;
using System.Collections.Generic;

namespace Milochau.Core.Aws.Cognito.Model
{
    /// <summary>
    /// Container for the parameters to the AdminUpdateUserAttributes operation.
    /// <note> 
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
    /// <para>
    /// Updates the specified user's attributes, including developer attributes, as an administrator.
    /// Works on any user. To delete an attribute from your user, submit the attribute in
    /// your API request with a blank value.
    /// </para>
    ///  
    /// <para>
    /// For custom attributes, you must prepend the <code>custom:</code> prefix to the attribute
    /// name.
    /// </para>
    ///  
    /// <para>
    /// In addition to updating user attributes, this API can also be used to mark phone and
    /// email as verified.
    /// </para>
    ///  <note> 
    /// <para>
    /// Amazon Cognito evaluates Identity and Access Management (IAM) policies in requests
    /// for this API operation. For this operation, you must use IAM credentials to authorize
    /// requests, and you must grant yourself the corresponding IAM permission in a policy.
    /// </para>
    ///  
    /// <para>
    ///  <b>Learn more</b> 
    /// </para>
    ///  <ul> <li> 
    /// <para>
    ///  <a href="https://docs.aws.amazon.com/IAM/latest/UserGuide/reference_aws-signing.html">Signing
    /// Amazon Web Services API Requests</a> 
    /// </para>
    ///  </li> <li> 
    /// <para>
    ///  <a href="https://docs.aws.amazon.com/cognito/latest/developerguide/user-pools-API-operations.html">Using
    /// the Amazon Cognito user pools API and user pool endpoints</a> 
    /// </para>
    ///  </li> </ul> </note>
    /// </summary>
    public partial class AdminUpdateUserAttributesRequest : AmazonWebServiceRequest
    {
        /// <summary>
        /// Gets and sets the property ClientMetadata. 
        /// <para>
        /// A map of custom key-value pairs that you can provide as input for any custom workflows
        /// that this action triggers.
        /// </para>
        ///  
        /// <para>
        /// You create custom workflows by assigning Lambda functions to user pool triggers. When
        /// you use the AdminUpdateUserAttributes API action, Amazon Cognito invokes the function
        /// that is assigned to the <i>custom message</i> trigger. When Amazon Cognito invokes
        /// this function, it passes a JSON payload, which the function receives as input. This
        /// payload contains a <code>clientMetadata</code> attribute, which provides the data
        /// that you assigned to the ClientMetadata parameter in your AdminUpdateUserAttributes
        /// request. In your function code in Lambda, you can process the <code>clientMetadata</code>
        /// value to enhance your workflow for your specific needs.
        /// </para>
        ///  
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

        /// <summary>
        /// Gets and sets the property UserAttributes. 
        /// <para>
        /// An array of name-value pairs representing user attributes.
        /// </para>
        ///  
        /// <para>
        /// For custom attributes, you must prepend the <code>custom:</code> prefix to the attribute
        /// name.
        /// </para>
        ///  
        /// <para>
        /// If your user pool requires verification before Amazon Cognito updates an attribute
        /// value that you specify in this request, Amazon Cognito doesn’t immediately update
        /// the value of that attribute. After your user receives and responds to a verification
        /// message to verify the new value, Amazon Cognito updates the attribute value. Your
        /// user can sign in and receive messages with the original attribute value until they
        /// verify the new value.
        /// </para>
        ///  
        /// <para>
        /// To update the value of an attribute that requires verification in the same API request,
        /// include the <code>email_verified</code> or <code>phone_number_verified</code> attribute,
        /// with a value of <code>true</code>. If you set the <code>email_verified</code> or <code>phone_number_verified</code>
        /// value for an <code>email</code> or <code>phone_number</code> attribute that requires
        /// verification to <code>true</code>, Amazon Cognito doesn’t send a verification message
        /// to your user.
        /// </para>
        /// </summary>
        public required List<AttributeType> UserAttributes { get; set; }

        /// <summary>
        /// Gets and sets the property Username. 
        /// <para>
        /// The user name of the user for whom you want to update user attributes.
        /// </para>
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// Gets and sets the property UserPoolId. 
        /// <para>
        /// The user pool ID for the user pool where you want to update user attributes.
        /// </para>
        /// </summary>
        public required string UserPoolId { get; set; }
    }
}
