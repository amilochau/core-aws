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

using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Amazon.XRay.Recorder.Core.Sampling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Amazon.Internal
{
    [Obsolete("This class is obsoleted because as of version 3.7.100 endpoint is resolved using a newer system that uses request level parameters to resolve the endpoint, use the service-specific client.DetermineServiceOperationEndPoint method instead.")]
    public class RegionEndpointV3 : IRegionEndpoint
    {
        private ServiceMap _serviceMap = new ServiceMap();
        public string RegionName { get; private set; }
        public string DisplayName { get; private set; }

        private EndpointsPartition partition;
        private Dictionary<string, EndpointsPartitionService> services;

        private bool _servicesLoaded = false;

        public RegionEndpointV3(string regionName, string displayName, EndpointsPartition partition, Dictionary<string, EndpointsPartitionService> services)
        {
            RegionName = regionName;
            DisplayName = displayName;

            this.partition = partition;
            this.services = services;
        }

        /// <summary>
        /// Retrieves the endpoint for the given service in the current region
        /// </summary>
        /// <param name="serviceName">Name of the service in endpoints.json</param>
        /// <param name="options"> Specify additional requirements on the <see cref="RegionEndpoint.Endpoint"/> to be returned.</param>
        /// <returns>Matching endpoint from endpoints.json, or a computed endpoint if possible</returns>
        public RegionEndpoint.Endpoint GetEndpointForService(string serviceName, GetEndpointForServiceOptions options)
        {
            var variants = BuildVariantHashSet(options);
            return GetEndpointForService(serviceName, variants);
        }

        /// <summary>
        /// Retrieves the endpoint for the given service in the current region
        /// </summary>
        /// <param name="serviceName">Name of the service in endpoints.json</param>
        /// <param name="variants">Set of tags describing an endpoint variant</param>
        /// <returns>Matching endpoint from endpoints.json, or a computed endpoint if possible</returns>
        public RegionEndpoint.Endpoint GetEndpointForService(string serviceName, HashSet<string> variants)
        {
            RegionEndpoint.Endpoint endpointObject = null;

            // lock on _partitionJsonData because:
            // a) ParseAllServices() will mutate _partitionJsonData, so it needs to be run inside a critical section.
            // b) RegionEndpointV3 objects are exclusively built by RegionEndpointProviderV3, which will
            //    constructor inject the _same instance_ of _servicesJsonData and _partitionJsonData into all 
            //    RegionEndpointProviderV3.
            // c) This provides thread-safety if multiple RegionEndpointV3 instances were to be initialized at 
            //    the same time: https://github.com/aws/aws-sdk-net/issues/1939
            lock (partition)
            {
                if (!_servicesLoaded)
                {
                    ParseAllServices();
                    _servicesLoaded = true;
                }

                if (!_serviceMap.TryGetEndpoint(serviceName, variants, out endpointObject))
                {
                    // For all current variants (dual-stack and FIPS) SDKs cannot
                    // fall back to normal endpoints and must raise an error.
                    if (variants?.Count > 0)
                    {
                        throw new AmazonClientException($"Requested endpoint for {serviceName} with variants [{string.Join(", ", variants.ToArray())}] could not be found.");
                    }
                    // Do a fallback of creating an unknown endpoint based on the
                    // current region's hostname template.
                    endpointObject = CreateUnknownEndpoint(serviceName);
                }
            }
            return endpointObject;
        }

        private RegionEndpoint.Endpoint CreateUnknownEndpoint(string serviceName)
        {
            string template = partition.Defaults.Hostname;

            string dnsSuffix = partition.DnsSuffix;
            string hostname = template.Replace("{service}", serviceName)
                                 .Replace("{region}", RegionName)
                                 .Replace("{dnsSuffix}", dnsSuffix);

            return new RegionEndpoint.Endpoint(hostname, null);
        }

        private void ParseAllServices()
        {
            foreach (string serviceName in services.Keys)
            {
                if (services[serviceName] != null)
                {
                    AddServiceToMap(services[serviceName], serviceName);
                }
            }
        }

        private void AddServiceToMap(EndpointsPartitionService service, string serviceName)
        {
            var partitionEndpoint = service.PartitionEndpoint ?? "";
            var isRegionalized = service.IsRegionalized ?? true;
            var regionKey = RegionName;

            // Use the partition's default endpoint if the service is not regionalized, like Route53, and there is no
            // endpoint defined for the this service name.
            if (!isRegionalized && service.Endpoints[regionKey] == null && !string.IsNullOrEmpty(partitionEndpoint))
            {
                regionKey = partitionEndpoint;
            }

            var regionEndpoint = service.Endpoints[regionKey];
            var mergedEndpoint = new MergedEndpoint();
            var variants = new Dictionary<HashSet<string>, EndpointsPartitionDefaultsVariant>(HashSet<string>.CreateSetComparer());

            // Create the merged endpoint definitions for both the normal endpoint and variants
            MergeJsonData(mergedEndpoint, regionEndpoint, variants); // first prioritizing the service+region object
            MergeJsonData(mergedEndpoint, service.Defaults, variants); // then service-level defaults
            MergeJsonData(mergedEndpoint, partition.Defaults, variants); // then partition-level defaults

            // Preserve existing behavior of short circuiting the normal endpoint if there isn't a region-specific entry
            if (regionEndpoint != null)
            {
                AddNormalEndpointToServiceMap(mergedEndpoint, RegionName, serviceName);
            }

            AddVariantEndpointsToServiceMap(mergedEndpoint, RegionName, serviceName, variants);
        }

        private static void MergeJsonData(MergedEndpoint target, EndpointsPartitionDefaults source, Dictionary<HashSet<string>, EndpointsPartitionDefaultsVariant> variants)
        {
            if (source == null || target == null)
            {
                return;
            }

            //target.Deprecated = source.Deprecated;
            target.Hostname ??= source.Hostname;
            target.SignatureVersions ??= source.SignatureVersions;
            //target.CredentialScope = source.CredentialScope;

            // Variants need special handling because they are identified by the "tags" within the
            // variant object. First build the key, and then merge the rest of the json properties for a given variant.
            foreach (var variant in source.Variants)
            {
                var tagsKey = new HashSet<string>();
                foreach (var label in variant.Tags)
                {
                    tagsKey.Add(label);
                }

                if (variants.ContainsKey(tagsKey))
                {
                    // We've encountered this variant at a lower level in the hierarchy
                    // so only merge properties which are still null
                    variants[tagsKey].DnsSuffix ??= variant.DnsSuffix;
                    variants[tagsKey].Tags ??= variant.Tags;
                    variants[tagsKey].Hostname ??= variant.Hostname;
                }
                else // First time encountering this variant, so merge the entire object
                {
                    variants[tagsKey] = variant;
                }
            }
        }

        private void AddNormalEndpointToServiceMap(MergedEndpoint mergedEndpoint, string regionName, string serviceName)
        {
            string template = mergedEndpoint.Hostname;
            string dnsSuffix = partition.DnsSuffix;
            string hostname = template.Replace("{service}", serviceName)
                                 .Replace("{region}", regionName)
                                 .Replace("{dnsSuffix}", dnsSuffix);


            string authRegion = null;
            var credentialScope = mergedEndpoint.CredentialScope;
            if (credentialScope != null)
            {
                authRegion = DetermineAuthRegion(credentialScope);
            }

            RegionEndpoint.Endpoint endpoint = new RegionEndpoint.Endpoint(hostname, authRegion);
            _serviceMap.Add(serviceName, endpoint);
        }

        private void AddVariantEndpointsToServiceMap(MergedEndpoint mergedEndpoint, string regionName, string serviceName, Dictionary<HashSet<string>, EndpointsPartitionDefaultsVariant> mergedVariants)
        {
            string authRegion = null;
            var credentialScope = mergedEndpoint.CredentialScope;
            if (credentialScope != null)
            {
                authRegion = DetermineAuthRegion(credentialScope);
            }

            foreach (var tagsKey in mergedVariants.Keys)
            {
                var variantHostnameTemplate = mergedVariants[tagsKey].Hostname;
                if (string.IsNullOrEmpty(variantHostnameTemplate))
                {
                    throw new AmazonClientException($"Unable to determine the hostname for {serviceName} with variants [{string.Join(", ", tagsKey.ToArray())}].");
                }

                if (variantHostnameTemplate.Contains("{region}") && string.IsNullOrEmpty(regionName))
                {
                    throw new AmazonClientException($"Unable to determine the region for {serviceName} with variants [{string.Join(", ", tagsKey.ToArray())}].");
                }

                var variantDnsSuffix = mergedVariants[tagsKey].DnsSuffix ?? partition.DnsSuffix;
                if (variantHostnameTemplate.Contains("{dnsSuffix}") && string.IsNullOrEmpty(variantDnsSuffix))
                {
                    throw new AmazonClientException($"Unable to determine the dnsSuffix for {serviceName} with variants [{string.Join(", ", tagsKey.ToArray())}].");
                }

                var variantHostname = variantHostnameTemplate.Replace("{service}", serviceName)
                                     .Replace("{region}", regionName)
                                     .Replace("{dnsSuffix}", variantDnsSuffix);

                _serviceMap.Add(serviceName, new RegionEndpoint.Endpoint(variantHostname, authRegion), tagsKey);
            }
        }

        private static string DetermineAuthRegion(MergedEndpointCredentialScope credentialScope)
        {
            string authRegion = null;
            if (credentialScope.Region != null)
            {
                authRegion = credentialScope.Region;
            }
            return authRegion;
        }

        /// <summary>
        /// Builds the set used to identify a specific endpoint variant
        /// </summary>
        /// <param name="dualStack">Whether to use a dualstack (IPv6 enabled) endpoint</param>
        /// <param name="fips">Whether to use a FIPS-compliant endpoint</param>
        /// <returns>Set used to identify the combined variant in endpoints.json</returns>
        private static HashSet<string> BuildVariantHashSet(GetEndpointForServiceOptions options)
        {
            options = options ?? new GetEndpointForServiceOptions();

            if (!options.DualStack && !options.FIPS)
            {
                return null;
            }

            var variants = new HashSet<string>();

            if (options.DualStack)
            {
                variants.Add("dualstack");
            }
            if (options.FIPS)
            {
                variants.Add("fips");
            }

            return variants;
        }

        class ServiceMap
        {
            /// <summary>
            /// Stores the plain endpoints for each service in the current region
            /// </summary>
            private Dictionary<string, RegionEndpoint.Endpoint> _serviceMap = new Dictionary<string, RegionEndpoint.Endpoint>();

            /// <summary>
            /// Stores the variants for each service in the current region, identified by the set of variant tags.
            /// </summary>
            private Dictionary<string, Dictionary<HashSet<string>, RegionEndpoint.Endpoint>> _variantMap = new Dictionary<string, Dictionary<HashSet<string>, RegionEndpoint.Endpoint>>();

            public void Add(string serviceName, RegionEndpoint.Endpoint endpoint, HashSet<string> variants = null)
            {
                if (variants == null || variants.Count == 0)
                {
                    _serviceMap.Add(serviceName, endpoint);
                }
                else
                {
                    if (!_variantMap.ContainsKey(serviceName) || _variantMap[serviceName] == null)
                    {
                        _variantMap.Add(serviceName, new Dictionary<HashSet<string>, RegionEndpoint.Endpoint>(HashSet<string>.CreateSetComparer()));
                    }
                    _variantMap[serviceName].Add(variants, endpoint);
                }
            }

            public bool TryGetEndpoint(string serviceName, HashSet<string> variants, out RegionEndpoint.Endpoint endpoint)
            {
                if (variants == null || variants.Count == 0)
                {
                    return _serviceMap.TryGetValue(serviceName, out endpoint);
                }
                else
                {
                    if (!_variantMap.ContainsKey(serviceName))
                    {
                        endpoint = default(RegionEndpoint.Endpoint);
                        return false;
                    }
                    else
                    {
                        return _variantMap[serviceName].TryGetValue(variants, out endpoint);
                    }
                }
            }
        }
    }


    [Obsolete("This class is obsoleted because as of version 3.7.100 endpoint is resolved using a newer system that uses request level parameters to resolve the endpoint, use the service-specific client.DetermineServiceOperationEndPoint method instead.")]
    public class RegionEndpointProviderV3 : IRegionEndpointProvider, IDisposable
    {
        private Dictionary<string, IRegionEndpoint> _regionEndpointMap = new Dictionary<string, IRegionEndpoint>();
        private Dictionary<string, IRegionEndpoint> _nonStandardRegionNameToObjectMap = new Dictionary<string, IRegionEndpoint>();


        private ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim();

        private static string GetUnknownRegionDescription(string regionName)
        {
            if (regionName.StartsWith("cn-", StringComparison.OrdinalIgnoreCase) ||
                regionName.EndsWith("cn-global", StringComparison.OrdinalIgnoreCase))
            {
                return "China (Unknown)";
            }
            else
            {
                return "Unknown";
            }
        }

        private static bool IsRegionInPartition(string regionName, EndpointsPartition partition, out string description)
        {
            var regionsData = partition.Regions;
            string regionPattern = (string)partition.RegionRegex;

            // see if the region name is a real region 
            if (regionsData[regionName] != null)
            {
                description = (string)regionsData[regionName].Description;
                return true;
            }

            // see if the region is global region by concatenating the partition and "-global" to construct the global name 
            // for the partition
            else if (regionName.Equals(string.Concat((string)partition.Partition, "-global"), StringComparison.OrdinalIgnoreCase))
            {
                description = "Global";
                return true;
            }

            // no region key in the entry, but it matches the pattern in this partition.
            // we can try to construct an endpoint based on the heuristics described in endpoints.json
            else if (new Regex(regionPattern).Match(regionName).Success)
            {
                description = GetUnknownRegionDescription(regionName);
                return true;
            }

            else
            {
                description = GetUnknownRegionDescription(regionName);
                return false;
            }
        }

        public IRegionEndpoint GetRegionEndpoint(string regionName)
        {
            try
            {
                try
                {
                    _readerWriterLock.EnterReadLock();

                    IRegionEndpoint endpoint;
                    if (_regionEndpointMap.TryGetValue(regionName, out endpoint))
                    {
                        return endpoint;
                    }
                }
                finally
                {
                    if (_readerWriterLock.IsReadLockHeld)
                    {
                        _readerWriterLock.ExitReadLock();
                    }
                }

                try
                {
                    _readerWriterLock.EnterWriteLock();

                    IRegionEndpoint endpoint;
                    // Check again to see if region is in cache in case another thread got the write lock before and filled the cache.
                    if (_regionEndpointMap.TryGetValue(regionName, out endpoint))
                    {
                        return endpoint;
                    }

                    var partitions = Endpoints.Reference.Partitions;
                    foreach (var partition in partitions)
                    {
                        string description;
                        if (IsRegionInPartition(regionName, partition, out description))
                        {
                            endpoint = new RegionEndpointV3(regionName, description, partition, partition.Services);
                            _regionEndpointMap.Add(regionName, endpoint);
                            return endpoint;
                        }
                    }
                }
                finally
                {
                    if (_readerWriterLock.IsWriteLockHeld)
                    {
                        _readerWriterLock.ExitWriteLock();
                    }
                }
            }
            catch (Exception)
            {
                throw new AmazonClientException("Invalid endpoint.json format.");
            }

            return GetNonstandardRegionEndpoint(regionName);
        }

        /// <summary>
        /// This region name is non-standard.  Search the whole endpoints.json file to
        /// determine the partition this region is in.
        /// </summary>
        private IRegionEndpoint GetNonstandardRegionEndpoint(string regionName)
        {
            try
            {
                _readerWriterLock.EnterReadLock();

                IRegionEndpoint regionEndpoint;
                if (_nonStandardRegionNameToObjectMap.TryGetValue(regionName, out regionEndpoint))
                {
                    return regionEndpoint;
                }
            }
            finally
            {
                if (_readerWriterLock.IsReadLockHeld)
                {
                    _readerWriterLock.ExitReadLock();
                }
            }

            try
            {
                _readerWriterLock.EnterWriteLock();

                IRegionEndpoint regionEndpoint;
                // Check again to see if region is in cache in case another thread got the write lock before and filled the cache.
                if (_nonStandardRegionNameToObjectMap.TryGetValue(regionName, out regionEndpoint))
                {
                    return regionEndpoint;
                }

                // default to "aws" partition
                var partitionData = Endpoints.Reference.Partitions[0];
                string regionDescription = GetUnknownRegionDescription(regionName);
                var servicesData = partitionData.Services;
                bool foundContainingPartition = false;

                const string validRegionRegexStr = @"^[a-zA-Z0-9]([a-zA-Z0-9\-]*[a-zA-Z0-9])?$";
                var match = Regex.Match(regionName, validRegionRegexStr, RegexOptions.Compiled);

                foreach (var partition in Endpoints.Reference.Partitions)
                {
                    var partitionServices = partition.Services;
                    foreach (string service in partitionServices.Keys)
                    {
                        if (partitionServices[service] != null)
                        {
                            var serviceData = partitionServices[service];
                            if (serviceData != null && serviceData.Endpoints[regionName] != null)
                            {
                                partitionData = partition;
                                servicesData = partitionServices;
                                foundContainingPartition = true;
                                break;
                            }
                        }
                    }
                }
                if (!foundContainingPartition && !match.Success)
                {
                    throw new ArgumentException("Invalid region endpoint provided");
                }
                regionEndpoint = new RegionEndpointV3(regionName, regionDescription, partitionData, servicesData);
                _nonStandardRegionNameToObjectMap.Add(regionName, regionEndpoint);
                return regionEndpoint;
            }
            finally
            {
                if (_readerWriterLock.IsWriteLockHeld)
                {
                    _readerWriterLock.ExitWriteLock();
                }
            }
        }
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _readerWriterLock.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
