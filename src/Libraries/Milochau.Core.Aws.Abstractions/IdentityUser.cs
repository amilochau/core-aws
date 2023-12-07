namespace Milochau.Core.Aws.Abstractions
{
    /// <summary>Identity user, as fetched from the authentication</summary>
    public class IdentityUser
    {
        /// <summary>User id</summary>
        public required string Id { get; set; }

        /// <summary>User name</summary>
        public required string Name { get; set; }

        /// <summary>User email</summary>
        public required string Email { get; set; }
    }
}
