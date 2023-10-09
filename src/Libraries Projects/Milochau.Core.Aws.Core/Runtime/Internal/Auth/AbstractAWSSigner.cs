using Milochau.Core.Aws.Core.Runtime.Credentials;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Auth
{
    public abstract class AbstractAWSSigner
    {
        public abstract void Sign(IRequest request, IClientConfig clientConfig, string awsAccessKeyId, string awsSecretAccessKey);

        public virtual void Sign(IRequest request, IClientConfig clientConfig, ImmutableCredentials credentials)
        {
            Sign(request, clientConfig, credentials?.AccessKey, credentials?.SecretKey);
        }

        public virtual Task SignAsync(
            IRequest request, 
            IClientConfig clientConfig,
            ImmutableCredentials credentials,
            CancellationToken token = default)
        {
            Sign(request, clientConfig, credentials);
            return Task.CompletedTask;
        }
    }
}
