namespace Milochau.Core.Aws.Abstractions
{
    /// <summary>Identity user, as fetched from the authentication</summary>
    public class IdentityUser
    {
        /// <summary>User id</summary>
        public string Id { get; set; } = null!;

        /// <summary>User name</summary>
        public string Name { get; set; } = null!;

        /// <summary>User email</summary>
        public string Email { get; set; } = null!;
    }
}
