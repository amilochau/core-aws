using Milochau.Core.Aws.Cognito.Model;
using Milochau.Core.Aws.Cognito.Model.Internal.MarshallTransformations;
using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Runtime.Credentials;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Cognito
{
    /// <summary>
    /// Implementation for accessing CognitoIdentityProvider
    ///
    /// With the Amazon Cognito user pools API, you can set up user pools and app clients,
    /// and authenticate users. To authenticate users from third-party identity providers
    /// (IdPs) in this API, you can <a href="https://docs.aws.amazon.com/cognito/latest/developerguide/cognito-user-pools-identity-federation-consolidate-users.html">link
    /// IdP users to native user profiles</a>. Learn more about the authentication and authorization
    /// of federated users in the <a href="https://docs.aws.amazon.com/cognito/latest/developerguide/cognito-userpools-server-contract-reference.html">Using
    /// the Amazon Cognito user pools API and user pool endpoints</a>.
    /// 
    ///  
    /// <para>
    /// This API reference provides detailed information about API operations and object types
    /// in Amazon Cognito. At the bottom of the page for each API operation and object, under
    /// <i>See Also</i>, you can learn how to use it in an Amazon Web Services SDK in the
    /// language of your choice.
    /// </para>
    ///  
    /// <para>
    /// Along with resource management operations, the Amazon Cognito user pools API includes
    /// classes of operations and authorization models for client-side and server-side user
    /// operations. For more information, see <a href="https://docs.aws.amazon.com/cognito/latest/developerguide/user-pools-API-operations.html">Using
    /// the Amazon Cognito native and OIDC APIs</a> in the <i>Amazon Cognito Developer Guide</i>.
    /// </para>
    ///  
    /// <para>
    /// You can also start reading about the <c>CognitoIdentityProvider</c> client in
    /// the following SDK guides.
    /// </para>
    ///  <ul> <li> 
    /// <para>
    ///  <a href="https://docs.aws.amazon.com/cli/latest/reference/cognito-idp/index.html#cli-aws-cognito-idp">Amazon
    /// Web Services Command Line Interface</a> 
    /// </para>
    ///  </li> <li> 
    /// <para>
    ///  <a href="https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/CognitoIdentityProvider/TCognitoIdentityProviderClient.html">Amazon
    /// Web Services SDK for .NET</a> 
    /// </para>
    ///  </li> </ul> 
    /// <para>
    /// To get started with an Amazon Web Services SDK, see <a href="http://aws.amazon.com/developer/tools/">Tools
    /// to Build on Amazon Web Services</a>. For example actions and scenarios, see <a href="https://docs.aws.amazon.com/cognito/latest/developerguide/service_code_examples_cognito-identity-provider.html">Code
    /// examples for Amazon Cognito Identity Provider using Amazon Web Services SDKs</a>.
    /// </para>
    /// </summary>
    public partial class AmazonCognitoIdentityProviderClient : AmazonServiceClient, IAmazonCognitoIdentityProvider
    {
        #region Constructors

        /// <summary>
        /// Constructs AmazonCognitoIdentityProviderClient
        /// </summary>
        public AmazonCognitoIdentityProviderClient(IAWSCredentials credentials)
            : base(credentials, new ClientConfig
            {
                AuthenticationServiceName = "cognito-idp",
                MonitoringServiceName = "cognito-idp"
            })
        { }

        #endregion

        #region  AdminUpdateUserAttributes

        /// <summary>
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
        /// For custom attributes, you must prepend the <c>custom:</c> prefix to the attribute
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
        /// <param name="request">Container for the necessary parameters to execute the AdminUpdateUserAttributes service method.</param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// 
        /// <returns>The response from the AdminUpdateUserAttributes service method, as returned by CognitoIdentityProvider.</returns>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/cognito-idp-2016-04-18/AdminUpdateUserAttributes">REST API Reference for AdminUpdateUserAttributes Operation</seealso>
        public virtual Task<AdminUpdateUserAttributesResponse> AdminUpdateUserAttributesAsync(AdminUpdateUserAttributesRequest request, CancellationToken cancellationToken) => InvokeAsync(request, new AdminUpdateUserAttributesInvokeOptions(), cancellationToken);

        #endregion
        #region  GetUser

        /// <summary>
        /// Gets the user attributes and metadata for a user.
        /// 
        ///  <note> 
        /// <para>
        /// Amazon Cognito doesn't evaluate Identity and Access Management (IAM) policies in requests
        /// for this API operation. For this operation, you can't use IAM credentials to authorize
        /// requests, and you can't grant IAM permissions in policies. For more information about
        /// authorization models in Amazon Cognito, see <a href="https://docs.aws.amazon.com/cognito/latest/developerguide/user-pools-API-operations.html">Using
        /// the Amazon Cognito native and OIDC APIs</a>.
        /// </para>
        ///  </note>
        /// </summary>
        /// <param name="request">Container for the necessary parameters to execute the GetUser service method.</param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// 
        /// <returns>The response from the GetUser service method, as returned by CognitoIdentityProvider.</returns>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/cognito-idp-2016-04-18/GetUser">REST API Reference for GetUser Operation</seealso>
        public virtual Task<GetUserResponse> GetUserAsync(GetUserRequest request, CancellationToken cancellationToken) => InvokeAsync(request, new GetUserInvokeOptions(), cancellationToken);

        #endregion
        #region InitiateAuth

        /// <summary>
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
        /// <param name="request">Container for the necessary parameters to execute the InitiateAuth service method.</param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/cognito-idp-2016-04-18/InitiateAuth">REST API Reference for InitiateAuth Operation</seealso>
        public virtual Task<InitiateAuthResponse> InitiateAuthAsync(InitiateAuthRequest request, CancellationToken cancellationToken) => InvokeAsync(request, new InitiateAuthInvokeOptions(), cancellationToken);

        #endregion
    }
}
