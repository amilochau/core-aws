using System.Text.Json;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals.Context
{
    public class CognitoIdentity : ICognitoIdentity
    {
        public string? IdentityId { get; set; }

        public string? IdentityPoolId { get; set; }

        internal static CognitoIdentity FromJson(string? json)
        {
            var result = new CognitoIdentity();

            if (!string.IsNullOrWhiteSpace(json))
            {
                var jsonData = JsonDocument.Parse(json).RootElement;

                if (jsonData.TryGetProperty("cognitoIdentityId", out var cognitoIdentityId))
                    result.IdentityId = cognitoIdentityId.GetString();
                if (jsonData.TryGetProperty("cognitoIdentityPoolId", out var cognitoIdentityPoolId))
                    result.IdentityPoolId = cognitoIdentityPoolId.GetString();
            }

            return result;
        }
    }
}
