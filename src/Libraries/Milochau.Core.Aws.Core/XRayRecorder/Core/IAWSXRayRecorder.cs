﻿using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Context;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Emitters;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Sampling.Local;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Strategies;
using System;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core
{
    /// <summary>
    /// Interface to record tracing information for AWS X-Ray
    /// </summary>
    public interface IAWSXRayRecorder : IDisposable
    {
        /// <summary>
        /// Gets or sets the sampling strategy
        /// </summary>
        LocalizedSamplingStrategy SamplingStrategy { get; set; }

        /// <summary>
        /// Get or sets the streaming strategy
        /// </summary>
        DefaultStreamingStrategy StreamingStrategy { get; set; }

        /// <summary>
        /// Defines exception serialization stategy to process recorded exceptions. <see cref="Strategies.ExceptionSerializationStrategy"/>
        /// </summary>
        DefaultExceptionSerializationStrategy ExceptionSerializationStrategy { get; set; }

        /// <summary>
        /// Instance of <see cref="LambdaContextContainer"/>, used to store segment/subsegment.
        /// </summary>
        LambdaContextContainer TraceContext { get; set; }

        /// <summary>
        /// Emitter used to send Traces.
        /// </summary>
        ISegmentEmitter Emitter { get; set; }

        /// <summary>
        /// Start a subsegment with a given name and optional creation timestamp
        /// </summary>
        /// <param name="name">Name of the subsegment</param>
        /// <param name="timestamp">Sets the start time for the subsegment</param>
        void BeginSubsegment(string name);

        /// <summary>
        /// End a subsegment
        /// </summary>
        /// <param name="timestamp">Sets the end time for the subsegment</param>
        void EndSubsegment();

        /// <summary>
        /// Set namespace to current segment
        /// </summary>
        /// <param name="value">The value of the namespace</param>
        void SetNamespace(string value);

        /// <summary>
        /// Mark the current segment as fault.
        /// </summary>
        void MarkFault();

        /// <summary>
        /// Mark the current segment as error.
        /// </summary>
        void MarkError();

        /// <summary>
        /// Add the exception to current segment
        /// </summary>
        /// <param name="ex">The exception to be added.</param>
        void AddException(Exception ex);

        /// <summary>
        /// Trace a given method with return value. 
        /// </summary>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates</typeparam>
        /// <param name="name">The name of the trace subsegment for the method</param>
        /// <param name="method">The method to be traced</param>
        /// <returns>The return value of the given method</returns>
        TResult TraceMethod<TResult>(string name, Func<TResult> method);

        /// <summary>
        /// Trace a given method returns void.
        /// </summary>
        /// <param name="name">The name of the trace subsegment for the method</param>
        /// <param name="method">The method to be traced</param>
        void TraceMethod(string name, Action method);

        /// <summary>
        /// Trace a given asynchronous function with return value. A subsegment will be created for this method.
        /// Any exception thrown by the method will be captured.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates</typeparam>
        /// <param name="name">The name of the trace subsegment for the method</param>
        /// <param name="method">The method to be traced</param>
        /// <returns>The return value of the given method</returns>
        Task<TResult> TraceMethodAsync<TResult>(string name, Func<Task<TResult>> method);

        /// <summary>
        /// Trace a given asynchronous method that returns no value.  A subsegment will be created for this method.
        /// Any exception thrown by the method will be captured.
        /// </summary>
        /// <param name="name">The name of the trace subsegment for the method</param>
        /// <param name="method">The method to be traced</param>
        Task TraceMethodAsync(string name, Func<Task> method);

        /// <summary>
        /// Adds the specified key and value as http information to current segment
        /// </summary>
        /// <param name="key">The key of the http information to add</param>
        /// <param name="value">The value of the http information to add</param>
        void AddHttpInformation(string key, object value);

        /// <summary>
        /// Mark the current segment as being throttled.
        /// </summary>
        void MarkThrottle();
    }
}
