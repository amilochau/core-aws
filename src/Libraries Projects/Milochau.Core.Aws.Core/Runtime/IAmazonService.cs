namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// All Amazon service interfaces like IAmazonS3 extend from this interface. This allows all the 
    /// Amazon service interfaces be identified by this base interface and helps with generic constraints.
    /// </summary>
    public interface IAmazonService
    {
        /// <summary>
        /// A readonly view of the configuration for the service client.
        /// </summary>
        IClientConfig Config
        {
            get;
        }
    }
}