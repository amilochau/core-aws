using Amazon.Runtime;
using Milochau.Core.Aws.Core.Runtime.Credentials;
using System;
using System.Diagnostics;
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

            var mainCredentials = FallbackCredentialsFactory.GetCredentials() as SSOAWSCredentials;
            if (mainCredentials == null)
            {
                throw new ArgumentException(nameof(mainCredentials));
            }

            mainCredentials.Options.ClientName = "Local-App";
            mainCredentials.Options.SsoVerificationCallback = args =>
            {
                // Launch a browser window that prompts the SSO user to complete an SSO sign-in.
                // This method is only invoked if the session doesn't already have a valid SSO token.
                // NOTE: Process.Start might not support launching a browser on macOS or Linux. If not, use an appropriate mechanism on those systems instead.
                Process.Start(new ProcessStartInfo
                {
                    FileName = args.VerificationUriComplete,
                    UseShellExecute = true
                });
            };

            var credentials = new Amazon.Runtime.AssumeRoleAWSCredentials(mainCredentials, roleArn, "debug");

            var immutableCredentials = await credentials.GetCredentialsAsync();

            return new Core.Runtime.Credentials.ImmutableCredentials(immutableCredentials.AccessKey, immutableCredentials.SecretKey, immutableCredentials.Token);
        }
    }
}
