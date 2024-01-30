namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// This class is the base class of all the configurations settings to connect
    /// to a service.
    /// </summary>
    public class ClientConfig : IClientConfig
    {
        /// <summary>Name of the service</summary>
        /// <remarks>Used to sign requests. See AmazonCognitoIdentityProviderConfig.AuthenticationServiceName in official SDK</remarks>
        public required string AuthenticationServiceName { get; set; }

        /// <summary>Name of the service</summary>
        /// <remarks>Used to monitor requests. See </remarks>
        public required string MonitoringServiceName { get; set; }
    }
}
