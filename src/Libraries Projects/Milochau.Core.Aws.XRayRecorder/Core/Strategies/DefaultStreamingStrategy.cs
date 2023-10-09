using System;
using Milochau.Core.Aws.XRayRecorder.Core.Internal.Entities;
using Milochau.Core.Aws.XRayRecorder.Core.Internal.Emitters;
using Milochau.Core.Aws.XRayRecorder.Core.Sampling;

namespace Milochau.Core.Aws.XRayRecorder.Core.Strategies
{
    /// <summary>
    /// The default streaming strategy. It uses the total count of a segment's children subsegments as a threshold. If the threshold is breached, it uses subtree streaming to stream out.
    /// </summary>
    public class DefaultStreamingStrategy : IStreamingStrategy
    {
        /// <summary>
        /// Default max subsegment size to stream for the strategy.
        /// </summary>
        private const long DefaultMaxSubsegmentSize = 100;
        
        /// <summary>
        /// Max subsegment size to stream fot the strategy.
        /// </summary>
        public long MaxSubsegmentSize { get; private set; } = DefaultMaxSubsegmentSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultStreamingStrategy"/> class.
        /// </summary>
        public DefaultStreamingStrategy() : this(DefaultMaxSubsegmentSize)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultStreamingStrategy"/> class.
        /// </summary>
        /// <param name="maxSubsegmentSize"></param>
        public DefaultStreamingStrategy(long maxSubsegmentSize)
        {
            if(maxSubsegmentSize < 0)
            {
                throw new ArgumentException("maxSubsegmentSize cannot be a negative number.");
            }
            MaxSubsegmentSize = maxSubsegmentSize;
        }

        /// <summary>
        /// Checks whether subsegments of the current instance of  <see cref="Entity"/> should be streamed.
        /// </summary>
        /// <param name="entity">Instance of <see cref="Entity"/></param>
        /// <returns>True if the subsegments are streamable.</returns>
        public bool ShouldStream(Entity entity)
        {
            return entity.Sampled == SampleDecision.Sampled && entity.RootSegment != null && entity.RootSegment.Size >= MaxSubsegmentSize;
        }

        private readonly object subsegmentsLock = new();
        /// <summary>
        /// Streams subsegments of instance of <see cref="Entity"/>.
        /// </summary>
        /// <param name="entity">Instance of <see cref="Entity"/>.</param>
        /// <param name="emitter">Instance of <see cref="ISegmentEmitter"/>.</param>
        public void Stream(Entity? entity, ISegmentEmitter emitter)
        {
            if (entity == null)
            {
                return;
            }

            lock (subsegmentsLock)
            {
                if (entity.Subsegments != null)
                {
                    foreach (var next in entity.Subsegments)
                    {
                        Stream(next, emitter);
                    }

                    entity.Subsegments.RemoveAll(x => x.HasStreamed);
                }
            }

            if (entity.Sampled != SampleDecision.Sampled || entity is Segment || entity.IsInProgress || entity.Reference > 0 || entity.Subsegments != null)
            {
                return;
            }

            var subsegment = entity as Subsegment;
            if (subsegment != null)
            {
                subsegment.TraceId = entity.RootSegment?.TraceId;
                subsegment.Type = "subsegment";
                subsegment.ParentId = subsegment.Parent.Id;
                emitter.Send(subsegment);
                subsegment.RootSegment?.DecrementSize();
                subsegment.HasStreamed = true;
            }
        }
    }
}
