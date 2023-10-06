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
using Amazon.Runtime.Endpoints;

namespace Amazon.Runtime
{
    public partial interface IClientConfig
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
        HttpClientFactory HttpClientFactory { get; }
    }

    /// <summary>
    /// This interface is the read only access to the ClientConfig object used when setting up service clients. Once service clients
    /// are initiated the config object should not be changed to avoid issues with using a service client in a multi threaded environment.
    /// </summary>
    public partial interface IClientConfig
    {
        /// <summary>
        /// The serviceId for the service, which is specified in the metadata in the ServiceModel.
        /// The transformed value of the service ID (replace any spaces in the service ID 
        /// with underscores and uppercase all letters) is used to set service-specific endpoint urls.
        /// I.e: AWS_ENDPOINT_URL_ELASTIC_BEANSTALK
        /// For configuration files, replace any spaces with underscores and lowercase all letters
        /// I.e. elastic_beanstalk = 
        ///     endpoint_url = http://localhost:8000
        /// </summary>
        string ServiceId { get; }

        /// <summary>
        /// Gets the RegionEndpoint property. The region constant to use that 
        /// determines the endpoint to use.  If this is not set
        /// then the client will fallback to the value of ServiceURL.
        /// </summary>
        RegionEndpoint RegionEndpoint { get; }

        /// <summary>
        /// The constant used to lookup in the region hash the endpoint.
        /// </summary>
        string RegionEndpointServiceName { get; }

        /// <summary>
        /// Gets and sets of the EndpointProvider property.
        /// This property is used for endpoints resolution.
        /// During service client creation it is set to service's default generated EndpointProvider,
        /// but can be changed to use custom user supplied EndpointProvider.
        /// </summary>
        IEndpointProvider EndpointProvider { get; }

        /// <summary>
        /// Gets Service Version
        /// </summary>
        string ServiceVersion { get; }

        /// <summary>
        /// Gets the AuthenticationServiceName property.
        /// Used in AWS4 request signing, this is the short-form
        /// name of the service being called.
        /// </summary>
        string AuthenticationServiceName { get; }


        /// <summary>
        /// Gets the UserAgent property.
        /// </summary>
        string UserAgent { get; }

        /// <summary>
        /// Returns the flag indicating how many retry HTTP requests an SDK should
        /// make for a single SDK operation invocation before giving up. This flag will 
        /// return 4 when the RetryMode is set to "Legacy" which is the default. For
        /// RetryMode values of "Standard" or "Adaptive" this flag will return 2. In 
        /// addition to the values returned that are dependant on the RetryMode, the
        /// value can be set to a specific value by using the AWS_MAX_ATTEMPTS environment
        /// variable, max_attempts in the shared configuration file, or by setting a
        /// value directly on this property. When using AWS_MAX_ATTEMPTS or max_attempts
        /// the value returned from this property will be one less than the value entered
        /// because this flag is the number of retry requests, not total requests.
        /// </summary>
        int MaxErrorRetry { get; }

        /// <summary>
        /// Credentials to use with a proxy.
        /// </summary>
        ICredentials ProxyCredentials { get; }

        /// <summary>
        /// Gets the default request timeout value.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the value is set, the value is assigned to the Timeout property of the HTTPWebRequest/HttpClient object used
        /// to send requests.
        /// </para>
        /// <para>
        /// Please specify a timeout value only if the operation will not complete within the default intervals
        /// specified for an HttpWebRequest/HttpClient.
        /// </para>
        /// </remarks>
        TimeSpan? Timeout { get; }

        /// <summary>
        /// Configures the endpoint calculation for a service to go to a dual stack (ipv6 enabled) endpoint
        /// for the configured region.
        /// </summary>
        /// <remarks>
        /// Note: AWS services are enabling dualstack endpoints over time. It is your responsibility to check 
        /// that the service actually supports a dualstack endpoint in the configured region before enabling 
        /// this option for a service.
        /// </remarks>
        bool UseDualstackEndpoint { get; }

        /// <summary>
        /// Configures the endpoint calculation to go to a FIPS (https://aws.amazon.com/compliance/fips/) endpoint
        /// for the configured region.
        /// </summary>
        bool UseFIPSEndpoint { get; }

        /// <summary>
        /// Using either the RegionEndpoint or the ServiceURL determine what the URL to the service is.
        /// </summary>
        /// <returns>The URL to the service.</returns>
        [Obsolete("This operation is obsoleted because as of version 3.7.100 endpoint is resolved using a newer system that uses request level parameters to resolve the endpoint, use the service-specific client.DetermineServiceOperationEndPoint method instead.")]
        string DetermineServiceURL();

        /// <summary>
        /// Returns the calculated clock skew value for this config's service endpoint. If AWSConfigs.CorrectForClockSkew is false,
        /// this value won't be used to construct service requests.
        /// </summary>
        TimeSpan ClockOffset { get; }

        /// <summary>
        /// When set to true, the service client will use the  x-amz-user-agent
        /// header instead of the User-Agent header to report version and
        /// environment information to the AWS service.
        ///
        /// Note: This is especially useful when using a platform like WebAssembly
        /// which doesn't allow to specify the User-Agent header.
        /// </summary>
        bool UseAlternateUserAgentHeader { get; }
    }
}