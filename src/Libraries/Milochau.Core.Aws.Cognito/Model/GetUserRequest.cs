using Milochau.Core.Aws.Core.Runtime.Internal;
using System;

namespace Milochau.Core.Aws.Cognito.Model
{
    /// <summary>
    /// Container for the parameters to the GetUser operation.
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
    public partial class GetUserRequest(Guid? userId) : AmazonWebServiceRequest(userId)
    {
        /// <summary>
        /// Gets and sets the property AccessToken. 
        /// <para>
        /// A non-expired access token for the user whose information you want to query.
        /// </para>
        /// </summary>
        public required string AccessToken { get; set; }
    }
}
