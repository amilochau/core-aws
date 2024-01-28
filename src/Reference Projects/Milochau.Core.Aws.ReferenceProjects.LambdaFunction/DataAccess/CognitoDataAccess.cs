using Milochau.Core.Aws.Cognito;
using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.DynamoDB.Model;
using Milochau.Core.Aws.DynamoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Milochau.Core.Aws.Cognito.Model;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction.DataAccess
{
    public interface ICognitoDataAccess
    {
        Task LoginAsync(CancellationToken cancellationToken);
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
                AccessToken = response.AuthenticationResult.AccessToken
            }, cancellationToken);
        }
    }
}
