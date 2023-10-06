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
using System.Globalization;
using Amazon.Runtime.Internal.Util;
using Amazon.Internal;
using System.Threading;
using Amazon.Runtime.Endpoints;

namespace Amazon.Runtime
{
    /// <summary>
    /// This class is the base class of all the configurations settings to connect
    /// to a service.
    /// </summary>
    public abstract partial class ClientConfig
    {
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
            string uniqueString;
            uniqueString = string.Concat( "CacheSize:", 1);

            if (clientConfig.Timeout.HasValue)
                uniqueString = string.Concat(uniqueString, "Timeout:", clientConfig.Timeout.Value.ToString());

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
                return clientConfig.ProxyCredentials == null;
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
        private bool useAlternateUserAgentHeader = AWSConfigs.UseAlternateUserAgentHeader;
        private string authServiceName = null;
        private TimeSpan? timeout = null;
        private bool? useDualstackEndpoint;
        private bool? useFIPSEndpoint;
        private int? maxRetries = null;
        private const int MaxRetriesDefault = 2;

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
                    RegionEndpoint = FallbackRegionFactory.GetRegionEndpoint();
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

            string url = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}{1}", "https://", endpoint.Hostname)).AbsoluteUri;
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
                    return MaxRetriesDefault;
                }

                return this.maxRetries.Value;
            }
            set { this.maxRetries = value; }
        }

        protected IDefaultConfiguration DefaultConfiguration { get; private set; }

        /// <summary>
        /// Credentials to use with a proxy.
        /// </summary>
        public ICredentials ProxyCredentials
        {
            get
            {
                if (!string.IsNullOrEmpty(AWSConfigs.ProxyConfig.Username) ||
                    !string.IsNullOrEmpty(AWSConfigs.ProxyConfig.Password))
                {
                    return new NetworkCredential(AWSConfigs.ProxyConfig.Username, AWSConfigs.ProxyConfig.Password ?? string.Empty);
                }
                return null;
            }
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
                    return false;
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
                    return false;
                }

                return this.useFIPSEndpoint.Value;
            }
            set { useFIPSEndpoint = value; }
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
        /// Gets and sets of the EndpointProvider property.
        /// This property is used for endpoints resolution.
        /// During service client creation it is set to service's default generated EndpointProvider,
        /// but can be changed to use custom user supplied EndpointProvider.
        /// </summary>
        public IEndpointProvider EndpointProvider { get; set; }
    }
}
