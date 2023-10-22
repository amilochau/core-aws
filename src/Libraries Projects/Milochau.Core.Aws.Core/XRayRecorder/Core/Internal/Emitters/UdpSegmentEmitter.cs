using System;
using System.Net.Sockets;
using System.Text;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Utils;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Emitters
{
    /// <summary>
    /// Send the segment to daemon
    /// </summary>
    public class UdpSegmentEmitter : ISegmentEmitter
    {
        private readonly UdpClient udpClient;
        private readonly DaemonConfig daemonConfig;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpSegmentEmitter"/> class.
        /// </summary>
        public UdpSegmentEmitter()
        {
            udpClient = new UdpClient();
            daemonConfig = DaemonConfig.GetEndPoint();
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
                var ip = daemonConfig.UDPEndpoint; //Need local var to ensure ip do not 
                udpClient.Send(data, data.Length, ip);
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
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                udpClient?.Dispose();

                disposed = true;
            }
        }
    }
}
