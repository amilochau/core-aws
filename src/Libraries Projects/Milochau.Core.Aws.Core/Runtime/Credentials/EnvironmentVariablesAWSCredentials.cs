using System;

namespace Milochau.Core.Aws.Core.Runtime.Credentials
{
    /// <summary>
    /// Uses aws credentials stored in environment variables to construct the credentials object.
    /// AWS_ACCESS_KEY_ID and AWS_SECRET_ACCESS_KEY are used for the access key id and secret key. 
    /// If the variable AWS_SESSION_TOKEN exists then it will be used to create temporary session 
    /// credentials.
    /// </summary>
    public class EnvironmentVariablesAWSCredentials : AWSCredentials
    {
        public const string ENVIRONMENT_VARIABLE_ACCESSKEY = "AWS_ACCESS_KEY_ID";
        public const string ENVIRONMENT_VARIABLE_SECRETKEY = "AWS_SECRET_ACCESS_KEY";
        public const string ENVIRONMENT_VARIABLE_SESSION_TOKEN = "AWS_SESSION_TOKEN";

        #region Public constructors

        /// <summary>
        /// Constructs an instance of EnvironmentVariablesAWSCredentials. If no credentials are found in 
        /// the environment variables then an InvalidOperationException is thrown.
        /// </summary>
        public EnvironmentVariablesAWSCredentials()
        {
            // We need to do an initial fetch to validate that we can use environment variables to get the credentials.
            FetchCredentials();
        }

        #endregion

        /// <summary>
        /// Creates immutable credentials from environment variables.
        /// </summary>
        /// <returns></returns>
        public static ImmutableCredentials FetchCredentials()
        {
            string accessKeyId = Environment.GetEnvironmentVariable(ENVIRONMENT_VARIABLE_ACCESSKEY);
            string secretKey = Environment.GetEnvironmentVariable(ENVIRONMENT_VARIABLE_SECRETKEY);
            string sessionToken = Environment.GetEnvironmentVariable(ENVIRONMENT_VARIABLE_SESSION_TOKEN);

            return new ImmutableCredentials(accessKeyId, secretKey, sessionToken);
        }

        /// <summary>
        /// Returns an instance of ImmutableCredentials for this instance
        /// </summary>
        /// <returns></returns>
        public override ImmutableCredentials GetCredentials()
        {
            return FetchCredentials();
        }
    }
}
