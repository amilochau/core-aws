using System;
using System.Net.Sockets;
using System.Text;
using Milochau.Core.Aws.XRayRecorder.Core.Internal.Entities;
using Milochau.Core.Aws.XRayRecorder.Core.Internal.Utils;

namespace Milochau.Core.Aws.XRayRecorder.Core.Internal.Emitters
{
    /// <summary>
    /// Send the segment to daemon
    /// </summary>
    public class UdpSegmentEmitter : ISegmentEmitter
    {
        private readonly UdpClient _udpClient;
        private DaemonConfig _daemonConfig;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpSegmentEmitter"/> class.
        /// </summary>
        public UdpSegmentEmitter()
        {
            _udpClient = new UdpClient();
            _daemonConfig = DaemonConfig.GetEndPoint();
        }

        /// <summary>
        /// Send segment to local daemon
        /// </summary>
        /// <param name="segment">The segment to be sent</param>
        public void Send(Entity? segment)
        {
            if (segment == null)
            {
                return;
            }

            try
            {
                var packet = segment.Marshall()!;
                var data = Encoding.ASCII.GetBytes(packet);
                var ip = _daemonConfig.UDPEndpoint; //Need local var to ensure ip do not 
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
                _udpClient?.Dispose();

                _disposed = true;
            }
        }
    }
}
