﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Core/Amazon.Runtime/AmazonServiceClient.cs
namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary></summary>
    public abstract class AmazonServiceClient : IDisposable
    {
        private static volatile bool _isProtocolUpdated;

        private bool _disposed;
        private Logger _logger;
        protected EndpointDiscoveryResolverBase EndpointDiscoveryResolver { get; private set; }
        protected RuntimePipeline RuntimePipeline { get; set; }
        protected internal AWSCredentials Credentials { get; private set; }

        /// <summary></summary>
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
                _logger = Logger.GetLogger(this.GetType());

            config.Validate();
            this.Credentials = credentials;
            _config = config;
            Signer = CreateSigner();
            EndpointDiscoveryResolver = new EndpointDiscoveryResolver(config, _logger);
            Initialize();
            UpdateSecurityProtocol();
            BuildRuntimePipeline();
        }

        protected AbstractAWSSigner Signer
        {
            get;
            private set;
        }

        protected AmazonServiceClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, ClientConfig config)
            : this(new SessionAWSCredentials(awsAccessKeyId, awsSecretAccessKey, awsSessionToken), config)
        {
        }

        protected AmazonServiceClient(string awsAccessKeyId, string awsSecretAccessKey, ClientConfig config)
            : this(new BasicAWSCredentials(awsAccessKeyId, awsSecretAccessKey), config)
        {
        }

        protected virtual void Initialize()
        {
        }

        #endregion

        #region Invoke methods

        [Obsolete("Invoke taking marshallers is obsolete. Use Invoke taking InvokeOptionsBase instead.")]
        protected TResponse Invoke<TRequest, TResponse>(TRequest request,
            IMarshaller<IRequest, AmazonWebServiceRequest> marshaller, ResponseUnmarshaller unmarshaller)
            where TRequest : AmazonWebServiceRequest
            where TResponse : AmazonWebServiceResponse
        {
            var options = new InvokeOptions();
            options.RequestMarshaller = marshaller;
            options.ResponseUnmarshaller = unmarshaller;
            return Invoke<TResponse>(request, options);
        }

        protected TResponse Invoke<TResponse>(AmazonWebServiceRequest request, InvokeOptionsBase options)
            where TResponse : AmazonWebServiceResponse
        {
            ThrowIfDisposed();

            var executionContext = new ExecutionContext(
                new RequestContext(this.Config.LogMetrics, Signer)
                {
                    ClientConfig = this.Config,
                    Marshaller = options.RequestMarshaller,
                    OriginalRequest = request,
                    Unmarshaller = options.ResponseUnmarshaller,
                    IsAsync = false,
                    ServiceMetaData = this.ServiceMetadata,
                    Options = options
                },
                new ResponseContext()
            );
            SetupCSMHandler(executionContext.RequestContext);
            var response = (TResponse)this.RuntimePipeline.InvokeSync(executionContext).Response;
            return response;
        }

        [Obsolete("InvokeAsync taking marshallers is obsolete. Use InvokeAsync taking InvokeOptionsBase instead.")]
        protected System.Threading.Tasks.Task<TResponse> InvokeAsync<TRequest, TResponse>(
            TRequest request, 
            IMarshaller<IRequest, AmazonWebServiceRequest> marshaller,
            ResponseUnmarshaller unmarshaller,
            System.Threading.CancellationToken cancellationToken)
            where TRequest: AmazonWebServiceRequest
            where TResponse : AmazonWebServiceResponse, new()
        {
            var options = new InvokeOptions();
            options.RequestMarshaller = marshaller;
            options.ResponseUnmarshaller = unmarshaller;
            return InvokeAsync<TResponse>(request, options, cancellationToken);
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
            if (this._disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        #endregion

        protected abstract AbstractAWSSigner CreateSigner();
        protected virtual void CustomizeRuntimePipeline(RuntimePipeline pipeline) { }

        private void BuildRuntimePipeline()
        {
            var httpRequestFactory = new HttpRequestMessageFactory(this.Config);
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
            switch (this.Config.RetryMode)
            {
                case RequestRetryMode.Adaptive:
                    retryPolicy = new AdaptiveRetryPolicy(this.Config);
                    break;
                case RequestRetryMode.Standard:
                    retryPolicy = new StandardRetryPolicy(this.Config);
                    break;
                case RequestRetryMode.Legacy:
                    retryPolicy = new DefaultRetryPolicy(this.Config);
                    break;
                default:
                    throw new InvalidOperationException("Unknown retry mode");
            }

            // Build default runtime pipeline.
            this.RuntimePipeline = new RuntimePipeline(new List<IPipelineHandler>
                {
                    httpHandler,
                    new Unmarshaller(this.SupportResponseLogging),
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
                    new CredentialsRetriever(this.Credentials),
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
                this.RuntimePipeline.AddHandlerBefore<ErrorHandler>(new CSMCallAttemptHandler());
                this.RuntimePipeline.AddHandlerBefore<MetricsHandler>(new CSMCallEventHandler());
            }

            CustomizeRuntimePipeline(this.RuntimePipeline);

            // Apply global pipeline customizations
            RuntimePipelineCustomizerRegistry.Instance.ApplyCustomizations(this.GetType(), this.RuntimePipeline);
        }

        /// <summary>
        /// Some AWS services like Cloud 9 require at least TLS 1.1. Version of .NET Framework 4.5 and earlier 
        /// do not eanble TLS 1.1 and TLS 1.2 by default. This code adds those protocols if using an earlier 
        /// version of .NET that explicitly set the protocol and didn't have TLS 1.1 and TLS 1.2. 
        /// </summary>
        private void UpdateSecurityProtocol()
        {
            if (_isProtocolUpdated) return;

            var amazonSecurityProtocolManager = new AmazonSecurityProtocolManager();

            try
            {
                if (!amazonSecurityProtocolManager.IsSecurityProtocolSystemDefault())
                {
                    amazonSecurityProtocolManager.UpdateProtocolsToSupported();
                }
            }
            catch (Exception ex)
            {
                if (ex is NotSupportedException)
                {
                    _logger.InfoFormat(ex.Message);
                }
                else
                {
                    _logger.InfoFormat("Unexpected error " + ex.GetType().Name +
                                       " encountered when trying to set Security Protocol.\n" + ex);
                }
            }
            _isProtocolUpdated = true;
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
            DontUnescapePathDotsAndSlashes(uri);
            return uri;
        }

        /// <summary>
        /// Patches the in-flight uri to stop it unescaping the path etc (what Uri did before
        /// Microsoft deprecated the constructor flag). This is particularly important for
        /// Amazon S3 customers who want to use backslash (\) in their key names.
        /// </summary>
        /// <remarks>
        /// Different behavior in the various runtimes has been observed and in addition some 
        /// 'documented' ways of doing this between 2.x and 4.x runtimes has also been observed 
        /// to not be reliable.
        /// 
        /// This patch effectively emulates what adding a schemesettings element to the 
        /// app.config file with value 'name="http" genericUriParserOptions="DontUnescapePathDotsAndSlashes"'
        /// does. As we're a dll, that avenue is not open to us.
        /// </remarks>
        /// <param name="uri"></param>
        private static void DontUnescapePathDotsAndSlashes(Uri uri)
        {
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
            if (!string.IsNullOrEmpty(this.Config.ServiceURL))
            {
                var regionName = Util.AWSSDKUtils.DetermineRegion(this.Config.ServiceURL);
                RegionEndpoint region = RegionEndpoint.GetBySystemName(regionName);
                newConfig.RegionEndpoint = region;
            }
            else
            {
                newConfig.RegionEndpoint = this.Config.RegionEndpoint;
            }

            newConfig.UseHttp = this.Config.UseHttp;


            newConfig.ProxyCredentials = this.Config.ProxyCredentials;
            newConfig.ProxyHost = this.Config.ProxyHost;
            newConfig.ProxyPort = this.Config.ProxyPort;
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