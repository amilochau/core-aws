using Amazon.Runtime;
using Milochau.Core.Aws.Core.Runtime.Credentials;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Integration
{
    /// <summary>
    /// Wrapper around AWS <see cref="Amazon.Runtime.AssumeRoleAWSCredentials"/>
    /// </summary>
    public class AssumeRoleAWSCredentials : IAWSCredentials
    {
        private readonly string roleArn;

        /// <summary>
        /// Constructor
        /// </summary>
        public AssumeRoleAWSCredentials(string roleArn)
        {
            this.roleArn = roleArn;
        }

        /// <inheritdoc/>
        public async Task<Core.Runtime.Credentials.ImmutableCredentials> GetCredentialsAsync()
        {
            // Get credentials from default profile
            var mainCredentials = FallbackCredentialsFactory.GetCredentials();
            var credentials = new Amazon.Runtime.AssumeRoleAWSCredentials(mainCredentials, roleArn, "debug");

            var immutableCredentials = await credentials.GetCredentialsAsync();

            return new Core.Runtime.Credentials.ImmutableCredentials
            {
                AccessKey = immutableCredentials.AccessKey,
                SecretKey = immutableCredentials.SecretKey,
                Token = immutableCredentials.Token,
            };
        }
    }
}
