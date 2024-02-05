using Milochau.Core.Aws.Core.References;
using System;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Runtime.Credentials
{
    /// <summary>
    /// Uses aws credentials stored in environment variables to construct the credentials object.
    /// AWS_ACCESS_KEY_ID and AWS_SECRET_ACCESS_KEY are used for the access key id and secret key. 
    /// If the variable AWS_SESSION_TOKEN exists then it will be used to create temporary session 
    /// credentials.
    /// </summary>
    public class EnvironmentVariablesAWSCredentials : IAWSCredentials
    {
        /// <summary>
        /// Constructs an instance of EnvironmentVariablesAWSCredentials. If no credentials are found in 
        /// the environment variables then an InvalidOperationException is thrown.
        /// </summary>
        public EnvironmentVariablesAWSCredentials()
        {
            // We need to do an initial fetch to validate that we can use environment variables to get the credentials.
            FetchCredentials();
        }

        /// <summary>
        /// Creates immutable credentials from environment variables.
        /// </summary>
        private static ImmutableCredentials FetchCredentials()
        {
            string accessKeyId = EnvironmentVariables.AccessKey;
            string secretKey = EnvironmentVariables.SecretKey;

            if (string.IsNullOrEmpty(accessKeyId) || string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("The environment variables were not set with AWS credentials.");
            }

            string? sessionToken = EnvironmentVariables.Token;

            return new ImmutableCredentials
            {
                AccessKey = accessKeyId,
                SecretKey = secretKey,
                Token = sessionToken
            };
        }

        /// <summary>
        /// Returns an instance of ImmutableCredentials for this instance
        /// </summary>
        public Task<ImmutableCredentials> GetCredentialsAsync()
        {
            return Task.FromResult(FetchCredentials());
        }
    }
}
