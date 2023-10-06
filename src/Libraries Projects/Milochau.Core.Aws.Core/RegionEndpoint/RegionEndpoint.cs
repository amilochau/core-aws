/*******************************************************************************
 *  Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *  Licensed under the Apache License, Version 2.0 (the "License"). You may not use
 *  this file except in compliance with the License. A copy of the License is located at
 *
 *  http://aws.amazon.com/apache2.0
 *
 *  or in the "license" file accompanying this file.
 *  This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
 *  CONDITIONS OF ANY KIND, either express or implied. See the License for the
 *  specific language governing permissions and limitations under the License.
 * *****************************************************************************
 *    __  _    _  ___
 *   (  )( \/\/ )/ __)
 *   /__\ \    / \__ \
 *  (_)(_) \/\/  (___/
 *
 *  AWS SDK for .NET
 *
 */
using Amazon.Internal;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Amazon
{
    /// <summary>
    /// This class contains region information used to lazily compute the service endpoints. The static constants representing the 
    /// regions can be used while constructing the AWS client instead of looking up the exact endpoint URL.
    /// </summary>
    public partial class RegionEndpoint
    {
        #region Statics
        private static Dictionary<string, RegionEndpoint> _hashBySystemName = new Dictionary<string, RegionEndpoint>(StringComparer.OrdinalIgnoreCase);
        private static ReaderWriterLockSlim _regionEndpointOverrideLock = new ReaderWriterLockSlim(); // controls access to _hashRegionEndpointOverride

        /// <summary>
        /// Represents the endpoint overridding rules in the endpoints.json
        /// Is used to map private region (ie us-east-1-regional) to public regions (us-east-1)
        /// For signing purposes. Map is keyed by region SystemName.
        /// </summary>
        private static Dictionary<string, RegionEndpoint> _hashRegionEndpointOverride = new Dictionary<string, RegionEndpoint>();

        /// <summary>
        /// Gets the region based on its system name like "us-west-1"
        /// </summary>
        /// <param name="systemName">The system name of the service like "us-west-1"</param>
        /// <returns></returns>
        public static RegionEndpoint GetBySystemName(string systemName)
        {   
            return GetEndpoint(systemName, null);
        }

        /// <summary>
        /// Gets the region endpoint override if exists
        /// </summary>
        /// <param name="regionEndpoint">The region endpoint to find the possible override for</param>
        /// <returns></returns>
        public static RegionEndpoint GetRegionEndpointOverride(RegionEndpoint regionEndpoint)
        {
            try
            {
                _regionEndpointOverrideLock.EnterReadLock();

                if (!_hashRegionEndpointOverride.TryGetValue(regionEndpoint.SystemName,
                    out var regionEndpointOverride))
                {
                    return null;
                }

                return regionEndpointOverride;
            }
            finally
            {
                _regionEndpointOverrideLock.ExitReadLock();
            }
        }

        private static RegionEndpoint GetEndpoint(string systemName, string displayName)
        {
            RegionEndpoint regionEndpoint = null;
            if (displayName == null)
            {
                lock (_hashBySystemName)
                {
                    if (_hashBySystemName.TryGetValue(systemName, out regionEndpoint)) // @todo how do we have this dictionary with values??
                        return regionEndpoint;
                }

                // GetRegionEndpoint will always return a non-null value. If the the region(systemName) is unknown,
                // the providers will create a fallback instance that will generate an endpoint to the best
                // of its knowledge.
                displayName = RegionEndpointProvider.GetRegionEndpoint(systemName).DisplayName;
            }
            
            lock (_hashBySystemName)
            {
                if (_hashBySystemName.TryGetValue(systemName, out regionEndpoint))
                    return regionEndpoint;

                regionEndpoint = new RegionEndpoint(systemName, displayName);
                _hashBySystemName.Add(regionEndpoint.SystemName, regionEndpoint);
            }

            return regionEndpoint;
        }

        private static IRegionEndpointProvider _regionEndpointProvider;
        private static IRegionEndpointProvider RegionEndpointProvider
        {
            get
            {
                if (_regionEndpointProvider == null)
                {
                    _regionEndpointProvider = new RegionEndpointProviderV3();
                }
                return _regionEndpointProvider;
            }
        }
        #endregion

        private RegionEndpoint(string systemName, string displayName)
        {
            this.SystemName = systemName;
            this.OriginalSystemName = systemName;
            this.DisplayName = displayName;
        }

        /// <summary>
        /// Gets the system name of a region.
        /// </summary>
        public string SystemName
        {
            get;
            private set;
        }

        public string OriginalSystemName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the display name of a region.
        /// </summary>
        public string DisplayName
        {
            get;
            private set;
        }

        private IRegionEndpoint InternedRegionEndpoint
        {
            get
            {
                return RegionEndpointProvider.GetRegionEndpoint(SystemName);
            }
        }

        /// <summary>
        /// Gets the endpoint for a service in a region.
        /// <para />
        /// For forwards compatibility, if the service being requested for isn't known in the region, this method 
        /// will generate an endpoint using the AWS endpoint heuristics. In this case, it is not guaranteed the
        /// endpoint will point to a valid service endpoint.
        /// </summary>
        /// <param name="serviceName">
        /// The services system name. Service system names can be obtained from the
        /// RegionEndpointServiceName member of the ClientConfig-derived class for the service.
        /// </param>
        /// <param name="options">
        /// Specify additional requirements on the <see cref="Endpoint"/> to be returned.
        /// </param>
        public Endpoint GetEndpointForService(string serviceName)
        {
            return InternedRegionEndpoint.GetEndpointForService(serviceName);
        }

        /// <summary>
        /// This class defines an endpoints hostname and which protocols it supports.
        /// </summary>
        public class Endpoint
        {
            internal Endpoint(string hostname, string authregion)
            {
                this.Hostname = hostname;
                this.AuthRegion = authregion;
            }

            /// <summary>
            /// Gets the hostname for the service.
            /// </summary>
            public string Hostname { get; private set; }

            /// <summary>
            /// The authentication region to be used in request signing.
            /// </summary>
            public string AuthRegion { get; private set; }
        }
    }
}
