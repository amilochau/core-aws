using Milochau.Core.Aws.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Http
{
    public static class HttpContextExtensions
    {
        public static IdentityUser? CreateIdentityUser(this HttpContext context)
        {
            var claims = context.User.Claims;

            return !(TryGetJwtClaim(claims, ClaimTypes.NameIdentifier, out var userSub) || TryGetJwtClaim(claims, "sub", out userSub))
                || !(TryGetJwtClaim(claims, ClaimTypes.Email, out var userEmail) || TryGetJwtClaim(claims, "email", out userEmail))
                || !TryGetJwtClaim(claims, "name", out var userName)
                || !TryGetJwtClaim(claims, "custom:user_id", out var rawUserId) || !Guid.TryParse(rawUserId, out var userId)
                ? null
                : new IdentityUser(userSub, userName, userEmail, userId);
        }

        private static bool TryGetJwtClaim(IEnumerable<Claim> claims, string type, [NotNullWhen(true)] out string? value)
        {
            value = null;
            var claim = claims.FirstOrDefault(x => x.Type == type);
            if (claim == null)
            {
                return false;
            }
            else
            {
                value = claim.Value;
                return true;
            }
        }
    }
}
