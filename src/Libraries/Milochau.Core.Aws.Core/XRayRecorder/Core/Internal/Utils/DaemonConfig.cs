using Milochau.Core.Aws.Core.References;
using System.Net;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Utils
{
    /// <summary>
    /// DaemonConfig stores X-Ray daemon configuration about the ip address and port for UDP and TCP port. It gets the address
    /// string from "AWS_TRACING_DAEMON_ADDRESS" and then from recorder's configuration for "daemon_address".
    /// A notation of '127.0.0.1:2000' or 'tcp:127.0.0.1:2000 udp:127.0.0.2:2001' or 'udp:127.0.0.1:2000 tcp:127.0.0.2:2001'
    /// are both acceptable. The first one means UDP and TCP are running at the same address.
    /// By default it assumes a X-Ray daemon running at 127.0.0.1:2000 listening to both UDP and TCP traffic.
    /// </summary>
    public class DaemonConfig
    {
        /// <summary>
        /// Default address for daemon.
        /// </summary>
        public const string DefaultAddress = "127.0.0.1:2000";
        private static readonly int _defaultDaemonPort = 2000;
        private static readonly IPAddress _defaultDaemonAddress = IPAddress.Loopback;

        /// <summary>
        /// Default UDP and TCP endpoint.
        /// </summary>
        public static readonly IPEndPoint DefaultEndpoint = new IPEndPoint(_defaultDaemonAddress, _defaultDaemonPort);

        /// <summary>
        /// Gets or sets UDP endpoint.
        /// </summary>
        internal EndPoint _udpEndpoint;

        /// <summary>
        /// Gets IP for UDP endpoint.
        /// </summary>
        public IPEndPoint UDPEndpoint
        {
            get => _udpEndpoint.GetIPEndPoint();
            set => _udpEndpoint = EndPoint.Of(value);
        }

        public DaemonConfig()
        {
            _udpEndpoint = EndPoint.Of(DefaultEndpoint);
        }

        internal static DaemonConfig ParsEndpoint(string? daemonAddress)
        {
            if (!IPEndPointExtension.TryParse(daemonAddress, out DaemonConfig? daemonEndPoint))
            {
                daemonEndPoint = new DaemonConfig();
            }
            return daemonEndPoint;
        }

        /// <summary>
        /// Parses daemonAddress and sets enpoint. If <see cref="EnvironmentVariableDaemonAddress"/> is set, this call is ignored.
        /// </summary>
        /// <param name="daemonAddress"> Dameon address to be parsed and set to <see cref="DaemonConfig"/> instance.</param>
        /// <returns></returns>
        public static DaemonConfig GetEndPoint()
        {
            return ParsEndpoint(EnvironmentVariables.GetEnvironmentVariable(EnvironmentVariables.Key_DaemonAddress));
        }
    }
}
