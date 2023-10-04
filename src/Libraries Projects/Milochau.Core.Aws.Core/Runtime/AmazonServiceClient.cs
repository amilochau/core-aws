/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 * 
 *  http://aws.amazon.com/apache2.0
 * 
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.Internal.Transform;
using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Threading;
using Amazon.Util.Internal;
using ExecutionContext = Amazon.Runtime.Internal.ExecutionContext;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;

namespace Amazon.Runtime
{
    public abstract class AmazonServiceClient : IDisposable
    {
        private bool _disposed;
        private Logger _logger;
        protected EndpointDiscoveryResolverBase EndpointDiscoveryResolver { get; private set; }
        protected RuntimePipeline RuntimePipeline { get; set; }
        protected internal AWSCredentials Credentials { get; private set; }
        public IClientConfig Config => _config;
        private readonly ClientConfig _config;
        protected virtual IServiceMetadata ServiceMetadata { get; } = new ServiceMetadata();
        protected virtual bool SupportResponseLogging
        {
            get { return true; }
        }

        #region Events


        private PreRequestEventHandler mBeforeMarshallingEvent;

        /// <summary>
        /// Occurs before a request is marshalled.
        /// </summary>
        internal event PreRequestEventHandler BeforeMarshallingEvent
        {
            add
            {
                lock (this)
                {
                    mBeforeMarshallingEvent += value;
                }
            }
            remove
            {
                lock (this)
                {
                    mBeforeMarshallingEvent -= value;
                }
            }
        }


        private RequestEventHandler mBeforeRequestEvent;

        /// <summary>
        /// Occurs before a request is issued against the service.
        /// </summary>
        public event RequestEventHandler BeforeRequestEvent
        {
            add
            {
                lock (this)
                {
                    mBeforeRequestEvent += value;
                }
            }
            remove
            {
                lock (this)
                {
                    mBeforeRequestEvent -= value;
                }
            }
        }

        private ResponseEventHandler mAfterResponseEvent;

        /// <summary>
        /// Occurs after a response is received from the service.
        /// </summary>
        public event ResponseEventHandler AfterResponseEvent
        {
            add
            {
                lock (this)
                {
                    mAfterResponseEvent += value;
                }
            }
            remove
            {
                lock (this)
                {
                    mAfterResponseEvent -= value;
                }
            }
        }

        private ExceptionEventHandler mExceptionEvent;

        /// <summary>
        /// Occurs after an exception is encountered.
        /// </summary>
        public event ExceptionEventHandler ExceptionEvent
        {
            add
            {
                lock (this)
                {
                    mExceptionEvent += value;
                }
            }
            remove
            {
                lock (this)
                {
                    mExceptionEvent -= value;
                }
            }
        }

        #endregion

        #region Constructors

        protected AmazonServiceClient(AWSCredentials credentials, ClientConfig config)
        {
            if (config.DisableLogging)
                _logger = Logger.EmptyLogger;
            else
                _logger = Logger.GetLogger(GetType());

            config.Validate();
            Credentials = credentials;
            _config = config;
            Signer = CreateSigner();
            EndpointDiscoveryResolver = new EndpointDiscoveryResolver(config, _logger);
            Initialize();
            BuildRuntimePipeline();
        }

        protected AbstractAWSSigner Signer
        {
            get;
            private set;
        }

        protected virtual void Initialize()
        {
        }

        #endregion

        #region Invoke methods

        protected TResponse Invoke<TResponse>(AmazonWebServiceRequest request, InvokeOptionsBase options)
            where TResponse : AmazonWebServiceResponse
        {
            ThrowIfDisposed();

            var executionContext = new ExecutionContext(
                new RequestContext(Config.LogMetrics, Signer)
                {
                    ClientConfig = Config,
                    Marshaller = options.RequestMarshaller,
                    OriginalRequest = request,
                    Unmarshaller = options.ResponseUnmarshaller,
                    IsAsync = false,
                    ServiceMetaData = ServiceMetadata,
                    Options = options
                },
                new ResponseContext()
            );
            SetupCSMHandler(executionContext.RequestContext);
            var response = (TResponse)RuntimePipeline.InvokeSync(executionContext).Response;
            return response;
        }

        protected System.Threading.Tasks.Task<TResponse> InvokeAsync<TResponse>(
            AmazonWebServiceRequest request,
            InvokeOptionsBase options,
            System.Threading.CancellationToken cancellationToken)
            where TResponse : AmazonWebServiceResponse, new()
        {
            ThrowIfDisposed();

            if (cancellationToken == default(CancellationToken))
                cancellationToken = _config.BuildDefaultCancellationToken();

            var executionContext = new ExecutionContext(
                new RequestContext(this.Config.LogMetrics, Signer)
                {
                    ClientConfig = this.Config,
                    Marshaller = options.RequestMarshaller,
                    OriginalRequest = request,
                    Unmarshaller = options.ResponseUnmarshaller,
                    IsAsync = true,
                    CancellationToken = cancellationToken,
                    ServiceMetaData = this.ServiceMetadata,
                    Options = options
                },
                new ResponseContext()
            );
            SetupCSMHandler(executionContext.RequestContext);
            return this.RuntimePipeline.InvokeAsync<TResponse>(executionContext);
        }

        protected virtual IEnumerable<DiscoveryEndpointBase> EndpointOperation(EndpointOperationContextBase context) { return null; }

        #endregion

        #region Process Handlers

        protected void ProcessPreRequestHandlers(IExecutionContext executionContext)
        {
            //if (request == null)
            //    return;
            if (mBeforeMarshallingEvent == null)
                return;

            PreRequestEventArgs args = PreRequestEventArgs.Create(executionContext.RequestContext.OriginalRequest);
            mBeforeMarshallingEvent(this, args);
        }

        protected void ProcessRequestHandlers(IExecutionContext executionContext)
        {
            var request = executionContext.RequestContext.Request;
            WebServiceRequestEventArgs args = WebServiceRequestEventArgs.Create(request);

            if (request.OriginalRequest != null)
                request.OriginalRequest.FireBeforeRequestEvent(this, args);

            if (mBeforeRequestEvent != null)
                mBeforeRequestEvent(this, args);
        }

        protected void ProcessResponseHandlers(IExecutionContext executionContext)
        {
            if (mAfterResponseEvent == null)
                return;

            WebServiceResponseEventArgs args = WebServiceResponseEventArgs.Create(
                executionContext.ResponseContext.Response,
                executionContext.RequestContext.Request,
                executionContext.ResponseContext.HttpResponse);

            mAfterResponseEvent(this, args);
        }

        protected virtual void ProcessExceptionHandlers(IExecutionContext executionContext, Exception exception)
        {
            if (mExceptionEvent == null)
                return;

            WebServiceExceptionEventArgs args = WebServiceExceptionEventArgs.Create(exception, executionContext.RequestContext.Request);
            mExceptionEvent(this, args);
        }

        #endregion

        #region Dispose methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (RuntimePipeline != null)
                    RuntimePipeline.Dispose();

                _disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        #endregion

        protected abstract AbstractAWSSigner CreateSigner();
        protected virtual void CustomizeRuntimePipeline(RuntimePipeline pipeline) { }

        private void BuildRuntimePipeline()
        {
            var httpRequestFactory = new HttpRequestMessageFactory(Config);
            var httpHandler = new HttpHandler<System.Net.Http.HttpContent>(httpRequestFactory, this);
            var preMarshallHandler = new CallbackHandler();
            preMarshallHandler.OnPreInvoke = this.ProcessPreRequestHandlers;

            var postMarshallHandler = new CallbackHandler();
            postMarshallHandler.OnPreInvoke = this.ProcessRequestHandlers;

            var postUnmarshallHandler = new CallbackHandler();
            postUnmarshallHandler.OnPostInvoke = this.ProcessResponseHandlers;

            var errorCallbackHandler = new ErrorCallbackHandler();
            errorCallbackHandler.OnError = this.ProcessExceptionHandlers;

            //Determine which retry policy to use based on the retry mode
            RetryPolicy retryPolicy;
            switch (Config.RetryMode)
            {
                case RequestRetryMode.Adaptive:
                    retryPolicy = new AdaptiveRetryPolicy(Config);
                    break;
                case RequestRetryMode.Standard:
                    retryPolicy = new StandardRetryPolicy(Config);
                    break;
                case RequestRetryMode.Legacy:
                    retryPolicy = new DefaultRetryPolicy(Config);
                    break;
                default:
                    throw new InvalidOperationException("Unknown retry mode");
            }

            // Build default runtime pipeline.
            RuntimePipeline = new RuntimePipeline(new List<IPipelineHandler>
                {
                    httpHandler,
                    new Unmarshaller(SupportResponseLogging),
                    new ErrorHandler(_logger),
                    postUnmarshallHandler,
                    new Signer(),
                    // EndpointDiscoveryResolver must come after CredentialsRetriever, RetryHander, and EndpointResolver as it depends on
                    // credentials, retrying of requests for 421 web exceptions, and the current set regional endpoint.
                    new EndpointDiscoveryHandler(),
                    // ChecksumHandler must come after CompressionHandler because we must calculate the checksum of a payload after compression.
                    // ChecksumHandler must come after EndpointsResolver because of an upcoming project.
                    new ChecksumHandler(),
                    // CredentialsRetriever must come after RetryHandler because of any credential related changes.
                    new CredentialsRetriever(Credentials),
                    new RetryHandler(retryPolicy),
                    new CompressionHandler(),
                    postMarshallHandler,
                    // EndpointResolver must come after CredentialsRetriever in an upcoming endpoint project.
                    new EndpointResolver(),
                    new Marshaller(),
                    preMarshallHandler,
                    errorCallbackHandler,
                    new MetricsHandler()
                },
                _logger
            );

            if (DeterminedCSMConfiguration.Instance.CSMConfiguration.Enabled && !string.IsNullOrEmpty(ServiceMetadata.ServiceId))
            {
                RuntimePipeline.AddHandlerBefore<ErrorHandler>(new CSMCallAttemptHandler());
                RuntimePipeline.AddHandlerBefore<MetricsHandler>(new CSMCallEventHandler());
            }

            CustomizeRuntimePipeline(RuntimePipeline);

            // Apply global pipeline customizations
            RuntimePipelineCustomizerRegistry.Instance.ApplyCustomizations(GetType(), RuntimePipeline);
        }

        /// <summary>
        /// Assembles the Uri for a given SDK request
        /// </summary>
        /// <param name="iRequest">Request to compute Uri for</param>
        /// <returns>Uri for the given SDK request</returns>
        public static Uri ComposeUrl(IRequest iRequest)
        {
            return ComposeUrl(iRequest, true);
        }

        /// <summary>
        /// Assembles the Uri for a given SDK request
        /// </summary>
        /// <param name="internalRequest">Request to compute Uri for</param>
        /// <param name="skipEncodingValidPathChars">If true the accepted path characters {/+:} are not encoded.</param>
        /// <returns>Uri for the given SDK request</returns>
        public static Uri ComposeUrl(IRequest internalRequest, bool skipEncodingValidPathChars)
        {
            Uri url = internalRequest.Endpoint;
            var resourcePath = internalRequest.ResourcePath;
            if (resourcePath == null)
                resourcePath = string.Empty;
            else
            {
                if (resourcePath.StartsWith("/", StringComparison.Ordinal))
                    resourcePath = resourcePath.Substring(1);

                // Microsoft added support for unicode bidi control characters to the Uri class in .NET 4.7.2
                // https://github.com/microsoft/dotnet/blob/master/Documentation/compatibility/uri-unicode-bidirectional-characters.md
                // However, we only want to support it on .NET Core 3.1 and higher due to not having to deal with .NET Standard support matrix.
                if (AWSSDKUtils.HasBidiControlCharacters(resourcePath) ||
                    (internalRequest.PathResources?.Any(v => AWSSDKUtils.HasBidiControlCharacters(v.Value)) == true))
                {
                    resourcePath = string.Join("/", AWSSDKUtils.SplitResourcePathIntoSegments(resourcePath, internalRequest.PathResources).ToArray());
                    throw new AmazonClientException(string.Format(CultureInfo.InvariantCulture,
                        "Target resource path [{0}] has bidirectional characters, which are not supported" +
                        "by System.Uri and thus cannot be handled by the .NET SDK.", resourcePath));
                }

                resourcePath = AWSSDKUtils.ResolveResourcePath(resourcePath, internalRequest.PathResources, skipEncodingValidPathChars);
            }

            // Construct any sub resource/query parameter additions to append to the
            // resource path. Services like S3 which allow '?' and/or '&' in resource paths 
            // should use SubResources instead of appending them to the resource path with 
            // query string delimiters during request marshalling.

            var delim = "?";
            var sb = new StringBuilder();

            if (internalRequest.SubResources?.Count > 0)
            {
                foreach (var subResource in internalRequest.SubResources)
                {
                    sb.AppendFormat("{0}{1}", delim, subResource.Key);
                    if (subResource.Value != null)
                        sb.AppendFormat("={0}", subResource.Value);
                    delim = "&";
                }
            }

            if (internalRequest.UseQueryString && internalRequest.Parameters?.Count > 0)
            {
                var queryString = AWSSDKUtils.GetParametersAsString(internalRequest);
                sb.AppendFormat("{0}{1}", delim, queryString);
            }

            var parameterizedPath = string.Empty;
            if (internalRequest.MarshallerVersion >= 2)
            {
                parameterizedPath = string.Concat(resourcePath, sb);
            }
            else
            {
                if (AWSSDKUtils.HasBidiControlCharacters(resourcePath))
                    throw new AmazonClientException(string.Format(CultureInfo.InvariantCulture,
                        "Target resource path [{0}] has bidirectional characters, which are not supported" +
                        "by System.Uri and thus cannot be handled by the .NET SDK.", resourcePath));

                parameterizedPath = string.Concat(AWSSDKUtils.ProtectEncodedSlashUrlEncode(resourcePath, skipEncodingValidPathChars), sb);
            }

            var hasSlash = url.AbsoluteUri.EndsWith("/", StringComparison.Ordinal) || parameterizedPath.StartsWith("/", StringComparison.Ordinal);
            var uri = hasSlash
                ? new Uri(url.AbsoluteUri + parameterizedPath)
                : new Uri(url.AbsoluteUri + "/" + parameterizedPath);
            return uri;
        }

        /// <summary>
        /// Used to create a copy of the config for a different service than the current instance.
        /// </summary>
        /// <typeparam name="C">Target service ClientConfig</typeparam>
        /// <returns>The new ClientConfig for the desired service</returns>
        internal C CloneConfig<C>()
            where C : ClientConfig, new()
        {
            var config = new C();
            CloneConfig(config);
            return config;
        }

        internal void CloneConfig(ClientConfig newConfig)
        {
            if (!string.IsNullOrEmpty(Config.ServiceURL))
            {
                var regionName = Util.AWSSDKUtils.DetermineRegion(Config.ServiceURL);
                RegionEndpoint region = RegionEndpoint.GetBySystemName(regionName);
                newConfig.RegionEndpoint = region;
            }
            else
            {
                newConfig.RegionEndpoint = Config.RegionEndpoint;
            }

            newConfig.UseHttp = Config.UseHttp;


            newConfig.ProxyCredentials = Config.ProxyCredentials;
            newConfig.ProxyHost = Config.ProxyHost;
            newConfig.ProxyPort = Config.ProxyPort;
        }

        private static void SetupCSMHandler(IRequestContext requestContext)
        {
            if (requestContext.CSMEnabled)
            {
                requestContext.CSMCallEvent = new MonitoringAPICallEvent(requestContext);
            }
        }
    }
}
