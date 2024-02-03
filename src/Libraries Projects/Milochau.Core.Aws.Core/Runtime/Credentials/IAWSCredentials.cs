using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Runtime.Credentials
{
    /// <summary>
    /// Abstract class that represents a credentials object for AWS services.
    /// </summary>
    public interface IAWSCredentials
    {
        public Task<ImmutableCredentials> GetCredentialsAsync();
    }
}
