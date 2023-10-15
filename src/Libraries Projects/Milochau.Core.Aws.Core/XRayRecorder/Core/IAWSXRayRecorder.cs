using System;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Strategies;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Context;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Emitters;
using System.Collections.Generic;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core
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
        /// Defines exception serialization stategy to process recorded exceptions. <see cref="IExceptionSerializationStrategy"/>
        /// </summary>
        IExceptionSerializationStrategy ExceptionSerializationStrategy { get; set; }
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
        void AddHttpInformation(string key, Dictionary<string, long> value);

        /// <summary>
        /// Mark the current segment as being throttled.
        /// </summary>
        void MarkThrottle();
    }
}
