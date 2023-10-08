using System;
using Amazon.XRay.Recorder.Core.Internal.Entities;

namespace Amazon.XRay.Recorder.Core.Internal.Emitters
{
    /// <summary>
    /// Interface of segment emitter
    /// </summary>
    public interface ISegmentEmitter : IDisposable
    {
        /// <summary>
        /// Send the segment to service
        /// </summary>
        /// <param name="segment">Segment to send</param>
        void Send(Entity segment);

        /// <summary>
        /// Sets the daemon address.
        /// The daemon address should be in format "IPAddress:Port", i.e. "127.0.0.1:2000"
        /// </summary>
        /// <param name="daemonAddress">The daemon address.</param>
        void SetDaemonAddress(string daemonAddress);
    }
}
