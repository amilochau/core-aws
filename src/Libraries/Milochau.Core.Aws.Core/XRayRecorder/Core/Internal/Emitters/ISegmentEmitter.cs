using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
using System;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Emitters
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
    }
}
