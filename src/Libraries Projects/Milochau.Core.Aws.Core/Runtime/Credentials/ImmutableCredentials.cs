using Milochau.Core.Aws.Core.Runtime.Internal.Util;
using Milochau.Core.Aws.Core.Util;

namespace Milochau.Core.Aws.Core.Runtime.Credentials
{
    /// <summary>
    /// Immutable representation of AWS credentials.
    /// </summary>
    public class ImmutableCredentials
    {
        #region Properties

        /// <summary>
        /// Gets the AccessKey property for the current credentials.
        /// </summary>
        public string AccessKey { get; private set; }

        /// <summary>
        /// Gets the SecretKey property for the current credentials.
        /// </summary>
        public string SecretKey { get; private set; }

        /// <summary>
        /// Gets the Token property for the current credentials.
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        /// Gets the UseToken property for the current credentials.
        /// Specifies if Token property is non-empty.
        /// </summary>
        public bool UseToken { get { return !string.IsNullOrEmpty(Token); } }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs an ImmutableCredentials object with supplied accessKey, secretKey.
        /// </summary>
        /// <param name="awsAccessKeyId"></param>
        /// <param name="awsSecretAccessKey"></param>
        /// <param name="token">Optional. Can be set to null or empty for non-session credentials.</param>
        public ImmutableCredentials(string awsAccessKeyId, string awsSecretAccessKey, string token)
        {
            AccessKey = awsAccessKeyId;
            SecretKey = awsSecretAccessKey;
            Token = token ?? string.Empty;
        }

        #endregion

        #region Overrides

        public override int GetHashCode()
        {
            return Hashing.Hash(AccessKey, SecretKey, Token);
        }

        public override bool Equals(object? obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            ImmutableCredentials? ic = obj as ImmutableCredentials;
            if (ic == null)
                return false;

            return AWSSDKUtils.AreEqual(
                new object[] { AccessKey, SecretKey, Token },
                new object[] { ic.AccessKey, ic.SecretKey, ic.Token });
        }

        #endregion
    }
}
