using Milochau.Core.Aws.Cognito;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Milochau.Core.Aws.Cognito.Model;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction.DataAccess
{
    public interface ICognitoDataAccess
    {
        Task LoginAsync(CancellationToken cancellationToken);
        Task UpdateAttributesAsync(CancellationToken cancellationToken);
    }

    public class CognitoDataAccess : ICognitoDataAccess
    {
        private readonly IAmazonCognitoIdentityProvider amazonCognitoIdentityProvider;

        public CognitoDataAccess(IAmazonCognitoIdentityProvider amazonCognitoIdentityProvider)
        {
            this.amazonCognitoIdentityProvider = amazonCognitoIdentityProvider;
        }

        public async Task LoginAsync(CancellationToken cancellationToken)
        {
            var response = await amazonCognitoIdentityProvider.InitiateAuthAsync(new InitiateAuthRequest
            {
                UserId = null,
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", "aaa@outlook.com" },
                    { "PASSWORD", "aaa" },
                },
                ClientId = "aaa",
            }, cancellationToken);

            if (string.IsNullOrEmpty(response.AuthenticationResult?.AccessToken))
            {
                return;
            }

            var user = await amazonCognitoIdentityProvider.GetUserAsync(new GetUserRequest
            {
                UserId = null,
                AccessToken = response.AuthenticationResult.AccessToken
            }, cancellationToken);
        }

        public async Task UpdateAttributesAsync(CancellationToken cancellationToken)
        {
            var response = await amazonCognitoIdentityProvider.AdminUpdateUserAttributesAsync(new AdminUpdateUserAttributesRequest
            {
                UserId = null,
                Username = "aaa@outlook.fr",
                UserPoolId = "eu-west-3_Trx7Zxn8M",
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType { Name = "custom:user_id", Value = "XXX" },
                },
            }, cancellationToken);
        }
    }
}
