using System;

namespace Milochau.Core.Aws.Abstractions
{
    /// <summary>Identity user, as fetched from the authentication</summary>
    public class IdentityUser(string sub, string name, string email, Guid userId)
    {
        /// <summary>User sub</summary>
        /// <remarks>This attribute can change in the future, when migrating the user store. Prefer using the <see cref="UserId"/> attribute</remarks>
        public string Sub { get; } = sub;

        /// <summary>User name</summary>
        public string Name { get; } = name;

        /// <summary>User email</summary>
        public string Email { get; } = email;

        /// <summary>User id</summary>
        public Guid UserId { get; } = userId;
    }
}
