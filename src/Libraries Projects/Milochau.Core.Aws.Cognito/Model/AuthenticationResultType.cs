namespace Milochau.Core.Aws.Cognito.Model
{
    /// <summary>
    /// The authentication result.
    /// </summary>
    public partial class AuthenticationResultType
    {
        /// <summary>
        /// Gets and sets the property AccessToken. 
        /// <para>
        /// A valid access token that Amazon Cognito issued to the user who you want to authenticate.
        /// </para>
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// Gets and sets the property ExpiresIn. 
        /// <para>
        /// The expiration period of the authentication result in seconds.
        /// </para>
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Gets and sets the property IdToken. 
        /// <para>
        /// The ID token.
        /// </para>
        /// </summary>
        public string? IdToken { get; set; }

        /// <summary>
        /// Gets and sets the property NewDeviceMetadata. 
        /// <para>
        /// The new device metadata from an authentication result.
        /// </para>
        /// </summary>
        public NewDeviceMetadataType? NewDeviceMetadata { get; set; }

        /// <summary>
        /// Gets and sets the property RefreshToken. 
        /// <para>
        /// The refresh token.
        /// </para>
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Gets and sets the property TokenType. 
        /// <para>
        /// The token type.
        /// </para>
        /// </summary>
        public string? TokenType { get; set; }
    }
}
