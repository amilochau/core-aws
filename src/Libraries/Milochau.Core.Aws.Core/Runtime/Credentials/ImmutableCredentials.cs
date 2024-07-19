namespace Milochau.Core.Aws.Core.Runtime.Credentials
{
    /// <summary>
    /// Immutable representation of AWS credentials.
    /// </summary>
    public class ImmutableCredentials(string accessKey, string secretKey, string? token)
    {
        /// <summary>
        /// Gets the AccessKey property for the current credentials.
        /// </summary>
        public string AccessKey { get; } = accessKey;

        /// <summary>
        /// Gets the SecretKey property for the current credentials.
        /// </summary>
        public string SecretKey { get; } = secretKey;

        /// <summary>
        /// Gets the Token property for the current credentials.
        /// </summary>
        public string? Token { get; } = token;

        /// <summary>
        /// Gets the UseToken property for the current credentials.
        /// Specifies if Token property is non-empty.
        /// </summary>
        public bool UseToken => !string.IsNullOrEmpty(Token);
    }
}
