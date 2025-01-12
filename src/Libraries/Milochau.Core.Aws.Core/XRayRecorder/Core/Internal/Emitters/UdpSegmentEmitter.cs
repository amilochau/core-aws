using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Utils;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Emitters
{
    /// <summary>
    /// Send the segment to daemon
    /// </summary>
    public class UdpSegmentEmitter : ISegmentEmitter
    {
        private readonly UdpClient udpClient = new UdpClient();
        private readonly DaemonConfig daemonConfig = DaemonConfig.GetEndPoint();
        private bool disposed;

        /// <summary>
        /// Send segment to local daemon
        /// </summary>
        /// <param name="segment">The segment to be sent</param>
        public void Send(Entity segment)
        {
            try
            {
                var packet = segment.Marshall();
                var data = Encoding.ASCII.GetBytes(packet);
                var ip = daemonConfig.UDPEndpoint; //Need local var to ensure ip do not updates
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
    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(FacadeSegment))]
    [JsonSerializable(typeof(Segment))]
    [JsonSerializable(typeof(Subsegment))]
    [JsonSerializable(typeof(Dictionary<string, long>))]
    [JsonSerializable(typeof(Dictionary<string, string>))]
    [JsonSerializable(typeof(Dictionary<string, string[]>))]
    internal partial class XRayJsonSerializerContext : JsonSerializerContext
    {
    }
}
