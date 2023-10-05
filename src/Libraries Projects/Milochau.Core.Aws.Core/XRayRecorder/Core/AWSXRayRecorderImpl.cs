﻿//-----------------------------------------------------------------------------
// <copyright file="AWSXRayRecorderImpl.cs" company="Amazon.com">
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Amazon.Runtime.Internal.Util;
using Amazon.XRay.Recorder.Core.Exceptions;
using Amazon.XRay.Recorder.Core.Internal.Context;
using Amazon.XRay.Recorder.Core.Internal.Emitters;
using Amazon.XRay.Recorder.Core.Internal.Entities;
using Amazon.XRay.Recorder.Core.Internal.Utils;
using Amazon.XRay.Recorder.Core.Sampling;
using Amazon.XRay.Recorder.Core.Strategies;

namespace Amazon.XRay.Recorder.Core
{
    /// <summary>
    /// This class provides utilities to build an instance of <see cref="AWSXRayRecorder"/> with different configurations.
    /// </summary>
    public abstract class AWSXRayRecorderImpl : IAWSXRayRecorder
    {
        private static readonly Logger _logger = Logger.GetLogger(typeof(AWSXRayRecorderImpl));

        /// <summary>
        /// The environment variable that setting context missing strategy.
        /// </summary>
        public const string EnvironmentVariableContextMissingStrategy = "AWS_XRAY_CONTEXT_MISSING";

        protected const long MaxSubsegmentSize = 100;

        private ISegmentEmitter _emitter;
        private bool disposed;
        protected ContextMissingStrategy cntxtMissingStrategy = ContextMissingStrategy.LOG_ERROR;
        private Dictionary<string, object> serviceContext = new Dictionary<string, object>();

        protected AWSXRayRecorderImpl(ISegmentEmitter emitter)
        {
            this._emitter = emitter;
        }

        /// <summary>
        /// Gets or sets the origin of the service.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Gets or sets the sampling strategy.
        /// </summary>
        public ISamplingStrategy SamplingStrategy { get; set; }

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
                _logger.DebugFormat(string.Format("Context missing mode : {0}", cntxtMissingStrategy));
                string modeFromEnvironmentVariable = Environment.GetEnvironmentVariable(EnvironmentVariableContextMissingStrategy);
                if (string.IsNullOrEmpty(modeFromEnvironmentVariable))
                {
                    _logger.DebugFormat(string.Format("{0} environment variable is not set. Do not override context missing mode.", EnvironmentVariableContextMissingStrategy));
                }
                else if (modeFromEnvironmentVariable.Equals(ContextMissingStrategy.LOG_ERROR.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    _logger.DebugFormat(string.Format("{0} environment variable is set to {1}. Override local value.", EnvironmentVariableContextMissingStrategy, modeFromEnvironmentVariable));
                    cntxtMissingStrategy = ContextMissingStrategy.LOG_ERROR;
                }
                else if (modeFromEnvironmentVariable.Equals(ContextMissingStrategy.RUNTIME_ERROR.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    _logger.DebugFormat(string.Format("{0} environment variable is set to {1}. Override local value.", EnvironmentVariableContextMissingStrategy, modeFromEnvironmentVariable));
                    cntxtMissingStrategy = ContextMissingStrategy.RUNTIME_ERROR;
                }
            }
        }

        /// <summary>
        /// Instance of <see cref="ITraceContext"/>, used to store segment/subsegment.
        /// </summary>
        public ITraceContext TraceContext { get; set; } = DefaultTraceContext.GetTraceContext();

        /// <summary>
        /// Gets the runtime context which is generated by plugins.
        /// </summary>
        public IDictionary<string, object> RuntimeContext { get; protected set; }

        /// <summary>
        /// Emitter used to send Traces.
        /// </summary>
        public ISegmentEmitter Emitter { get => _emitter; set => _emitter = value; }

        protected bool Disposed { get => disposed; set => disposed = value; }

        protected Dictionary<string, object> ServiceContext { get => serviceContext; set => serviceContext = value; }

        /// <summary>
        /// Defines exception serialization stategy to process recorded exceptions. <see cref="Strategies.ExceptionSerializationStrategy"/>
        /// </summary>
        public ExceptionSerializationStrategy ExceptionSerializationStrategy { get; set; } = new DefaultExceptionSerializationStrategy();

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
        /// Checks whether Tracing is enabled or disabled.
        /// </summary>
        /// <returns> Returns true if Tracing is disabled else false.</returns>
        public abstract Boolean IsTracingDisabled();

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

            if (IsTracingDisabled())
            {
                _logger.DebugFormat("X-Ray tracing is disabled, do not set namespace.");
                return;
            }

            try
            {
                var subsegment = TraceContext.GetEntity() as Subsegment;

                if (subsegment == null)
                {
                    _logger.DebugFormat("Failed to cast the entity from TraceContext to Subsegment. SetNamespace is only available to Subsegment.");
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
            if (IsTracingDisabled())
            {
                _logger.DebugFormat("X-Ray tracing is disabled, do not add http information.");
                return;
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            try
            {
                TraceContext.GetEntity().Http[key] = value;
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
            if (IsTracingDisabled())
            {
                _logger.DebugFormat("X-Ray tracing is disabled, do not mark fault.");
                return;
            }

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
            if (IsTracingDisabled())
            {
                _logger.DebugFormat("X-Ray tracing is disabled, do not mark error.");
                return;
            }

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
            if (IsTracingDisabled())
            {
                _logger.DebugFormat("X-Ray tracing is disabled, do not mark throttle.");
                return;
            }

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
        /// Sets the daemon address for <see cref="Emitter"/> and <see cref="DefaultSamplingStrategy"/> if set.
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
            if (Emitter != null)
            {
                Emitter.SetDaemonAddress(daemonAddress);
            }

            if (SamplingStrategy != null && SamplingStrategy.GetType().Equals(typeof(DefaultSamplingStrategy)))
            {
                DefaultSamplingStrategy defaultSampler = (DefaultSamplingStrategy)SamplingStrategy;
                defaultSampler.LoadDaemonConfig(DaemonConfig.GetEndPoint(daemonAddress));
            }
        }

        /// <summary>
        /// Configures recorder instance with <see cref="ITraceContext"/>.
        /// </summary>
        /// <param name="traceContext">Instance of <see cref="ITraceContext"/></param>
        public void SetTraceContext(ITraceContext traceContext)
        {
            if (traceContext != null)
            {
                TraceContext = traceContext;
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
                if (Emitter != null)
                {
                    Emitter.Dispose();
                }

                Disposed = true;
            }
        }

        /// <summary>
        /// Populates runtime and service contexts.
        /// </summary>
        protected void PopulateContexts()
        {
            RuntimeContext = new Dictionary<string, object>();

            // Prepare XRay section for runtime context
            var xrayContext = new ConcurrentDictionary<string, string>();

            xrayContext["sdk"] = "X-Ray for .NET Core";
            string currentAssemblyLocation = Assembly.GetExecutingAssembly().Location;
            if (!string.IsNullOrEmpty(currentAssemblyLocation))
            {
                xrayContext["sdk_version"] = FileVersionInfo.GetVersionInfo(currentAssemblyLocation).ProductVersion;
            }
            else
            {
                xrayContext["sdk_version"] = "Unknown";
            }

            RuntimeContext["xray"] = xrayContext;
            ServiceContext["runtime"] = ".NET Core Framework";
            ServiceContext["runtime_version"] = Environment.Version.ToString();
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

        /// <summary>
        /// Configures recorder with <see cref="Strategies.ExceptionSerializationStrategy"/>.  While setting number consider max trace size
        /// limit : https://aws.amazon.com/xray/pricing/
        /// </summary>
        /// <param name="exceptionSerializationStartegy">An instance of <see cref="ExceptionSerializationStrategy"/></param>
        public void SetExceptionSerializationStrategy(ExceptionSerializationStrategy exceptionSerializationStartegy)
        {
            this.ExceptionSerializationStrategy = exceptionSerializationStartegy;
        }
    }
}
