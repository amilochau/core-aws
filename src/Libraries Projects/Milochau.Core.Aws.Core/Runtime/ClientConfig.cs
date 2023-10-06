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
using System;
using System.Net;
using Amazon.Util;
using System.Globalization;
using Amazon.Runtime.Internal.Util;
using System.Net.Http;
using Amazon.Internal;
using System.Threading;
using Amazon.Runtime.Endpoints;
using Amazon.Runtime.Internal;

namespace Amazon.Runtime
{
    /// <summary>
    /// This class is the base class of all the configurations settings to connect
    /// to a service.
    /// </summary>
    public abstract partial class ClientConfig
    {
        private IWebProxy proxy = null;
        private string proxyHost;
        private int proxyPort = -1;

        private static RegionEndpoint GetDefaultRegionEndpoint()
        {
            return FallbackRegionFactory.GetRegionEndpoint();
        }

        /// <summary>
        /// Returns a WebProxy instance configured to match the proxy settings
        /// in the client configuration.
        /// </summary>
        public IWebProxy GetWebProxy()
        {
            return proxy;
        }

        /// <summary>
        /// Unpacks the host, port and any credentials info into the instance's
        /// proxy-related fields.
        /// Unlike the SetWebProxy implementation on .NET 3.5/4.5,the Host and the Port are not reconstructed from the 
        /// input proxyuri
        /// </summary>
        /// <param name="proxy">The proxy details</param>
        public void SetWebProxy(IWebProxy proxy)
        {
            this.proxy = proxy;
        }

        /// <summary>
        /// Gets and sets of the ProxyHost property.
        /// </summary>
        public string ProxyHost
        {
            get
            {
                if (string.IsNullOrEmpty(this.proxyHost))
                    return AWSConfigs.ProxyConfig.Host;

                return this.proxyHost;
            }
            set
            {
                this.proxyHost = value;
                if (this.ProxyPort>0)
                {
                    this.proxy = new Amazon.Runtime.Internal.Util.WebProxy(ProxyHost, ProxyPort);
                }
            }
        }
        /// <summary>
        /// Gets and sets of the ProxyPort property.
        /// </summary>
        public int ProxyPort
        {
            get
            {
                if (this.proxyPort <= 0)
                    return AWSConfigs.ProxyConfig.Port.GetValueOrDefault();

                return this.proxyPort;
            }
            set
            {
                this.proxyPort = value;
                if (this.ProxyHost!=null)
                {
                    this.proxy = new Amazon.Runtime.Internal.Util.WebProxy(ProxyHost, ProxyPort);
                }
            }
        }
        /// <summary>
        /// Get or set the value to use for <see cref="HttpClientHandler.MaxConnectionsPerServer"/> on requests.
        /// If this property is null, <see cref="HttpClientHandler.MaxConnectionsPerServer"/>
        /// will be left at its default value of <see cref="int.MaxValue"/>.
        /// </summary>
        public int? MaxConnectionsPerServer
        {
            get;
            set;
        }
        
        /// <summary>
        /// HttpClientFactory used to create new HttpClients.
        /// If null, an HttpClient will be created by the SDK.
        /// Note that IClientConfig members such as ProxyHost, ProxyPort, GetWebProxy, and AllowAutoRedirect
        /// will have no effect unless they're used explicitly by the HttpClientFactory implementation.
        ///
        /// See https://docs.microsoft.com/en-us/xamarin/cross-platform/macios/http-stack?context=xamarin/ios and
        /// https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/http-stack?context=xamarin%2Fcross-platform&tabs=macos#ssltls-implementation-build-option
        /// for guidance on creating HttpClients for your platform.
        /// </summary>
        public HttpClientFactory HttpClientFactory { get; set; } = AWSConfigs.HttpClientFactory;
        
        /// <summary>
        /// Returns true if the clients should be cached by HttpRequestMessageFactory, false otherwise.
        /// </summary>
        /// <param name="clientConfig"></param>
        /// <returns></returns>
        internal static bool CacheHttpClients(IClientConfig clientConfig)
        {
            if (clientConfig.HttpClientFactory == null)
                return clientConfig.CacheHttpClient;
            else
                return clientConfig.HttpClientFactory.UseSDKHttpClientCaching(clientConfig);
        }

        /// <summary>
        /// Returns true if the SDK should dispose HttpClients after one use, false otherwise.
        /// </summary>
        /// <param name="clientConfig"></param>
        /// <returns></returns>
        internal static bool DisposeHttpClients(IClientConfig clientConfig)
        {
            if (clientConfig.HttpClientFactory == null)
                return !clientConfig.CacheHttpClient;
            else
                return clientConfig.HttpClientFactory.DisposeHttpClientsAfterUse(clientConfig);
        }

        /// <summary>
        ///  Create a unique string used for caching the HttpClient based on the settings that are used from the ClientConfig that are set on the HttpClient.
        /// </summary>
        /// <param name="clientConfig"></param>
        /// <returns></returns>
        internal static string CreateConfigUniqueString(IClientConfig clientConfig)
        {
            if (clientConfig.HttpClientFactory != null)
            {
                return clientConfig.HttpClientFactory.GetConfigUniqueString(clientConfig);
            }
            string uniqueString = string.Empty;
            uniqueString = string.Concat("AllowAutoRedirect:", clientConfig.AllowAutoRedirect.ToString(), "CacheSize:", clientConfig.HttpClientCacheSize);

            if (clientConfig.Timeout.HasValue)
                uniqueString = string.Concat(uniqueString, "Timeout:", clientConfig.Timeout.Value.ToString());

            if (clientConfig.MaxConnectionsPerServer.HasValue)
                uniqueString = string.Concat(uniqueString, "MaxConnectionsPerServer:", clientConfig.MaxConnectionsPerServer.Value.ToString());

            return uniqueString;
        }


        /// <summary>
        /// Determines if HttpClients created with the given IClientConfig should be cached at the SDK
        /// client level, or cached globally.
        ///
        /// If there is no HttpClientFactory assigned and proxy or proxy credentials are set
        /// this returns false because those properties can't be serialized as part of the key in the global http client cache.
        /// </summary>
        internal static bool UseGlobalHttpClientCache(IClientConfig clientConfig)
        {
            if (clientConfig.HttpClientFactory == null)
                return clientConfig.ProxyCredentials == null && clientConfig.GetWebProxy() == null;
            else
                return clientConfig.HttpClientFactory.GetConfigUniqueString(clientConfig) != null;
        }
    }

    /// <summary>
    /// This class is the base class of all the configurations settings to connect
    /// to a service.
    /// </summary>
    public abstract partial class ClientConfig : IClientConfig
    {
        // Represents infinite timeout. http://msdn.microsoft.com/en-us/library/system.threading.timeout.infinite.aspx
        internal static readonly TimeSpan InfiniteTimeout = TimeSpan.FromMilliseconds(-1);
        /// <summary>
        /// Represents upper limit value for <see cref="RequestMinCompressionSizeBytes"/>
        /// </summary>
        internal const long UpperLimitCompressionSizeBytes = 10485760;

        // Represents max timeout.
        public static readonly TimeSpan MaxTimeout = TimeSpan.FromMilliseconds(int.MaxValue);

        private string serviceId = null;
        private RegionEndpoint regionEndpoint = null;
        private bool probeForRegionEndpoint = true;
        private bool throttleRetries = true;
        private bool useHttp = false;
        private bool useAlternateUserAgentHeader = AWSConfigs.UseAlternateUserAgentHeader;
        private string serviceURL = null;
        private string authServiceName = null;
        private bool readEntireResponse = false;
        private bool logResponse = false;
        private int bufferSize = AWSSDKUtils.DefaultBufferSize;
        private long progressUpdateInterval = AWSSDKUtils.DefaultProgressUpdateInterval;
        private bool resignRetries = false;
        private ICredentials proxyCredentials;
        private bool logMetrics = AWSConfigs.LoggingConfig.LogMetrics;
        private bool disableLogging = false;
        private TimeSpan? timeout = null;
        private bool allowAutoRedirect = true;
        private bool? useDualstackEndpoint;
        private bool? useFIPSEndpoint;
        private bool? disableRequestCompression;
        private long? requestMinCompressionSizeBytes;
        private bool disableHostPrefixInjection = false;
        private bool? endpointDiscoveryEnabled = null;
        private int endpointDiscoveryCacheLimit = 1000;
        private int? maxRetries = null;
        private const int MaxRetriesDefault = 2;
        private const long DefaultMinCompressionSizeBytes = 10240;

        /// <summary>
        /// Gets Service Version
        /// </summary>
        public abstract string ServiceVersion
        {
            get;
        }

        /// <summary>
        /// Gets and sets of the UserAgent property.
        /// </summary>
        public abstract string UserAgent { get; }

        /// <summary>
        /// When set to true, the service client will use the  x-amz-user-agent
        /// header instead of the User-Agent header to report version and
        /// environment information to the AWS service.
        ///
        /// Note: This is especially useful when using a platform like WebAssembly
        /// which doesn't allow to specify the User-Agent header.
        /// </summary>
        public bool UseAlternateUserAgentHeader
        {
            get { return this.useAlternateUserAgentHeader; }
            set { this.useAlternateUserAgentHeader = value; }
        }

        /// <summary>
        /// <para>
        /// Gets and sets the RegionEndpoint property.  The region constant that 
        /// determines the endpoint to use.
        /// 
        /// Setting this property to null will force the SDK to recalculate the
        /// RegionEndpoint value based on App/WebConfig, environment variables,
        /// profile, etc.
        /// </para>
        /// </summary>
        public RegionEndpoint RegionEndpoint
        {
            get
            {
                if (probeForRegionEndpoint)
                {
                    RegionEndpoint = GetDefaultRegionEndpoint();
                    this.probeForRegionEndpoint = false;
                }
                return this.regionEndpoint;
            }
            set
            {
                this.regionEndpoint = value;
                this.probeForRegionEndpoint = this.regionEndpoint == null;

                // legacy support for initial pseudo regions - convert to base Region 
                // and set FIPSEndpoint to true
                if (!string.IsNullOrEmpty(value?.SystemName) &&
                    (value.SystemName.Contains("fips-") || value.SystemName.Contains("-fips")))
                {
                    Logger.GetLogger(GetType()).InfoFormat($"FIPS Pseudo Region support is deprecated. Will attempt to convert {value.SystemName}.");

                    this.UseFIPSEndpoint = true;
                    this.regionEndpoint = RegionEndpoint.GetBySystemName(value.SystemName.Replace("fips-", "").Replace("-fips", ""));
                    this.RegionEndpoint.OriginalSystemName = value.SystemName;
                }
            }
        }

        /// <summary>
        /// The constant used to lookup in the region hash the endpoint.
        /// </summary>
        public abstract string RegionEndpointServiceName
        {
            get;
        }

        /// <summary>
        /// <para>
        /// Gets and sets of the ServiceURL property.
        /// This is an optional property; change it
        /// only if you want to try a different service
        /// endpoint.
        /// </para>
        /// <para>
        /// RegionEndpoint and ServiceURL are mutually exclusive properties. 
        /// Whichever property is set last will cause the other to automatically 
        /// be reset to null.
        /// </para>
        /// </summary>
        public string ServiceURL
        {
            get
            {
                return this.serviceURL;
            }
        }

        /// <summary>
        /// Gets and sets the UseHttp.
        /// If this property is set to true, the client attempts
        /// to use HTTP protocol, if the target endpoint supports it.
        /// By default, this property is set to false.
        /// </summary>
        public bool UseHttp
        {
            get { return this.useHttp; }
            set { this.useHttp = value; }
        }

        /// <summary>
        /// Given this client configuration, return a string form ofthe service endpoint url.
        /// </summary>
        [Obsolete("This operation is obsoleted because as of version 3.7.100 endpoint is resolved using a newer system that uses request level parameters to resolve the endpoint, use the service-specific client.DetermineServiceOperationEndPoint method instead.")]
        public virtual string DetermineServiceURL()
        {
            return GetUrl(this, RegionEndpoint);
        }

        internal static string GetUrl(IClientConfig config, RegionEndpoint regionEndpoint)
        {
            var endpoint =
                regionEndpoint.GetEndpointForService(
                    config.RegionEndpointServiceName,
                    config.ToGetEndpointForServiceOptions());

            string url = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}{1}", config.UseHttp ? "http://" : "https://", endpoint.Hostname)).AbsoluteUri;
            return url;
        }

        /// <summary>
        /// Gets and sets the AuthenticationServiceName property.
        /// Used in AWS4 request signing, this is the short-form
        /// name of the service being called.
        /// </summary>
        public string AuthenticationServiceName
        {
            get { return this.authServiceName; }
            set { this.authServiceName = value; }
        }
        /// <summary>
        /// The serviceId for the service, which is specified in the metadata in the ServiceModel.
        /// The transformed value of the service ID (replace any spaces in the service ID 
        /// with underscores and uppercase all letters) is used to set service-specific endpoint urls.
        /// I.e: AWS_ENDPOINT_URL_ELASTIC_BEANSTALK
        /// For configuration files, replace any spaces with underscores and lowercase all letters
        /// I.e. elastic_beanstalk = 
        ///     endpoint_url = http://localhost:8000
        /// </summary>
        public string ServiceId
        {
            get { return this.serviceId; }
            set { this.serviceId = value; }
        }
        /// <summary>
        /// Returns the flag indicating how many retry HTTP requests an SDK should
        /// make for a single SDK operation invocation before giving up. This flag will 
        /// return 4 when the RetryMode is set to "Legacy" which is the default. For
        /// RetryMode values of "Standard" or "Adaptive" this flag will return 2. In 
        /// addition to the values returned that are dependent on the RetryMode, the
        /// value can be set to a specific value by using the AWS_MAX_ATTEMPTS environment
        /// variable, max_attempts in the shared configuration file, or by setting a
        /// value directly on this property. When using AWS_MAX_ATTEMPTS or max_attempts
        /// the value returned from this property will be one less than the value entered
        /// because this flag is the number of retry requests, not total requests.
        /// </summary>
        public int MaxErrorRetry
        {
            get
            {
                if (!this.maxRetries.HasValue)
                {
                    //For standard and adaptive modes first check the environment variables
                    //and shared config for a value. Otherwise default to the new default value.
                    //In the shared config or environment variable MaxAttempts is the total number 
                    //of attempts. This will include the initial call and must be deducted from
                    //from the number of actual retries.
                    return FallbackInternalConfigurationFactory.MaxAttempts - 1 ?? MaxRetriesDefault;
                }

                return this.maxRetries.Value;
            }
            set { this.maxRetries = value; }
        }

        /// <summary>
        /// Determines if MaxErrorRetry has been manually set.
        /// </summary>
        public bool IsMaxErrorRetrySet
        {
            get
            {
                return this.maxRetries.HasValue;
            }
        }

        /// <summary>
        /// Gets and sets the LogResponse property.
        /// If this property is set to true, the service response is logged.
        /// The size of response being logged is controlled by the AWSConfigs.LoggingConfig.LogResponsesSizeLimit property.
        /// </summary>
        public bool LogResponse
        {
            get { return this.logResponse; }
            set { this.logResponse = value; }
        }

        /// <summary>
        /// Gets and sets the ReadEntireResponse property.
        /// NOTE: This property does not effect response processing and is deprecated.
        /// To enable response logging, the ClientConfig.LogResponse and AWSConfigs.LoggingConfig
        /// properties can be used.
        /// </summary>
        [Obsolete("This property does not effect response processing and is deprecated." +
            "To enable response logging, the ClientConfig.LogResponse and AWSConfigs.LoggingConfig.LogResponses properties can be used.")]
        public bool ReadEntireResponse
        {
            get { return this.readEntireResponse; }
            set { this.readEntireResponse = value; }
        }

        /// <summary>
        /// Gets and Sets the BufferSize property.
        /// The BufferSize controls the buffer used to read in from input streams and write 
        /// out to the request.
        /// </summary>
        public int BufferSize
        {
            get { return this.bufferSize; }
            set { this.bufferSize = value; }
        }

        /// <summary>
        /// <para>
        /// Gets or sets the interval at which progress update events are raised
        /// for upload operations. By default, the progress update events are 
        /// raised at every 100KB of data transferred. 
        /// </para>
        /// <para>
        /// If the value of this property is set less than ClientConfig.BufferSize, 
        /// progress updates events will be raised at the interval specified by ClientConfig.BufferSize.
        /// </para>
        /// </summary>
        public long ProgressUpdateInterval
        {
            get { return progressUpdateInterval; }
            set { progressUpdateInterval = value; }
        }


        /// <summary>
        /// Flag on whether to resign requests on retry or not.
        /// For Amazon S3 and Amazon Glacier this value will always be set to true.
        /// </summary>
        public bool ResignRetries
        {
            get { return this.resignRetries; }
            set { this.resignRetries = value; }
        }

        /// <summary>
        /// This flag controls if .NET HTTP infrastructure should follow redirection
        ///  responses (e.g. HTTP 307 - temporary redirect).
        /// </summary>
        public bool AllowAutoRedirect
        {
            get
            {
                return this.allowAutoRedirect;
            }
            set
            {
                this.allowAutoRedirect = value;
            }
        }

        /// <summary>
        /// Flag on whether to log metrics for service calls.
        /// 
        /// This can be set in the application's configs, as below:
        /// <code>
        /// &lt;?xml version="1.0" encoding="utf-8" ?&gt;
        /// &lt;configuration&gt;
        ///     &lt;appSettings&gt;
        ///         &lt;add key="AWSLogMetrics" value"true"/&gt;
        ///     &lt;/appSettings&gt;
        /// &lt;/configuration&gt;
        /// </code>
        /// </summary>
        public bool LogMetrics
        {
            get { return this.logMetrics; }
            set { this.logMetrics = value; }
        }

        /// <summary>
        /// Gets and sets the DisableLogging. If true logging for this client will be disabled.
        /// </summary>
        public bool DisableLogging
        {
            get { return this.disableLogging; }
            set { this.disableLogging = value; }
        }

        protected IDefaultConfiguration DefaultConfiguration { get; private set; }

        /// <summary>
        /// Credentials to use with a proxy.
        /// </summary>
        public ICredentials ProxyCredentials
        {
            get
            {
                if (this.proxyCredentials == null &&
                    (!string.IsNullOrEmpty(AWSConfigs.ProxyConfig.Username) ||
                    !string.IsNullOrEmpty(AWSConfigs.ProxyConfig.Password)))
                {
                    return new NetworkCredential(AWSConfigs.ProxyConfig.Username, AWSConfigs.ProxyConfig.Password ?? string.Empty);
                }
                return this.proxyCredentials;
            }
            set { this.proxyCredentials = value; }
        }

        #region Constructor 

        protected ClientConfig(IDefaultConfiguration defaultConfiguration)
        {
            DefaultConfiguration = defaultConfiguration;

            Initialize();
        }

        #endregion

        protected virtual void Initialize()
        {
        }

        /// <remarks>
        /// <para>
        /// If the value is set, the value is assigned to the Timeout property of the HttpWebRequest/HttpClient object used
        /// to send requests.
        /// </para>
        /// <para>
        /// Please specify a timeout value only if the operation will not complete within the default intervals
        /// specified for an HttpWebRequest/HttpClient.
        /// </para>
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">The timeout specified is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The timeout specified is less than or equal to zero and is not Infinite.</exception>
        /// <seealso cref="P:System.Net.HttpWebRequest.Timeout"/>
        /// <seealso cref="P:System.Net.Http.HttpClient.Timeout"/>
        public TimeSpan? Timeout
        {
            get
            {
                if (this.timeout.HasValue)
                    return this.timeout;

                // TimeToFirstByteTimeout is not a perfect match with HttpWebRequest/HttpClient.Timeout.  However, given
                // that both are configured to only use Timeout until the Response Headers are downloaded, this value
                // provides a reasonable default value.
                return DefaultConfiguration.TimeToFirstByteTimeout;
            }
            set
            {
                ValidateTimeout(value);
                this.timeout = value;
            }
        }

        /// <summary>
        /// Generates a <see cref="CancellationToken"/> based on the value
        /// for <see cref="DefaultConfiguration.TimeToFirstByteTimeout"/>.
        /// <para />
        /// NOTE: <see cref="Amazon.Runtime.HttpWebRequestMessage.GetResponseAsync"/> uses 
        /// </summary>
        internal CancellationToken BuildDefaultCancellationToken()
        {
            // TimeToFirstByteTimeout is not a perfect match with HttpWebRequest/HttpClient.Timeout.  However, given
            // that both are configured to only use Timeout until the Response Headers are downloaded, this value
            // provides a reasonable default value.
            var cancelTimeout = DefaultConfiguration.TimeToFirstByteTimeout;

            return cancelTimeout.HasValue
                ? new CancellationTokenSource(cancelTimeout.Value).Token
                : default(CancellationToken);
        }

        /// <summary>
        /// Configures the endpoint calculation for a service to go to a dual stack (ipv6 enabled) endpoint
        /// for the configured region.
        /// </summary>
        /// <remarks>
        /// Note: AWS services are enabling dualstack endpoints over time. It is your responsibility to check 
        /// that the service actually supports a dualstack endpoint in the configured region before enabling 
        /// this option for a service.
        /// </remarks>
        public bool UseDualstackEndpoint
        {
            get
            {
                if (!this.useDualstackEndpoint.HasValue)
                {
                    return FallbackInternalConfigurationFactory.UseDualStackEndpoint ?? false;
                }

                return this.useDualstackEndpoint.Value;
            }
            set { useDualstackEndpoint = value; }
        }

        /// <summary>
        /// Configures the endpoint calculation to go to a FIPS (https://aws.amazon.com/compliance/fips/) endpoint
        /// for the configured region.
        /// </summary>
        public bool UseFIPSEndpoint
        {
            get
            {
                if (!this.useFIPSEndpoint.HasValue)
                {
                    return FallbackInternalConfigurationFactory.UseFIPSEndpoint ?? false;
                }

                return this.useFIPSEndpoint.Value;
            }
            set { useFIPSEndpoint = value; }
        }
        /// <summary>
        /// Controls whether request payloads are automatically compressed for supported operations.
        /// This setting only applies to operations that support compression.
        /// The default value is "false". Set to "true" to disable compression.
        /// </summary>
        public bool DisableRequestCompression
        {
            get
            {
                if (!this.disableRequestCompression.HasValue)
                {
                    return FallbackInternalConfigurationFactory.DisableRequestCompression ?? false;
                }

                return this.disableRequestCompression.Value;
            }
            set { disableRequestCompression = value; }
        }

        /// <summary>
        /// Minimum size in bytes that a request body should be to trigger compression.
        /// </summary>
        public long RequestMinCompressionSizeBytes
        {
            get
            {
                if (!this.requestMinCompressionSizeBytes.HasValue)
                {
                    return FallbackInternalConfigurationFactory.RequestMinCompressionSizeBytes ?? DefaultMinCompressionSizeBytes;
                }

                return this.requestMinCompressionSizeBytes.Value;
            }
            set
            {
                ValidateMinCompression(value);
                requestMinCompressionSizeBytes = value;
            }
        }

        private static void ValidateMinCompression(long minCompressionSize)
        {
            if (minCompressionSize < 0 || minCompressionSize > UpperLimitCompressionSizeBytes)
            {
                throw new ArgumentException(string.Format("Invalid value {0} for {1}." +
                    " A long value between 0 and {2} bytes inclusive is expected.", minCompressionSize,
                    nameof(requestMinCompressionSizeBytes), UpperLimitCompressionSizeBytes));
            }
        }

        /// <summary>
        /// Enable or disable the Retry Throttling feature by setting the ThrottleRetries flag to True/False respectively.
        /// Retry Throttling is a feature that intelligently throttles retry attempts when a large percentage of requests 
        /// are failing and retries are unsuccessful as well. In such situations the allotted retry capacity for the service URL
        /// will be drained until requests start to succeed again. Once the requisite capacity is available, retries would 
        /// be permitted again. When retries are throttled, the service enters a fail-fast behaviour as the traditional retry attempt
        /// for the request would be circumvented. Hence, errors will resurface quickly. This will result in a greater number of exceptions
        /// but prevents requests being tied up in unsuccessful retry attempts.
        /// Note: Retry Throttling is enabled by default. Set the ThrottleRetries flag to false to switch off this feature.
        /// </summary>
        public bool ThrottleRetries
        {
            get { return throttleRetries; }
            set { throttleRetries = value; }
        }

        /// <summary>
        /// The calculated clock skew correction for a specific endpoint, if there is one.
        /// This field will be set if a service call resulted in an exception
        /// and the SDK has determined that there is a difference between local
        /// and server times.
        /// 
        /// If <seealso cref="CorrectForClockSkew"/> is set to true, this
        /// value will still be set to the correction, but it will not be used by the
        /// SDK and clock skew errors will not be retried.
        /// </summary>
        public TimeSpan ClockOffset
        {
            get
            {
                if (AWSConfigs.ManualClockCorrection.HasValue)
                {
                    return AWSConfigs.ManualClockCorrection.Value;
                }
                else
                {
                    string endpoint = DetermineServiceURL();
                    return CorrectClockSkew.GetClockCorrectionForEndpoint(endpoint);
                }
            }
        }

        /// <summary>
        /// Gets and sets the DisableHostPrefixInjection flag. If true, host prefix injection will be disabled for this client, the default value of this flag is false. 
        /// Host prefix injection prefixes the service endpoint with request members from APIs which use this feature. 
        /// Example: for a hostPrefix of "foo-name." and a endpoint of "service.region.amazonaws.com" the default behavior is to
        /// prefix the endpoint with the hostPrefix resulting in a final endpoint of "foo-name.service.region.amazonaws.com". Setting 
        /// DisableHostPrefixInjection to true will disable hostPrefix injection resulting in a final endpoint of
        /// "service.region.amazonaws.com" regardless of the value of hostPrefix. E.g. You may want to disable host prefix injection for testing against a local mock endpoint.
        /// </summary>
        public bool DisableHostPrefixInjection
        {
            get { return this.disableHostPrefixInjection; }
            set { this.disableHostPrefixInjection = value; }
        }

        /// <summary>
        /// Returns the flag indicating if endpoint discovery should be enabled or disabled for operations that are not required to use endpoint discovery.
        /// </summary>
        public bool EndpointDiscoveryEnabled
        {
            get
            {
                if (!this.endpointDiscoveryEnabled.HasValue)
                {
                    return FallbackInternalConfigurationFactory.EndpointDiscoveryEnabled ?? false;
                }

                return this.endpointDiscoveryEnabled.Value;
            }
            set { this.endpointDiscoveryEnabled = value; }
        }

        /// <summary>
        /// Returns the maximum number of discovered endpoints that can be stored within the cache for the client. The default limit is 1000 cache entries.
        /// </summary>
        public int EndpointDiscoveryCacheLimit
        {
            get { return this.endpointDiscoveryCacheLimit; }
            set { this.endpointDiscoveryCacheLimit = value; }
        }

        /// <summary>
        /// Under Adaptive retry mode, this flag determines if the client should wait for
        /// a send token to become available or don't block and fail the request immediately
        /// if a send token is not available.
        /// </summary>
        public bool FastFailRequests { get; set; } = false;

        /// <summary>
        /// Throw an exception if the boxed TimeSpan parameter doesn't have a value or is out of range.
        /// </summary>
        public static void ValidateTimeout(TimeSpan? timeout)
        {
            if (!timeout.HasValue)
            {
                throw new ArgumentNullException("timeout");
            }

            if (timeout != InfiniteTimeout && (timeout <= TimeSpan.Zero || timeout > MaxTimeout))
            {
                throw new ArgumentOutOfRangeException("timeout");
            }
        }

        /// <summary>
        /// <para>
        /// This is a switch used for performance testing and is not intended for production applications 
        /// to change. This switch may be removed in a future version of the SDK as the .NET Core platform matures.
        /// </para>
        /// <para>
        /// If true, the HttpClient is cached and reused for every request made by the service client 
        /// and shared with other service clients.
        /// </para>
        /// <para>
        /// For the .NET Core platform this is default to true because the HttpClient manages the connection
        /// pool.
        /// </para>
        /// </summary>
        public bool CacheHttpClient {get; set;} = true;

        private int? _httpClientCacheSize;
        /// <summary>
        /// If CacheHttpClient is set to true then HttpClientCacheSize controls the number of HttpClients cached.
        /// <para>
        /// The default value is 1 which is suitable for Windows and for all other platforms that have HttpClient
        /// implementations using <see cref="System.Net.Http.SocketsHttpHandler"/> (.NET Core 2.1 and higher).
        /// </para>
        /// </summary>
        public int HttpClientCacheSize
        {
            get
            {
                if(_httpClientCacheSize.HasValue)
                {
                    return _httpClientCacheSize.Value;
                }

// Use both NETCOREAPP3_1 and NETCOREAPP3_1_OR_GREATER because currently the build server only has .NET Core 3.1 SDK installed
// which predates the OR_GREATER preprocessor statements. The NETCOREAPP3_1_OR_GREATER is used for future proofing.
                return 1;
            }
            set => _httpClientCacheSize = value;
        }

        /// <summary>
        /// Gets and sets of the EndpointProvider property.
        /// This property is used for endpoints resolution.
        /// During service client creation it is set to service's default generated EndpointProvider,
        /// but can be changed to use custom user supplied EndpointProvider.
        /// </summary>
        public IEndpointProvider EndpointProvider { get; set; }
    }
}
