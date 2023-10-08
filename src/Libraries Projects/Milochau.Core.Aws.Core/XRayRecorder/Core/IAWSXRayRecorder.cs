﻿//-----------------------------------------------------------------------------
// <copyright file="IAWSXRayRecorder.cs" company="Amazon.com">
//      Copyright 2016 Amazon.com, Inc. or its affiliates. All Rights Reserved.
//
//      Licensed under the Apache License, Version 2.0 (the "License").
//      You may not use this file except in compliance with the License.
//      A copy of the License is located at
//
//      http://aws.amazon.com/apache2.0
//
//      or in the "license" file accompanying this file. This file is distributed
//      on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
//      express or implied. See the License for the specific language governing
//      permissions and limitations under the License.
// </copyright>
//-----------------------------------------------------------------------------

using System;
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
