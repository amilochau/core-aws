﻿using System;
using Amazon.XRay.Recorder.Core.Strategies;
using Amazon.XRay.Recorder.Core.Internal.Context;
using Amazon.XRay.Recorder.Core.Internal.Emitters;

namespace Amazon.XRay.Recorder.Core
{
    /// <summary>
    /// Interface to record tracing information for AWS X-Ray
    /// </summary>
    public interface IAWSXRayRecorder : IDisposable
    {
        /// <summary>
        /// Get or sets the streaming strategy
        /// </summary>
        IStreamingStrategy StreamingStrategy { get; set; }

        /// <summary>
        /// Gets or sets the context missing strategy.
        /// </summary>
        ContextMissingStrategy ContextMissingStrategy { get; set; }

        /// <summary>
        /// Defines exception serialization stategy to process recorded exceptions. <see cref="Strategies.ExceptionSerializationStrategy"/>
        /// </summary>
        ExceptionSerializationStrategy ExceptionSerializationStrategy { get; set; }
        /// <summary>
        /// Instance of <see cref="ITraceContext"/>, used to store segment/subsegment.
        /// </summary>
        ITraceContext TraceContext { get; set; }

        /// <summary>
        /// Emitter used to send Traces.
        /// </summary>
        ISegmentEmitter Emitter { get; set; }

        /// <summary>
        /// Start a subsegment with a given name and optional creation timestamp
        /// </summary>
        /// <param name="name">Name of the subsegment</param>
        /// <param name="timestamp">Sets the start time for the subsegment</param>
        void BeginSubsegment(string name, DateTime? timestamp = null);

        /// <summary>
        /// End a subsegment
        /// </summary>
        /// <param name="timestamp">Sets the end time for the subsegment</param>
        void EndSubsegment(DateTime? timestamp = null);

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
        /// Adds the specified key and value as http information to current segment
        /// </summary>
        /// <param name="key">The key of the http information to add</param>
        /// <param name="value">The value of the http information to add</param>
        void AddHttpInformation(string key, object value);

        /// <summary>
        /// Mark the current segment as being throttled.
        /// </summary>
        void MarkThrottle();

        /// <summary>
        /// Sets the daemon address.
        /// The daemon address should be in format "IPAddress:Port", i.e. "127.0.0.1:2000".
        /// If environment variable is set to specific daemon address, the call to this method
        /// will be ignored.
        /// </summary>
        /// <param name="daemonAddress">The daemon address.</param>
        void SetDaemonAddress(string daemonAddress);
    }
}
