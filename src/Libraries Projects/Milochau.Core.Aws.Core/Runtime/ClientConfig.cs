using Milochau.Core.Aws.Core.RegionEndpoints;
using Milochau.Core.Aws.Core.Runtime.Endpoints;

namespace Milochau.Core.Aws.Core.Runtime
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

        private RegionEndpoint? regionEndpoint = null;
        private bool probeForRegionEndpoint = true;
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
        public RegionEndpoint? RegionEndpoint
        {
            get
            {
                if (probeForRegionEndpoint)
                {
                    RegionEndpoint = FallbackRegionFactory.GetRegionEndpoint();
                    probeForRegionEndpoint = false;
                }
                return regionEndpoint;
            }
            set
            {
                regionEndpoint = value;
                probeForRegionEndpoint = regionEndpoint == null;
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
        /// Gets and sets the AuthenticationServiceName property.
        /// Used in AWS4 request signing, this is the short-form
        /// name of the service being called.
        /// </summary>
        public string? AuthenticationServiceName { get; set; } = null;

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
                if (!maxRetries.HasValue)
                {
                    //For standard and adaptive modes first check the environment variables
                    //and shared config for a value. Otherwise default to the new default value.
                    //In the shared config or environment variable MaxAttempts is the total number 
                    //of attempts. This will include the initial call and must be deducted from
                    //from the number of actual retries.
                    return MaxRetriesDefault;
                }

                return maxRetries.Value;
            }
            set { maxRetries = value; }
        }

        #region Constructor 

        protected ClientConfig()
        {
        }

        #endregion

        /// <summary>
        /// Gets and sets of the EndpointProvider property.
        /// This property is used for endpoints resolution.
        /// During service client creation it is set to service's default generated EndpointProvider,
        /// but can be changed to use custom user supplied EndpointProvider.
        /// </summary>
        public IEndpointProvider? EndpointProvider { get; set; }
    }
}
