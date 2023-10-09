namespace Milochau.Core.Aws.Core.Runtime.Credentials
{
    /// <summary>
    /// Abstract class that represents a credentials object for AWS services.
    /// </summary>
    public abstract class AWSCredentials
    {
        /// <summary>
        /// Returns a copy of ImmutableCredentials
        /// </summary>
        /// <returns></returns>
        public abstract ImmutableCredentials GetCredentials();

        /// <summary>
        /// Called by AmazonServiceClient to validate the credential state
        /// on client construction.
        /// </summary>
        protected virtual void Validate() { }

        public virtual System.Threading.Tasks.Task<ImmutableCredentials> GetCredentialsAsync()
        {
            return System.Threading.Tasks.Task.FromResult<ImmutableCredentials>(GetCredentials());
        }
    }
}
