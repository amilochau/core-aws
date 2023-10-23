using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Emitters;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Strategies;
using System;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core
{
    /// <summary>
    /// A collection of methods used to record tracing information for AWS X-Ray.
    /// </summary>
    /// <seealso cref="IAWSXRayRecorder" />
    public class AWSXRayRecorder
    {
        /// <summary>
        /// Emitter used to send Traces.
        /// </summary>
        private static readonly ISegmentEmitter emitter = new UdpSegmentEmitter();

        /// <summary>
        /// Instance of <see cref="IStreamingStrategy"/>, used to define the streaming strategy for segment/subsegment.
        /// </summary>
        private static readonly IStreamingStrategy streamingStrategy = new DefaultStreamingStrategy();

        /// <summary>
        /// Defines exception serialization stategy to process recorded exceptions. <see cref="IExceptionSerializationStrategy"/>
        /// </summary>
        public static IExceptionSerializationStrategy ExceptionSerializationStrategy { get; } = new DefaultExceptionSerializationStrategy();

        /// <summary>
        /// Begin a tracing subsegment. A new segment will be created and added as a subsegment to previous segment/subsegment.
        /// </summary>
        public static Subsegment BeginSubsegment(FacadeSegment facadeSegment, string name)
        {
            var environmentRootTraceId = TraceHeader.FromString(EnvironmentVariables.GetEnvironmentVariable(EnvironmentVariables.Key_TraceId)).RootTraceId;

            if (environmentRootTraceId != null && !environmentRootTraceId.Equals(facadeSegment.RootSegment?.TraceId)) // If true, customer has leaked subsegments across invocation
            {
                return BeginSubsegment(facadeSegment, name); // This adds Facade segment with updated environment variables
            }
            else
            {
                var subsegment = new Subsegment(name, facadeSegment);
                facadeSegment.AddSubsegment(subsegment);
                subsegment.Sampled = facadeSegment.Sampled;
                subsegment.SetStartTimeToNow();
                return subsegment;
            }
        }

        /// <summary>
        /// End a subsegment.
        /// </summary>
        public static void EndSubsegment(Subsegment subsegment)
        {
            subsegment.IsInProgress = false;
            subsegment.Release();
            subsegment.SetEndTimeToNow();

            // Check emittable
            if (subsegment.IsEmittable())
            {
                // Emit
                emitter.Send(subsegment.RootSegment);
            }
            else if (streamingStrategy.ShouldStream(subsegment))
            {
                streamingStrategy.Stream(subsegment.RootSegment, emitter);
            }

            subsegment.Parent.IsInProgress = false;
            subsegment.Parent.Release();
            if (subsegment.Parent.RootSegment != null && subsegment.Parent.RootSegment.Size >= 0)
            {
                streamingStrategy.Stream(subsegment.Parent, emitter); //Facade segment is not emitted, all its subsegments, if emmittable, are emitted
            }
        }
    }
}
