namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// This class is the base class of all the configurations settings to connect
    /// to a service.
    /// </summary>
    public abstract class ClientConfig : IClientConfig
    {
        public const int DefaultBufferSize = 8192;

        /// <summary>Name of the service</summary>
        /// <remarks>Used to sign requests</remarks>
        public string AuthenticationServiceName { get; protected set; }

        /// <summary>Name of the service</summary>
        /// <remarks>Used to monitor requests</remarks>
        public string MonitoringServiceName { get; protected set; }
    }
}
