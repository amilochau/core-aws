using Milochau.Core.Aws.Abstractions;
using Milochau.Core.Aws.Core.Lambda.Events;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.ApiGateway
{
    /// <summary>Maybe authenticated request</summary>
    public abstract class MaybeAuthenticatedRequest
    {
        /// <summary>Authenticated user</summary>
        [JsonIgnore]
        public IdentityUser? User { get; set; } = null!;

        /// <summary>Parse request to get authentication data</summary>
        /// <remarks>
        /// Use static abstract member in the future.
        /// See https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-11.0/static-abstracts-in-interfaces
        /// </remarks>
        protected bool ParseAuthentication(APIGatewayHttpApiV2ProxyRequest request)
        {
            if (request.TryGetJwtClaims("sub", out var userId)
                && request.TryGetJwtClaims("email", out var userEmail)
                && request.TryGetJwtClaims("name", out var userName))
            {
                User = new IdentityUser
                {
                    Id = userId,
                    Email = userEmail,
                    Name = userName,
                };
            }

            return true;
        }
    }
}
