namespace Milochau.Core.Aws.Core.Runtime.Credentials
{
    /// <summary>
    /// Immutable representation of AWS credentials.
    /// </summary>
    public class ImmutableCredentials
    {
        /// <summary>
        /// Gets the AccessKey property for the current credentials.
        /// </summary>
        public string AccessKey { get; set; } = null!; // @todo use required with .NET 8

        /// <summary>
        /// Gets the SecretKey property for the current credentials.
        /// </summary>
        public string SecretKey { get; set; } = null!; // @todo use required with .NET 8

        /// <summary>
        /// Gets the Token property for the current credentials.
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// Gets the UseToken property for the current credentials.
        /// Specifies if Token property is non-empty.
        /// </summary>
        public bool UseToken { get { return !string.IsNullOrEmpty(Token); } }
    }
}
