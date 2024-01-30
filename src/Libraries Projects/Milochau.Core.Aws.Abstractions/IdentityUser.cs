namespace Milochau.Core.Aws.Abstractions
{
    /// <summary>Identity user, as fetched from the authentication</summary>
    public class IdentityUser
    {
        /// <summary>User sub</summary>
        /// <remarks>This attribute can change in the future, when migrating the user store. Prefer using the <see cref="UserId"/> attribute</remarks>
        public string Sub { get; set; } = null!;

        /// <summary>User name</summary>
        public string Name { get; set; } = null!;

        /// <summary>User email</summary>
        public string Email { get; set; } = null!;

        /// <summary>User id</summary>
        public string UserId { get; set; } = null!;
    }
}
