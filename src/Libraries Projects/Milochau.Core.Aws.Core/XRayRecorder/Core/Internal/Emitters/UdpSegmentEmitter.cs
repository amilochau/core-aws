using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Amazon.XRay.Recorder.Core.Internal.Entities;
using Amazon.XRay.Recorder.Core.Internal.Utils;

namespace Amazon.XRay.Recorder.Core.Internal.Emitters
{
    /// <summary>
    /// Send the segment to daemon
    /// </summary>
    public class UdpSegmentEmitter : ISegmentEmitter
    {
        private readonly ISegmentMarshaller _marshaller;
        private readonly UdpClient _udpClient;
        private DaemonConfig _daemonConfig;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpSegmentEmitter"/> class.
        /// </summary>
        public UdpSegmentEmitter() : this(new JsonSegmentMarshaller())
        {
        }

        private UdpSegmentEmitter(ISegmentMarshaller marshaller)
        {
            _marshaller = marshaller;
            _udpClient = new UdpClient();
            _daemonConfig = DaemonConfig.GetEndPoint();
        }

        /// <summary>
        /// Gets the end point to daemon.
        /// <para>
        /// Two successive calls may not return the same IP as the backing
        /// endpoint may be a HostEndpoint that could update.
        /// </para>
        /// </summary>
        public IPEndPoint EndPoint => _daemonConfig.UDPEndpoint;

        /// <summary>
        /// Send segment to local daemon
        /// </summary>
        /// <param name="segment">The segment to be sent</param>
        public void Send(Entity segment)
        {
            try
            {
                var packet = _marshaller.Marshall(segment);
                var data = Encoding.ASCII.GetBytes(packet);
                var ip = EndPoint; //Need local var to ensure ip do not updates
                _udpClient.Send(data, data.Length, ip);
            }
            catch (SocketException)
            {
            }
            catch (ArgumentNullException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary>
        /// Sets the daemon address.
        /// The daemon address should be in format "IPAddress:Port", i.e. "127.0.0.1:2000"
        /// </summary>
        /// <param name="daemonAddress">The daemon address.</param>
        public void SetDaemonAddress(string daemonAddress)
        {
            if (Environment.GetEnvironmentVariable(DaemonConfig.EnvironmentVariableDaemonAddress) == null)
            {
                SetEndPointOrDefault(daemonAddress);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_udpClient != null)
                {
                    _udpClient.Dispose();
                }

                _disposed = true;
            }
        }

        private void SetEndPointOrDefault(string daemonAddress)
        {
            _daemonConfig = DaemonConfig.GetEndPoint(daemonAddress);
        }
    }
}
