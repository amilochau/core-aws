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
using Amazon.Runtime.Internal;
using Amazon.Runtime.CredentialManagement.Internal;
using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Amazon.Runtime.CredentialManagement
{
    /// <summary>
    /// A named group of options that are persisted and used to obtain AWSCredentials.
    /// </summary>
    public class CredentialProfile
    {
        private Dictionary<string, string> _properties;

        private Dictionary<string, Dictionary<string, string>> _nestedProperties;
        /// <summary>
        /// An optional Dictionary of Dictionaries that can contain nested properties.
        /// For example: in a configuration file like so:
        /// [profile foo]
        /// s3 = 
        ///   max_retries = 10
        ///   max_concurrent_requests = 50
        /// NestedProperties contains: {{s3:{max_retries:10},{max_concurrent_requests:50}}}
        /// </summary>
        internal Dictionary<string, Dictionary<string, string>> NestedProperties
        {
            get => _nestedProperties ?? (_nestedProperties = new Dictionary<string, Dictionary<string,string>>());
            set => _nestedProperties = value;
        }
        /// <summary>
        /// The name of the CredentialProfile
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The options to be used to create AWSCredentials.
        /// </summary>
        public CredentialProfileOptions Options { get; private set; }

        /// <summary>
        /// The region to be used with this CredentialProfile
        /// </summary>
        public RegionEndpoint Region { get; set; }

        /// <summary>
        /// The unique key for this CredentialProfile.
        /// This key is used by the Visual Studio Toolkit to associate external artifacts with this profile.
        /// </summary>
        internal Guid? UniqueKey { get; set; }

        /// <summary>
        /// The desired <see cref="DefaultConfiguration.Name"/> that
        /// <see cref="IDefaultConfigurationProvider"/> should use.
        /// <para />
        /// If this is null/empty, then the <see cref="DefaultConfigurationMode.Legacy"/> Mode will be used.
        /// </summary>
        public string DefaultConfigurationModeName { get; set; }

        /// <summary>
        /// The endpoint discovery enabled value for this CredentialProfile
        /// </summary>
        public bool? EndpointDiscoveryEnabled { get; set; }

        /// <summary>
        /// The request retry mode  as legacy, standard, or adaptive
        /// </summary>
        public RequestRetryMode? RetryMode { get; set; }

        /// <summary>
        /// Specified how many HTTP requests an SDK should make for a single
        /// SDK operation invocation before giving up.
        /// </summary>
        public int? MaxAttempts { get; set; }

        /// <summary>
        /// Endpoint of the EC2 Instance Metadata Service
        /// </summary>
        public string EC2MetadataServiceEndpoint { get; set; }

        /// <summary>
        /// Internet protocol version to be used for communicating with the EC2 Instance Metadata Service
        /// </summary>
        public EC2MetadataServiceEndpointMode? EC2MetadataServiceEndpointMode { get; set; }

        /// <summary>
        /// Configures the endpoint calculation to go to a dual stack (ipv6 enabled) endpoint
        /// for the configured region.
        /// </summary>
        public bool? UseDualstackEndpoint { get; set; }

        /// <summary>
        /// Configures the endpoint calculation to go to a FIPS (https://aws.amazon.com/compliance/fips/) endpoint
        /// for the configured region.
        /// </summary>
        public bool? UseFIPSEndpoint { get; set; }

        /// <summary>
        /// If this flag is set to true, the SDK will ignore the configured endpoint urls set in the
        /// configuration file.
        /// </summary>
        public bool? IgnoreConfiguredEndpointUrls { get; set; }
        /// <summary>
        /// This configures the global endpoint URL for a profile. This cannot be used in a services section 
        /// and will be ignored if set in the services section.
        /// </summary>
        public string EndpointUrl { get; set; }

        /// <summary>
        /// Controls whether request payloads are automatically compressed for supported operations.
        /// This setting only applies to operations that support compression.
        /// The default value is "false". Set to "true" to disable compression.
        /// </summary>
        public bool? DisableRequestCompression { get; set; }

        /// <summary>
        /// Minimum size in bytes that a request body should be to trigger compression.
        /// </summary>
        public long? RequestMinCompressionSizeBytes { get; set; }

        /// <summary>
        /// An optional dictionary of name-value pairs stored with the CredentialProfile
        /// </summary>
        internal Dictionary<string, string> Properties
        {
            get => _properties ?? (_properties = new Dictionary<string, string>());
            set => _properties = value;
        }

        /// <summary>
        /// True if the properties of the Options object can be converted into AWSCredentials, false otherwise.
        /// See <see cref="CredentialProfileOptions"/> for more details about which options are available.
        /// </summary>
        public bool CanCreateAWSCredentials => ProfileType.HasValue;

        /// <summary>
        /// The CredentialProfileType of this CredentialProfile, if one applies.
        /// </summary>
        internal CredentialProfileType? ProfileType => CredentialProfileTypeDetector.DetectProfileType(Options);

        /// <summary>
        /// Construct a new CredentialProfile.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="profileOptions"></param>
        public CredentialProfile(string name, CredentialProfileOptions profileOptions)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name must not be null or empty.");
            }

            Options = profileOptions ?? throw new ArgumentNullException("profileOptions");
            Name = name;
        }

        private string GetPropertiesString()
        {
            return "{" + string.Join(",", Properties.OrderBy(p=>p.Key).Select(p => p.Key + "=" + p.Value).ToArray()) + "}";
        }

        public override string ToString()
        {
            return "[Name=" + Name + "," +
                "Options = " + Options + "," +
                "Region = " + (Region == null ? "" : Region.SystemName) + "," +
                "Properties = " + GetPropertiesString() + "," +
                "ProfileType = " + ProfileType + "," +
                "UniqueKey = " + UniqueKey + "," +
                "CanCreateAWSCredentials = " + CanCreateAWSCredentials + "," +
                "RetryMode= " + RetryMode + "," +
                "MaxAttempts= " + MaxAttempts +
                "]";
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            var p = obj as CredentialProfile;
            if (p == null)
                return false;

            return AWSSDKUtils.AreEqual(
                new object[] { Name, Options, Region, ProfileType, CanCreateAWSCredentials, UniqueKey },
                new object[] { p.Name, p.Options, p.Region, p.ProfileType, p.CanCreateAWSCredentials, p.UniqueKey }) &&
                AWSSDKUtils.DictionariesAreEqual(Properties, p.Properties);
        }

        public override int GetHashCode()
        {
            return Hashing.Hash(Name, Options, Region, ProfileType, CanCreateAWSCredentials, GetPropertiesString(), UniqueKey);
        }
    }
}
