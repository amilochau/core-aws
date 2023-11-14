namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// This interface is the read only access to the ClientConfig object used when setting up service clients. Once service clients
    /// are initiated the config object should not be changed to avoid issues with using a service client in a multi threaded environment.
    /// </summary>
    public partial interface IClientConfig
    {
        /// <summary>Name of the service</summary>
        /// <remarks>Used to sign requests</remarks>
        string AuthenticationServiceName { get; }

        /// <summary>Name of the service</summary>
        /// <remarks>
        /// Used to monitor requests
        /// Should not start with "Amazon" or "."
        /// "SimpleNotificationService" should be set to "SNS"</remarks>
        string MonitoringServiceName { get; }
    }
}