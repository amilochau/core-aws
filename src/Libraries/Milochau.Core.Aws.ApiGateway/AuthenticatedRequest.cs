using Milochau.Core.Aws.Abstractions;
using Milochau.Core.Aws.Core.Lambda.Events;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.ApiGateway
{
    /// <summary>Authenticated request</summary>
    public abstract class AuthenticatedRequest
    {
        /// <summary>Authenticated user</summary>
        [JsonIgnore]
        public required IdentityUser User { get; set; }

        /// <summary>Parse request to get authentication data</summary>
        /// <remarks>
        /// Use static abstract member in the future.
        /// See https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-11.0/static-abstracts-in-interfaces
        /// </remarks>
        protected bool ParseAuthentication(APIGatewayHttpApiV2ProxyRequest request)
        {
            if (!request.TryGetJwtClaims("sub", out var userSub))
            {
                return false;
            }
            if (!request.TryGetJwtClaims("email", out var userEmail))
            {
                return false;
            }
            if (!request.TryGetJwtClaims("name", out var userName))
            {
                return false;
            }
            if (!request.TryGetJwtClaims("custom:user_id", out var userId))
            {
                return false;
            }

            User = new IdentityUser
            {
                Sub = userSub,
                Email = userEmail,
                Name = userName,
                UserId = userId,
            };
            return true;
        }
    }
}
