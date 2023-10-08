using Amazon.XRay.Recorder.Core.Internal.Entities;
using Amazon.XRay.Recorder.Core.Internal.Emitters;

namespace Amazon.XRay.Recorder.Core.Strategies
{
    /// <summary>
    /// Interface of streaming strategy which is used to determine when and how the subsegments will be streamed.
    /// </summary>
    public interface IStreamingStrategy
    {
        /// <summary>
        /// Determines whenther or not the provided segment/subsegment requires any subsegment streaming.
        /// </summary>
        /// <param name="entity">An instance of <see cref="Entity"/>.</param>
        /// <returns>true if the segment/subsegment should be streamed.</returns>
        bool ShouldStream(Entity entity);

        /// <summary>
        /// Streams subsegments of instance of <see cref="Entity"/>.
        /// </summary>
        /// <param name="entity">Instance of <see cref="Entity"/>.</param>
        /// <param name="emitter">Instance if <see cref="ISegmentEmitter"/>.</param>
        void Stream(Entity entity, ISegmentEmitter emitter);
    }
}
