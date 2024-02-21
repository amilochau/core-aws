using System;

namespace Milochau.Core.Aws.Abstractions
{
    /// <summary>Identity user, as fetched from the authentication</summary>
    public class IdentityUser
    {
        /// <summary>User sub</summary>
        /// <remarks>This attribute can change in the future, when migrating the user store. Prefer using the <see cref="UserId"/> attribute</remarks>
        public required string Sub { get; set; }

        /// <summary>User name</summary>
        public required string Name { get; set; }

        /// <summary>User email</summary>
        public required string Email { get; set; }

        /// <summary>User id</summary>
        public required Guid UserId { get; set; }
    }
}
