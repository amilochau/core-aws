using Milochau.Core.Aws.Core.XRayRecorder.Core.Exceptions;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Context;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Emitters;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Sampling;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Sampling.Local;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Strategies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core
{
    /// <summary>
    /// This class provides utilities to build an instance of <see cref="AWSXRayRecorder"/> with different configurations.
    /// </summary>
    public abstract class AWSXRayRecorderImpl(ISegmentEmitter emitter) : IAWSXRayRecorder
    {
        protected const long MaxSubsegmentSize = 100;

        /// <summary>
        /// Gets or sets the sampling strategy.
        /// </summary>
        public LocalizedSamplingStrategy SamplingStrategy { get; set; } = new LocalizedSamplingStrategy();

        /// <summary>
        /// Instance of <see cref="LambdaContextContainer"/>, used to store segment/subsegment.
        /// </summary>
        public LambdaContextContainer TraceContext { get; set; } = new LambdaContextContainer();

        /// <summary>
        /// Emitter used to send Traces.
        /// </summary>
        public ISegmentEmitter Emitter { get; set; } = emitter;

        protected bool Disposed { get; set; }

        /// <summary>
        /// Defines exception serialization stategy to process recorded exceptions. <see cref="Strategies.ExceptionSerializationStrategy"/>
        /// </summary>
        public DefaultExceptionSerializationStrategy ExceptionSerializationStrategy { get; set; } = new DefaultExceptionSerializationStrategy();

        /// <summary>
        /// Instance of <see cref="DefaultStreamingStrategy"/>, used to define the streaming strategy for segment/subsegment.
        /// </summary>
        public DefaultStreamingStrategy StreamingStrategy { get; set; } = new DefaultStreamingStrategy();

        /// <summary>
        /// Begin a tracing subsegment. A new subsegment will be created and added as a subsegment to previous segment.
        /// </summary>
        /// <param name="name">Name of the operation.</param>
        /// <param name="timestamp">Sets the start time of the subsegment</param>
        /// <exception cref="ArgumentNullException">The argument has a null value.</exception>
        /// <exception cref="EntityNotAvailableException">Entity is not available in trace context.</exception>
        public abstract void BeginSubsegment(string name);

        /// <summary>
        /// End a subsegment.
        /// </summary>
        /// <param name="timestamp">Sets the end time for the subsegment</param>
        public abstract void EndSubsegment();

        /// <summary>
        /// Set namespace to current segment.
        /// </summary>
        /// <param name="value">The value of the namespace.</param>
        /// <exception cref="System.ArgumentException">Value cannot be null or empty.</exception>
        public void SetNamespace(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));
            }

            try
            {
                if (TraceContext.GetEntity() is not Subsegment subsegment)
                {
                    return;
                }

                subsegment.Namespace = value;
            }
            catch (EntityNotAvailableException)
            {
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
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            }

            ArgumentNullException.ThrowIfNull(value);

            try
            {
                TraceContext.GetEntity().Http[key] = value;
            }
            catch (EntityNotAvailableException)
            {
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
            catch (EntityNotAvailableException)
            {
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
            catch (EntityNotAvailableException)
            {
            }
        }

        /// <summary>
        /// Add the exception to current segment and also mark current segment as fault.
        /// </summary>
        /// <param name="ex">The exception to be added.</param>
        /// <exception cref="EntityNotAvailableException">Entity is not available in trace context.</exception>
        public void AddException(Exception ex)
        {
            try
            {
                TraceContext.GetEntity().AddException(ex);
            }
            catch (EntityNotAvailableException)
            {
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
            catch (EntityNotAvailableException)
            {
            }
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
        protected void PrepEndSegment(Segment segment)
        {
            segment.IsInProgress = false;
            segment.Release();
        }

        /// <summary>
        /// Trace a given function with return value. A subsegment will be created for this method.
        /// Any exception thrown by the method will be captured.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="name">The name of the trace subsegment for the method.</param>
        /// <param name="method">The method to be traced.</param>
        /// <returns>The return value of the given method.</returns>
        public TResult TraceMethod<TResult>(string name, Func<TResult> method)
        {
            BeginSubsegment(name);

            try
            {
                return method();
            }
            catch (Exception e)
            {
                AddException(e);
                throw;
            }

            finally
            {
                EndSubsegment();
            }
        }

        /// <summary>
        /// Trace a given method returns void.  A subsegment will be created for this method.
        /// Any exception thrown by the method will be captured.
        /// </summary>
        /// <param name="name">The name of the trace subsegment for the method.</param>
        /// <param name="method">The method to be traced.</param>
        public void TraceMethod(string name, Action method)
        {
            BeginSubsegment(name);

            try
            {
                method();
            }
            catch (Exception e)
            {
                AddException(e);
                throw;
            }

            finally
            {
                EndSubsegment();
            }
        }

        /// <summary>
        /// Trace a given asynchronous function with return value. A subsegment will be created for this method.
        /// Any exception thrown by the method will be captured.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates</typeparam>
        /// <param name="name">The name of the trace subsegment for the method</param>
        /// <param name="method">The method to be traced</param>
        /// <returns>The return value of the given method</returns>
        public async Task<TResult> TraceMethodAsync<TResult>(string name, Func<Task<TResult>> method)
        {
            BeginSubsegment(name);

            try
            {
                return await method();
            }
            catch (Exception e)
            {
                AddException(e);
                throw;
            }

            finally
            {
                EndSubsegment();
            }
        }

        /// <summary>
        /// Trace a given asynchronous method that returns no value.  A subsegment will be created for this method.
        /// Any exception thrown by the method will be captured.
        /// </summary>
        /// <param name="name">The name of the trace subsegment for the method</param>
        /// <param name="method">The method to be traced</param>
        public async Task TraceMethodAsync(string name, Func<Task> method)
        {
            BeginSubsegment(name);

            try
            {
                await method();
            }
            catch (Exception e)
            {
                AddException(e);
                throw;
            }

            finally
            {
                EndSubsegment();
            }
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

        /// <summary>
        /// Set the specified entity (segment/subsegment) into <see cref="TraceContext"/>.
        /// </summary>
        /// <param name="entity">The entity to be set</param>
        /// <exception cref="EntityNotAvailableException">Thrown when the entity is not available to set</exception>
        public void SetEntity(Entity entity)
        {
            TraceContext.SetEntity(entity);
        }

        /// <summary>
        /// Checks whether entity is present in <see cref="TraceContext"/>.
        /// </summary>
        /// <returns>True if entity is present TraceContext else false.</returns>
        public bool IsEntityPresent()
        {
            return TraceContext.IsEntityPresent();
        }

        /// <summary>
        /// Clear entity from <see cref="TraceContext"/>.
        /// </summary>
        public void ClearEntity()
        {
            TraceContext.ClearEntity();
        }

        /// <summary>
        /// Configures recorder with <see cref="Strategies.ExceptionSerializationStrategy"/>.  While setting number consider max trace size
        /// limit : https://aws.amazon.com/xray/pricing/
        /// </summary>
        /// <param name="exceptionSerializationStartegy">An instance of <see cref="ExceptionSerializationStrategy"/></param>
        public void SetExceptionSerializationStrategy(DefaultExceptionSerializationStrategy exceptionSerializationStartegy)
        {
            ExceptionSerializationStrategy = exceptionSerializationStartegy;
        }
    }
}
