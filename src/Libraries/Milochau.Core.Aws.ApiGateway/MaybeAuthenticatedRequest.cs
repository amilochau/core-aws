using Milochau.Core.Aws.Abstractions;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.ApiGateway
{
    /// <summary>Maybe authenticated request</summary>
    public abstract class MaybeAuthenticatedRequest(IdentityUser? user)
    {
        /// <summary>Authenticated user</summary>
        [JsonIgnore]
        public IdentityUser? User { get; set; } = user;
    }
}
