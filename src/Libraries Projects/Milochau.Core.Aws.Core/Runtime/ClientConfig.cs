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
using Amazon.Internal;
using System.Threading;
using Amazon.Runtime.Endpoints;

namespace Amazon.Runtime
{
    /// <summary>
    /// This class is the base class of all the configurations settings to connect
    /// to a service.
    /// </summary>
    public abstract partial class ClientConfig : IClientConfig
    {
        /// <summary>
        /// Represents upper limit value for <see cref="RequestMinCompressionSizeBytes"/>
        /// </summary>
        internal const long UpperLimitCompressionSizeBytes = 10485760;

        private RegionEndpoint regionEndpoint = null;
        private bool probeForRegionEndpoint = true;
        private string authServiceName = null;
        private bool? useDualstackEndpoint;
        private bool? useFIPSEndpoint;
        private int? maxRetries = null;
        private const int MaxRetriesDefault = 2;

        /// <summary>
        /// Gets and sets of the UserAgent property.
        /// </summary>
        public abstract string UserAgent { get; }

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
        /// Gets and sets of the EndpointProvider property.
        /// This property is used for endpoints resolution.
        /// During service client creation it is set to service's default generated EndpointProvider,
        /// but can be changed to use custom user supplied EndpointProvider.
        /// </summary>
        public IEndpointProvider EndpointProvider { get; set; }
    }
}
