using System;
using Milochau.Core.Aws.XRayRecorder.Core.Exceptions;
using Milochau.Core.Aws.XRayRecorder.Core.Internal.Emitters;
using Milochau.Core.Aws.XRayRecorder.Core.Internal.Entities;
using Milochau.Core.Aws.XRayRecorder.Core.Sampling;

namespace Milochau.Core.Aws.XRayRecorder.Core
{
    /// <summary>
    /// A collection of methods used to record tracing information for AWS X-Ray.
    /// </summary>
    /// <seealso cref="IAWSXRayRecorder" />
    public class AWSXRayRecorder : AWSXRayRecorderImpl
    {
        static AWSXRayRecorder _instance = AWSXRayRecorderBuilder.Build();
        private const string LambdaTraceHeaderKey = "_X_AMZN_TRACE_ID";

        private static string? _lambdaVariables;

        /// <summary>
        /// Initializes a new instance of the <see cref="AWSXRayRecorder" /> class.
        /// with default configuration.
        /// </summary>
        public AWSXRayRecorder() : this(new UdpSegmentEmitter())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AWSXRayRecorder" /> class
        /// with given instance of <see cref="ISegmentEmitter" />.
        /// </summary>
        /// <param name="emitter">Segment emitter</param>
        internal AWSXRayRecorder(ISegmentEmitter emitter) : base(emitter)
        {
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="AWSXRayRecorder"/> with default configuration.
        /// </summary>
        /// <returns>An instance of <see cref="AWSXRayRecorder"/> class.</returns>
        public static AWSXRayRecorder Instance
        {
            get
            {
                _instance ??= AWSXRayRecorderBuilder.Build();

                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        /// <summary>
        /// Begin a tracing subsegment. A new segment will be created and added as a subsegment to previous segment/subsegment.
        /// </summary>
        /// <param name="name">Name of the operation</param>
        /// <param name="timestamp">Sets the start time of the subsegment</param>
        /// <exception cref="ArgumentNullException">The argument has a null value.</exception>
        /// <exception cref="EntityNotAvailableException">Entity is not available in trace context.</exception>
        public override void BeginSubsegment(string name, DateTime? timestamp = null)
        {
            try
            {
                ProcessSubsegmentInLambdaContext(name, timestamp);
            }
            catch (EntityNotAvailableException e)
            {
                HandleEntityNotAvailableException(e, "Failed to start subsegment because the parent segment is not available.");
            }
        }

        /// <summary>
        /// Begin a tracing subsegment. A new subsegment will be created and added as a subsegment to previous facade segment or subsegment.
        /// </summary>
        private void ProcessSubsegmentInLambdaContext(string name, DateTime? timestamp = null)
        {
            if (!TraceContext.IsEntityPresent()) // No facade segment available and first subsegment of a subsegment branch needs to be added
            {
                AddFacadeSegment();
                AddSubsegmentInLambdaContext(name, timestamp);
            }
            else // Facade / Subsegment already present
            {
                var entity = TraceContext.GetEntity(); // can be Facade segment or Subsegment
                var environmentRootTraceId = TraceHeader.FromString(GetTraceVariablesFromEnvironment()).RootTraceId;

                if ((null != environmentRootTraceId) && !environmentRootTraceId.Equals(entity.RootSegment?.TraceId)) // If true, customer has leaked subsegments across invocation
                {
                    TraceContext.ClearEntity(); // reset TraceContext
                    BeginSubsegment(name, timestamp); // This adds Facade segment with updated environment variables
                }
                else
                {
                    AddSubsegmentInLambdaContext(name, timestamp);
                }
            }
        }

        /// <summary>
        /// Begin a Facade Segment.
        /// </summary>
        internal void AddFacadeSegment()
        {
            _lambdaVariables = GetTraceVariablesFromEnvironment();

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
                Sampled = traceHeader.Sampled
            };
            TraceContext.SetEntity(newSegment);
        }

        private void AddSubsegmentInLambdaContext(string name, DateTime? timestamp = null)
        {
            // If the request is not sampled, the passed subsegment will still be available in TraceContext to
            // stores the information of the trace. The trace information will still propagated to 
            // downstream service, in case downstream may overwrite the sample decision.
            Entity parentEntity = TraceContext.GetEntity();
            Subsegment subsegment = new Subsegment(name, parentEntity);
            parentEntity.AddSubsegment(subsegment);
            subsegment.Sampled = parentEntity.Sampled;
            if (timestamp == null)
            {
                subsegment.SetStartTimeToNow();
            }
            else
            {
                subsegment.SetStartTime(timestamp.Value);
            }
            TraceContext.SetEntity(subsegment);
        }

        /// <summary>
        /// End a subsegment.
        /// </summary>
        /// <param name="timestamp">Sets the end time for the subsegment</param>
        /// <exception cref="EntityNotAvailableException">Entity is not available in trace context.</exception>
        public override void EndSubsegment(DateTime? timestamp = null)
        {
            try
            {
                ProcessEndSubsegmentInLambdaContext(timestamp);
            }
            catch (EntityNotAvailableException e)
            {
                HandleEntityNotAvailableException(e, "Failed to end subsegment because subsegment is not available in trace context.");
            }
            catch (InvalidCastException e)
            {
                HandleEntityNotAvailableException(new EntityNotAvailableException("Failed to cast the entity to Subsegment.", e), "Failed to cast the entity to Subsegment.");
            }
        }

        private void ProcessEndSubsegmentInLambdaContext(DateTime? timestamp = null)
        {
            var subsegment = PrepEndSubsegmentInLambdaContext();

            if (timestamp == null)
            {
                subsegment.SetEndTimeToNow();
            }
            else
            {
                subsegment.SetEndTime(timestamp.Value);
            }

            // Check emittable
            if (subsegment.IsEmittable())
            {
                // Emit
                Emitter.Send(subsegment.RootSegment);
            }
            else if (StreamingStrategy.ShouldStream(subsegment))
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
            TraceContext.SetEntity(subsegment.Parent);

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
            catch (EntityNotAvailableException e)
            {
                HandleEntityNotAvailableException(e, "Failed to end facade segment because cannot get the segment from trace context.");
            }
            catch (InvalidCastException e)
            {
                HandleEntityNotAvailableException(new EntityNotAvailableException("Failed to cast the entity to Facade segment.", e), "Failed to cast the entity to Facade Segment.");
            }
        }

        /// <summary>
        /// Returns value set for environment variable "_X_AMZN_TRACE_ID"
        /// </summary>
        private static string? GetTraceVariablesFromEnvironment()
        {
            return Environment.GetEnvironmentVariable(LambdaTraceHeaderKey);
        }
    }
}
