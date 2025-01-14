using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Exceptions;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Emitters;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Sampling;
using System;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core
{
    /// <summary>
    /// A collection of methods used to record tracing information for AWS X-Ray.
    /// </summary>
    /// <seealso cref="Amazon.XRay.Recorder.Core.IAWSXRayRecorder" />
    public class AWSXRayRecorder : AWSXRayRecorderImpl
    {
        private static string? _lambdaVariables;

        /// <summary>
        /// Initializes a new instance of the <see cref="AWSXRayRecorder" /> class.
        /// with default configuration.
        /// </summary>
        public AWSXRayRecorder() : base(new UdpSegmentEmitter())
        {
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="AWSXRayRecorder"/> with default configuration.
        /// </summary>
        /// <returns>An instance of <see cref="AWSXRayRecorder"/> class.</returns>
        public static AWSXRayRecorder Instance { get; } = new AWSXRayRecorder();

        /// <summary>
        /// Begin a tracing subsegment. A new segment will be created and added as a subsegment to previous segment/subsegment.
        /// </summary>
        /// <param name="name">Name of the operation</param>
        /// <param name="timestamp">Sets the start time of the subsegment</param>
        /// <exception cref="ArgumentNullException">The argument has a null value.</exception>
        /// <exception cref="EntityNotAvailableException">Entity is not available in trace context.</exception>
        public override void BeginSubsegment(string name)
        {
            try
            {
                ProcessSubsegmentInLambdaContext(name);
            }
            catch (EntityNotAvailableException)
            {
            }
        }

        /// <summary>
        /// Begin a tracing subsegment. A new subsegment will be created and added as a subsegment to previous facade segment or subsegment.
        /// </summary>
        private void ProcessSubsegmentInLambdaContext(string name)
        {
            if (!TraceContext.IsEntityPresent()) // No facade segment available and first subsegment of a subsegment branch needs to be added
            {
                AddFacadeSegment();
                AddSubsegmentInLambdaContext(name);
            }
            else // Facade / Subsegment already present
            {
                var entity = TraceContext.GetEntity(); // can be Facade segment or Subsegment
                var environmentRootTraceId = TraceHeader.FromString(EnvironmentVariables.TraceId).RootTraceId;

                if (environmentRootTraceId != null && !environmentRootTraceId.Equals(entity.RootSegment?.TraceId)) // If true, customer has leaked subsegments across invocation
                {
                    TraceContext.ClearEntity(); // reset TraceContext
                    BeginSubsegment(name); // This adds Facade segment with updated environment variables
                }
                else
                {
                    AddSubsegmentInLambdaContext(name);
                }
            }
        }

        /// <summary>
        /// Begin a Facade Segment.
        /// </summary>
        internal void AddFacadeSegment()
        {
            _lambdaVariables = EnvironmentVariables.TraceId;

            if (!TraceHeader.TryParseAll(_lambdaVariables, out TraceHeader traceHeader))
            {
                traceHeader = new TraceHeader
                {
                    RootTraceId = TraceId.NewId(),
                    ParentId = null,
                    Sampled = SampleDecision.NotSampled
                };
            }

            Segment newSegment = new FacadeSegment("Facade", traceHeader.RootTraceId, traceHeader.ParentId)
            {
                Sampled = traceHeader.Sampled,
            };
            TraceContext.SetEntity(newSegment);
        }

        private void AddSubsegmentInLambdaContext(string name)
        {
            // If the request is not sampled, the passed subsegment will still be available in TraceContext to
            // stores the information of the trace. The trace information will still propagated to 
            // downstream service, in case downstream may overwrite the sample decision.
            Entity parentEntity = TraceContext.GetEntity();
            var subsegment = new Subsegment(name)
            {
                RootSegment = parentEntity.RootSegment,
            };
            parentEntity.AddSubsegment(subsegment);
            subsegment.Sampled = parentEntity.Sampled;
            subsegment.SetStartTimeToNow();
            TraceContext.SetEntity(subsegment);
        }

        /// <summary>
        /// End a subsegment.
        /// </summary>
        /// <param name="timestamp">Sets the end time for the subsegment</param>
        /// <exception cref="EntityNotAvailableException">Entity is not available in trace context.</exception>
        public override void EndSubsegment()
        {
            try
            {
                ProcessEndSubsegmentInLambdaContext();
            }
            catch (EntityNotAvailableException)
            {
            }
            catch (InvalidCastException)
            {
            }
        }

        private void ProcessEndSubsegmentInLambdaContext()
        {
            var subsegment = PrepEndSubsegmentInLambdaContext();

            subsegment.SetEndTimeToNow();

            // Check emittable
            if (subsegment.RootSegment != null && subsegment.IsEmittable())
            {
                // Emit
                Emitter.Send(subsegment.RootSegment);
            }
            else if (subsegment.RootSegment != null && StreamingStrategy.ShouldStream(subsegment))
            {
                StreamingStrategy.Stream(subsegment.RootSegment, Emitter);
            }

            if (TraceContext.IsEntityPresent() && TraceContext.GetEntity().GetType() == typeof(FacadeSegment)) //implies FacadeSegment in the Trace Context
            {
                EndFacadeSegment();
                return;
            }
        }

        private Subsegment PrepEndSubsegmentInLambdaContext()
        {
            // If the request is not sampled, a subsegment will still be available in TraceContext.
            //This behavor is specific to AWS Lambda environment
            Entity entity = TraceContext.GetEntity();
            Subsegment subsegment = (Subsegment)entity;

            subsegment.IsInProgress = false;

            // Restore parent segment to trace context
            if (subsegment.Parent != null)
            {
                TraceContext.SetEntity(subsegment.Parent);
            }

            // Drop ref count
            subsegment.Release();

            return subsegment;
        }

        private void EndFacadeSegment()
        {
            try
            {
                // If the request is not sampled, a segment will still be available in TraceContext.
                // Need to clean up the segment, but do not emit it.
                FacadeSegment facadeSegment = (FacadeSegment)TraceContext.GetEntity();

                PrepEndSegment(facadeSegment);
                if (facadeSegment.RootSegment != null && facadeSegment.RootSegment.Size >= 0)
                {
                    StreamingStrategy.Stream(facadeSegment, Emitter); //Facade segment is not emitted, all its subsegments, if emmittable, are emitted
                }

                TraceContext.ClearEntity();
            }
            catch (EntityNotAvailableException)
            {
            }
            catch (InvalidCastException)
            {
            }
        }
    }
}
