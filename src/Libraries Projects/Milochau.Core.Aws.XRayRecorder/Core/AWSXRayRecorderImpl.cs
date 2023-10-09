using System;
using Milochau.Core.Aws.XRayRecorder.Core.Exceptions;
using Milochau.Core.Aws.XRayRecorder.Core.Internal.Context;
using Milochau.Core.Aws.XRayRecorder.Core.Internal.Emitters;
using Milochau.Core.Aws.XRayRecorder.Core.Internal.Entities;
using Milochau.Core.Aws.XRayRecorder.Core.Strategies;

namespace Milochau.Core.Aws.XRayRecorder.Core
{
    /// <summary>
    /// This class provides utilities to build an instance of <see cref="AWSXRayRecorder"/> with different configurations.
    /// </summary>
    public abstract class AWSXRayRecorderImpl : IAWSXRayRecorder
    {
        /// <summary>
        /// The environment variable that setting context missing strategy.
        /// </summary>
        public const string EnvironmentVariableContextMissingStrategy = "AWS_XRAY_CONTEXT_MISSING";

        /// <summary></summary>
        protected const long MaxSubsegmentSize = 100;

        /// <summary></summary>
        protected ContextMissingStrategy cntxtMissingStrategy = ContextMissingStrategy.LOG_ERROR;

        /// <summary></summary>
        protected AWSXRayRecorderImpl(ISegmentEmitter emitter)
        {
            Emitter = emitter;
        }

        /// <summary>
        /// Gets or sets the context missing strategy.
        /// </summary>
        public ContextMissingStrategy ContextMissingStrategy
        {
            get
            {
                return cntxtMissingStrategy;
            }

            set
            {
                cntxtMissingStrategy = value;
                string? modeFromEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableContextMissingStrategy);
                if (string.IsNullOrEmpty(modeFromEnvironmentVariable))
                {
                }
                else if (modeFromEnvironmentVariable.Equals(ContextMissingStrategy.LOG_ERROR.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    cntxtMissingStrategy = ContextMissingStrategy.LOG_ERROR;
                }
                else if (modeFromEnvironmentVariable.Equals(ContextMissingStrategy.RUNTIME_ERROR.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    cntxtMissingStrategy = ContextMissingStrategy.RUNTIME_ERROR;
                }
            }
        }

        /// <summary>
        /// Instance of <see cref="ITraceContext"/>, used to store segment/subsegment.
        /// </summary>
        public ITraceContext TraceContext { get; set; } = DefaultTraceContext.GetTraceContext();

        /// <summary>
        /// Emitter used to send Traces.
        /// </summary>
        public ISegmentEmitter Emitter { get; set; }

        /// <summary></summary>
        protected bool Disposed { get; set; }

        /// <summary>
        /// Defines exception serialization stategy to process recorded exceptions. <see cref="Strategies.IExceptionSerializationStrategy"/>
        /// </summary>
        public IExceptionSerializationStrategy ExceptionSerializationStrategy { get; set; } = new DefaultExceptionSerializationStrategy();

        /// <summary>
        /// Instance of <see cref="IStreamingStrategy"/>, used to define the streaming strategy for segment/subsegment.
        /// </summary>
        public IStreamingStrategy StreamingStrategy { get; set; } = new DefaultStreamingStrategy();

        /// <summary>
        /// Begin a tracing subsegment. A new subsegment will be created and added as a subsegment to previous segment.
        /// </summary>
        /// <param name="name">Name of the operation.</param>
        /// <param name="timestamp">Sets the start time of the subsegment</param>
        /// <exception cref="ArgumentNullException">The argument has a null value.</exception>
        /// <exception cref="EntityNotAvailableException">Entity is not available in trace context.</exception>
        public abstract void BeginSubsegment(string name, DateTime? timestamp = null);

        /// <summary>
        /// End a subsegment.
        /// </summary>
        /// <param name="timestamp">Sets the end time for the subsegment</param>
        public abstract void EndSubsegment(DateTime? timestamp = null);

        /// <summary>
        /// Set namespace to current segment.
        /// </summary>
        /// <param name="value">The value of the namespace.</param>
        public void SetNamespace(string value)
        {
            try
            {
                if (TraceContext.GetEntity() is not Subsegment subsegment)
                {
                    return;
                }

                subsegment.Namespace = value;
            }
            catch (EntityNotAvailableException e)
            {
                HandleEntityNotAvailableException(e, "Failed to set namespace because of subsegment is not available.");
            }
        }

        /// <summary>
        /// Adds the specified key and value as http information to current segment.
        /// </summary>
        /// <param name="key">The key of the http information to add.</param>
        /// <param name="value">The value of the http information to add.</param>
        /// <exception cref="ArgumentException">Key is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Value is null.</exception>
        /// <exception cref="EntityNotAvailableException">Entity is not available in trace context.</exception>
        public void AddHttpInformation(string key, object value)
        {
            try
            {
                TraceContext.GetEntity().AddToHttp(key, value);
            }
            catch (EntityNotAvailableException e)
            {
                HandleEntityNotAvailableException(e, "Failed to add http because segment is not available in trace context.");
            }
        }

        /// <summary>
        /// Mark the current segment as fault.
        /// </summary>
        /// <exception cref="EntityNotAvailableException">Entity is not available in trace context.</exception>
        public void MarkFault()
        {
            try
            {
                Entity entity = TraceContext.GetEntity();
                entity.HasFault = true;
                entity.HasError = false;
            }
            catch (EntityNotAvailableException e)
            {
                HandleEntityNotAvailableException(e, "Failed to mark fault because segment is not available in trace context.");
            }
        }

        /// <summary>
        /// Mark the current segment as error.
        /// </summary>
        /// <exception cref="EntityNotAvailableException">Entity is not available in trace context.</exception>
        public void MarkError()
        {
            try
            {
                Entity entity = TraceContext.GetEntity();
                entity.HasError = true;
                entity.HasFault = false;
            }
            catch (EntityNotAvailableException e)
            {
                HandleEntityNotAvailableException(e, "Failed to mark error because segment is not available in trace context.");
            }
        }

        /// <summary>
        /// Mark the current segment as being throttled. And Error will also be marked for current segment.
        /// </summary>
        /// <exception cref="EntityNotAvailableException">Entity is not available in trace context.</exception>
        public void MarkThrottle()
        {
            try
            {
                TraceContext.GetEntity().IsThrottled = true;
                MarkError();
            }
            catch (EntityNotAvailableException e)
            {
                HandleEntityNotAvailableException(e, "Failed to mark throttle because segment is not available in trace context.");
            }
        }

        /// <summary>
        /// Sets the daemon address for <see cref="Emitter"/>.
        /// A notation of '127.0.0.1:2000' or 'tcp:127.0.0.1:2000 udp:127.0.0.2:2001' or 
        ///'udp:127.0.0.1:2000 tcp:127.0.0.2:2001'
        /// are acceptable.The former one means UDP and TCP are running at
        /// the same address.
        /// If environment variable is set to specific daemon address, the call to this method
        /// will be ignored.
        /// </summary>
        /// <param name="daemonAddress">The daemon address.</param>
        public void SetDaemonAddress(string daemonAddress)
        {
            Emitter?.SetDaemonAddress(daemonAddress);
        }

        /// <summary>
        /// Free resources within the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Free resources within the object.
        /// </summary>
        /// <param name="disposing">To dispose or not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
                Emitter?.Dispose();

                Disposed = true;
            }
        }

        /// <summary>
        /// Sets segment IsInProgress to false and releases the segment.
        /// </summary>
        /// <param name="segment">Instance of <see cref="Segment"/>.</param>
        protected static void PrepEndSegment(Segment segment)
        {
            segment.IsInProgress = false;
            segment.Release();
        }

        /// <summary>
        /// If entity is not available in the <see cref="TraceContext"/>, exception is thrown.
        /// </summary>
        /// <param name="e">Instance of <see cref="EntityNotAvailableException"/>.</param>
        /// <param name="message">String message.</param>
        protected void HandleEntityNotAvailableException(EntityNotAvailableException e, string message)
        {
            TraceContext.HandleEntityMissing(this, e, message);
        }

        /// <summary>
        /// Gets entity (segment/subsegment) from the <see cref="TraceContext"/>.
        /// </summary>
        /// <returns>The entity (segment/subsegment)</returns>
        /// <exception cref="EntityNotAvailableException">Thrown when the entity is not available to get.</exception>
        public Entity GetEntity()
        {
            return TraceContext.GetEntity();
        }
    }
}
