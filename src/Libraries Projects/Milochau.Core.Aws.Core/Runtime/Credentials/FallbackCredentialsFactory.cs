using System.Threading;

namespace Milochau.Core.Aws.Core.Runtime.Credentials
{
    // Credentials fallback mechanism
    public static class FallbackCredentialsFactory
    {
        // Lock to control caching credentials across multiple threads.
        private static ReaderWriterLockSlim cachedCredentialsLock = new ReaderWriterLockSlim();
    
        private static AWSCredentials cachedCredentials;
        public static AWSCredentials GetCredentials()
        {
            try
            {
                cachedCredentialsLock.EnterReadLock();
                if (cachedCredentials != null)
                {
                    return cachedCredentials;
                }
            }
            finally
            {
                cachedCredentialsLock.ExitReadLock();
            }
            
            try
            {
                cachedCredentialsLock.EnterWriteLock();
                if (cachedCredentials != null)
                {
                    return cachedCredentials;
                }
                
                cachedCredentials = new EnvironmentVariablesAWSCredentials();

                if (cachedCredentials == null)
                {
                    throw new AmazonServiceException("Unable to find credentials");
                }

                return cachedCredentials;
            }
            finally
            {
                cachedCredentialsLock.ExitWriteLock();
            }
        }
    }
}
