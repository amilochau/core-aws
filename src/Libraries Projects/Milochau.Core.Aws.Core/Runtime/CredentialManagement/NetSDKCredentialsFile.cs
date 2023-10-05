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
using Amazon.Runtime.Internal.Settings;
using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using Amazon.Util.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace Amazon.Runtime.CredentialManagement
{
    /// <summary>
    /// This class allows profiles supporting AWSCredentials to be registered with
    /// the SDK so that they can later be reference by a profile name. The credential profiles will be available
    /// for use in the AWS Toolkit for Visual Studio and the AWS Tools for Windows PowerShell.
    /// <para>
    /// The credentials are stored under the current users AppData folder encrypted using Windows Data Protection API.
    /// </para>
    /// <para>
    /// This class is not threadsafe.
    /// </para>
    /// </summary>
    public class NetSDKCredentialsFile : ICredentialProfileSource
    {
        public const string DefaultProfileName = "Default";

        // Values kept from ProfileManager to support backward compatibility.
        private const string AWSCredentialsProfileType = "AWS";
        private const string SAMLRoleProfileType = "SAML";

        private const string DefaultConfigurationModeNameField = "DefaultsMode";

        private const string RegionField = "Region";

        private const string EndpointDiscoveryEnabledField = "EndpointDiscoveryEnabled";
        private const string S3UseArnRegionField = "S3UseArnRegion";

        private const string StsRegionalEndpointsField = "StsRegionalEndpoints";

        private const string S3RegionalEndpointField = "S3RegionalEndpoint";

        private const string S3DisableMultiRegionAccessPointsField = "S3DisableMultiRegionAccessPoints";

        private const string RetryModeField = "RetryMode";
        private const string MaxAttemptsField = "MaxAttempts";

        private const string SsoAccountId = "sso_account_id";
        private const string SsoRegion = "sso_region";
        private const string SsoRoleName = "sso_role_name";
        private const string SsoStartUrl = "sso_start_url";
        private const string SsoSession = "sso_session";
        private const string EndpointUrlField = "endpoint_url";
        private const string ServicesField = "services";
        private const string IgnoreConfiguredEndpointUrlsField = "ignore_configured_endpoint_urls";
        private static readonly HashSet<string> ReservedPropertyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            SettingsConstants.DisplayNameField,
            SettingsConstants.ProfileTypeField,
            DefaultConfigurationModeNameField,
            RegionField,
            EndpointDiscoveryEnabledField,
            S3UseArnRegionField,
            StsRegionalEndpointsField,
            S3RegionalEndpointField,
            S3DisableMultiRegionAccessPointsField,
            RetryModeField,
            MaxAttemptsField,
            SsoAccountId,
            SsoRegion,
            SsoRoleName,
            SsoStartUrl,
            EndpointUrlField,
            ServicesField,
            IgnoreConfiguredEndpointUrlsField
        };

        private static readonly CredentialProfilePropertyMapping PropertyMapping =
            new CredentialProfilePropertyMapping(
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "AccessKey", SettingsConstants.AccessKeyField },
                    { "CredentialSource", SettingsConstants.CredentialSourceField },
                    { "EndpointName", SettingsConstants.EndpointNameField },
                    { "ExternalID", SettingsConstants.ExternalIDField},
                    { "MfaSerial", SettingsConstants.MfaSerialField},
                    { "RoleArn", SettingsConstants.RoleArnField },
                    { "RoleSessionName", SettingsConstants.RoleSessionName},
                    { "SecretKey", SettingsConstants.SecretKeyField },
                    { "SourceProfile", SettingsConstants.SourceProfileField },
                    { "Token", SettingsConstants.SessionTokenField },
                    { "UserIdentity", SettingsConstants.UserIdentityField },
                    { "Services", SettingsConstants.Services },
                    { "EndpointUrl", SettingsConstants.EndpointUrl },
                    // Not implemented for NetSDKCredentials. Applicable only
                    // for SharedCredentials
                    { "CredentialProcess" , SettingsConstants.CredentialProcess },
                    { "WebIdentityTokenFile", SettingsConstants.WebIdentityTokenFile },
                    { nameof(CredentialProfileOptions.SsoAccountId), SsoAccountId },
                    { nameof(CredentialProfileOptions.SsoRegion), SsoRegion },
                    { nameof(CredentialProfileOptions.SsoRoleName), SsoRoleName },
                    { nameof(CredentialProfileOptions.SsoStartUrl), SsoStartUrl },
                    { nameof(CredentialProfileOptions.SsoSession), SsoSession}
                }
            );

        private readonly NamedSettingsManager _settingsManager;

        public NetSDKCredentialsFile()
        {
            _settingsManager = new NamedSettingsManager(SettingsConstants.RegisteredProfiles);
        }

        /// <summary>
        /// Get the profile with the name given, if it exists in this store.
        /// </summary>
        /// <param name="profileName">The name of the profile to find.</param>
        /// <param name="profile">The profile, if it was found, null otherwise</param>
        /// <returns>True if the profile was found, false otherwise.</returns>
        public bool TryGetProfile(string profileName, out CredentialProfile profile)
        {
            Dictionary<string, string> properties;
            string uniqueKeyStr;
            if (_settingsManager.TryGetObject(profileName, out uniqueKeyStr, out properties))
            {
                try
                {
                    CredentialProfileOptions profileOptions;
                    Dictionary<string, string> userProperties;
                    Dictionary<string, string> reservedProperties;
                    PropertyMapping.ExtractProfileParts(
                        properties, 
                        ReservedPropertyNames,
                        out profileOptions,
                        out reservedProperties,
                        out userProperties);

                    string defaultConfigurationModeName;
                    reservedProperties.TryGetValue(DefaultConfigurationModeNameField, out defaultConfigurationModeName);

                    string regionString;
                    RegionEndpoint region = null;
                    if (reservedProperties.TryGetValue(RegionField, out regionString))
                    {
                        region = RegionEndpoint.GetBySystemName(regionString);
                    }

                    Guid? uniqueKey = null;
                    if (!GuidUtils.TryParseNullableGuid(uniqueKeyStr, out uniqueKey))
                    {
                        profile = null;
                        return false;
                    }

                    string endpointDiscoveryEnabledString;
                    bool? endpointDiscoveryEnabled = null;
                    if (reservedProperties.TryGetValue(EndpointDiscoveryEnabledField, out endpointDiscoveryEnabledString))
                    {
                        bool endpointDiscoveryEnabledOut;
                        if (!bool.TryParse(endpointDiscoveryEnabledString, out endpointDiscoveryEnabledOut))
                        {
                            profile = null;
                            return false;
                        }

                        endpointDiscoveryEnabled = endpointDiscoveryEnabledOut;
                    }

                    StsRegionalEndpointsValue? stsRegionalEndpoints = null;
                    if (reservedProperties.TryGetValue(StsRegionalEndpointsField, out var stsRegionalEndpointsString))
                    {
                        if (!Enum.TryParse<StsRegionalEndpointsValue>(stsRegionalEndpointsString, true, out var tempStsRegionalEndpoints))
                        {
                            profile = null;
                            return false;
                        }
                        stsRegionalEndpoints = tempStsRegionalEndpoints;
                    }

                    string s3UseArnRegionString;
                    bool? s3UseArnRegion = null;
                    if(reservedProperties.TryGetValue(S3UseArnRegionField, out s3UseArnRegionString))
                    {
                        bool s3UseArnRegionOut;
                        if (!bool.TryParse(s3UseArnRegionString, out s3UseArnRegionOut))
                        {
                            profile = null;
                            return false;
                        }

                        s3UseArnRegion = s3UseArnRegionOut;
                    }

                    string s3DisableMultiRegionAccessPointsString;
                    bool? s3DisableMultiRegionAccessPoints = null;
                    if (reservedProperties.TryGetValue(S3DisableMultiRegionAccessPointsField, out s3DisableMultiRegionAccessPointsString))
                    {
                        bool s3DisableMultiRegionAccessPointsOut;
                        if (!bool.TryParse(s3DisableMultiRegionAccessPointsString, out s3DisableMultiRegionAccessPointsOut))
                        {
                            profile = null;
                            return false;
                        }

                        s3DisableMultiRegionAccessPoints = s3DisableMultiRegionAccessPointsOut;
                    }
                    
                    S3UsEast1RegionalEndpointValue? s3RegionalEndpoint = null;
                    if (reservedProperties.TryGetValue(S3RegionalEndpointField, out var s3RegionalEndpointString))
                    {
                        if (!Enum.TryParse<S3UsEast1RegionalEndpointValue>(s3RegionalEndpointString, true, out var tempS3RegionalEndpoint))
                        {
                            profile = null;
                            return false;
                        }
                        s3RegionalEndpoint = tempS3RegionalEndpoint;
                    }

                    RequestRetryMode? requestRetryMode = null;
                    if (reservedProperties.TryGetValue(RetryModeField, out var retryModeString))
                    {
                        if (!Enum.TryParse<RequestRetryMode>(retryModeString, true, out var tempRetryMode))
                        {
                            profile = null;
                            return false;
                        }
                        requestRetryMode = tempRetryMode;
                    }

                    int? maxAttempts = null;
                    if (reservedProperties.TryGetValue(MaxAttemptsField, out var maxAttemptsString))
                    {
                        if (!int.TryParse(maxAttemptsString, out var maxAttemptsTemp) || maxAttemptsTemp <= 0)
                        {
                            Logger.GetLogger(GetType()).InfoFormat("Invalid value {0} for {1} in profile {2}. A positive integer is expected.", maxAttemptsString, MaxAttemptsField, profileName);
                            profile = null;
                            return false;
                        }

                        maxAttempts = maxAttemptsTemp;
                    }

                    profile = new CredentialProfile(profileName, profileOptions)
                    {
                        UniqueKey = uniqueKey,
                        Properties = userProperties,
                        Region = region,
                        DefaultConfigurationModeName = defaultConfigurationModeName,
                        EndpointDiscoveryEnabled = endpointDiscoveryEnabled,
                        StsRegionalEndpoints = stsRegionalEndpoints,
                        S3UseArnRegion = s3UseArnRegion,
                        S3RegionalEndpoint = s3RegionalEndpoint,
                        S3DisableMultiRegionAccessPoints = s3DisableMultiRegionAccessPoints,
                        RetryMode = requestRetryMode,
                        MaxAttempts = maxAttempts,
                    };
                    return true;
                }
                catch (ArgumentException)
                {
                    profile = null;
                    return false;
                }
            }
            else
            {
                profile = null;
                return false;
            }
        }
    }
}