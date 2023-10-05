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

using Amazon.Runtime.Internal.Util;
using System.Collections.Generic;
using System.ComponentModel;

namespace Amazon.Runtime.Internal
{
    /// <summary>
    /// InternalConfiguration holds the cached SDK configuration values
    /// obtained from the environment and profile configuration
    /// factories. These configuration values are loaded internally and
    /// are not the same as user exposed AWSConfigs.
    /// </summary>
    public class InternalConfiguration
    {
        /// <summary>
        /// Flag indicating if Endpoint Discovery is enabled.
        /// </summary>
        public bool? EndpointDiscoveryEnabled { get; set; }
        
        /// <summary>
        /// The retry mode to use: Legacy, Standard, or Adaptive.
        /// </summary>
        public RequestRetryMode? RetryMode { get; set; }
        
        /// <summary>
        /// The max number of request attempts.
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
        /// The desired <see cref="DefaultConfiguration.Name"/> that
        /// <see cref="IDefaultConfigurationProvider"/> should use.
        /// <para />
        /// If this is null/empty, then the <see cref="DefaultConfigurationMode.Legacy"/> Mode will be used.
        /// </summary>
        public string DefaultConfigurationModeName { get; set; }

        /// <summary>
        /// Configures the endpoint calculation for a service to go to a dual stack (ipv6 enabled) endpoint
        /// for the configured region.
        /// </summary>
        public bool? UseDualstackEndpoint { get; set; }

        /// <summary>
        /// Configures the endpoint calculation to go to a FIPS (https://aws.amazon.com/compliance/fips/) endpoint
        /// for the configured region.
        /// </summary>
        public bool? UseFIPSEndpoint { get; set; }
    
        /// <summary>
        /// Configures the SDK To ignore configured endpoint URLs in the configuration files or environment variables.
        /// The environment variable overrides the value set in the configuration file.
        /// </summary>
        public bool? IgnoreConfiguredEndpointUrls { get; set; }


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
    }

    /// <summary>
    /// Determines the configuration values based on environment variables. If 
    /// no values is found for a configuration the value will be set to null.
    /// </summary>
    public class EnvironmentVariableInternalConfiguration : InternalConfiguration
    {
        private Logger _logger = Logger.GetLogger(typeof(EnvironmentVariableInternalConfiguration));

        public const string ENVIRONMENT_VARIABLE_AWS_ENABLE_ENDPOINT_DISCOVERY = "AWS_ENABLE_ENDPOINT_DISCOVERY";
        public const string ENVIRONMENT_VARIABLE_AWS_MAX_ATTEMPTS = "AWS_MAX_ATTEMPTS";
        public const string ENVIRONMENT_VARIABLE_AWS_RETRY_MODE = "AWS_RETRY_MODE";
        public const string ENVIRONMENT_VARIABLE_AWS_EC2_METADATA_SERVICE_ENDPOINT = "AWS_EC2_METADATA_SERVICE_ENDPOINT";
        public const string ENVIRONMENT_VARIABLE_AWS_EC2_METADATA_SERVICE_ENDPOINT_MODE = "AWS_EC2_METADATA_SERVICE_ENDPOINT_MODE";
        public const string ENVIRONMENT_VARIABLE_AWS_USE_DUALSTACK_ENDPOINT = "AWS_USE_DUALSTACK_ENDPOINT";
        public const string ENVIRONMENT_VARIABLE_AWS_USE_FIPS_ENDPOINT = "AWS_USE_FIPS_ENDPOINT";
        public const string ENVIRONMENT_VARIABLE_AWS_IGNORE_CONFIGURED_ENDPOINT_URLS = "AWS_IGNORE_CONFIGURED_ENDPOINT_URLS";
        public const string ENVIRONMENT_VARIABLE_AWS_DISABLE_REQUEST_COMPRESSION = "AWS_DISABLE_REQUEST_COMPRESSION";
        public const string ENVIRONMENT_VARIABLE_AWS_REQUEST_MIN_COMPRESSION_SIZE_BYTES = "AWS_REQUEST_MIN_COMPRESSION_SIZE_BYTES";

        /// <summary>
        /// Attempts to construct a configuration instance of configuration environment 
        /// variables. If an environment variable value isn't found then the individual value 
        /// for that environment variable will be null. If unable to obtain a value converter 
        /// to convert a configuration string to the appropriate type a InvalidOperationException 
        /// is thrown.
        /// </summary>
        public EnvironmentVariableInternalConfiguration()
        {
            EndpointDiscoveryEnabled = GetEnvironmentVariable(ENVIRONMENT_VARIABLE_AWS_ENABLE_ENDPOINT_DISCOVERY, s => (bool)new BooleanConverter().ConvertFromString(s));
            MaxAttempts = GetEnvironmentVariable(ENVIRONMENT_VARIABLE_AWS_MAX_ATTEMPTS, s => (int)new Int32Converter().ConvertFromString(s));
            RetryMode = GetEnvironmentVariable(ENVIRONMENT_VARIABLE_AWS_RETRY_MODE, s => (RequestRetryMode)new EnumConverter(typeof(RequestRetryMode)).ConvertFromString(s));
            EC2MetadataServiceEndpoint = GetEC2MetadataEndpointEnvironmentVariable();
            EC2MetadataServiceEndpointMode = GetEnvironmentVariable(ENVIRONMENT_VARIABLE_AWS_EC2_METADATA_SERVICE_ENDPOINT_MODE, s => (EC2MetadataServiceEndpointMode)new EnumConverter(typeof(EC2MetadataServiceEndpointMode)).ConvertFromString(s));
            UseDualstackEndpoint = GetEnvironmentVariable(ENVIRONMENT_VARIABLE_AWS_USE_DUALSTACK_ENDPOINT, s => (bool)new BooleanConverter().ConvertFromString(s));
            UseFIPSEndpoint = GetEnvironmentVariable(ENVIRONMENT_VARIABLE_AWS_USE_FIPS_ENDPOINT, s => (bool)new BooleanConverter().ConvertFromString(s));
            IgnoreConfiguredEndpointUrls = GetEnvironmentVariable(ENVIRONMENT_VARIABLE_AWS_IGNORE_CONFIGURED_ENDPOINT_URLS, false);
            DisableRequestCompression = GetEnvironmentVariable(ENVIRONMENT_VARIABLE_AWS_DISABLE_REQUEST_COMPRESSION, s => (bool)new BooleanConverter().ConvertFromString(s));
            RequestMinCompressionSizeBytes = GetEnvironmentVariable(ENVIRONMENT_VARIABLE_AWS_REQUEST_MIN_COMPRESSION_SIZE_BYTES, s => (long)new Int64Converter().ConvertFromString(s));
        }

        private bool GetEnvironmentVariable(string name, bool defaultValue)
        {
            if(!TryGetEnvironmentVariable(name, out var value))
            {
                return defaultValue;
            }
            try
            {
                return bool.Parse(value);
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);
                throw new FormatException(e.Message, e.InnerException);
                
            }
        }

        private bool TryGetEnvironmentVariable(string environmentVariableName, out string value)
        {
            value = Environment.GetEnvironmentVariable(environmentVariableName);
            if (string.IsNullOrEmpty(value))
            {
                _logger.InfoFormat($"The environment variable {environmentVariableName} was not set with a value.");
                value = null;
                return false;
            }

            return true;
        }

        private T? GetEnvironmentVariable<T>(string name, Func<string, T> converter) where T : struct
        {
            if (!TryGetEnvironmentVariable(name, out var value))
            {
                return null;
            }

            return converter(value);
        }

        /// <summary>
        /// Loads the EC2 Instance Metadata endpoint from the environment variable, and validates it is a well-formed uri
        /// </summary>
        /// <returns>Override EC2 instance metadata endpoint if valid, else an empty string</returns>
        private string GetEC2MetadataEndpointEnvironmentVariable()
        {
            if (!TryGetEnvironmentVariable(ENVIRONMENT_VARIABLE_AWS_EC2_METADATA_SERVICE_ENDPOINT, out var rawValue))
            {
                return null;
            }

            if (!Uri.IsWellFormedUriString(rawValue, UriKind.Absolute))
            {
                throw new AmazonClientException($"The environment variable {ENVIRONMENT_VARIABLE_AWS_EC2_METADATA_SERVICE_ENDPOINT} was set with value " +
                    $"{rawValue}, but it could not be parsed as a well-formed Uri.");
            }

            return rawValue;
        }
    }

    /// <summary>
    /// Probing mechanism to determine the configuration values from various sources.
    /// </summary>
    public static class FallbackInternalConfigurationFactory
    {
        private static InternalConfiguration _cachedConfiguration;

        static FallbackInternalConfigurationFactory()
        {
            Reset();
        }

        private delegate InternalConfiguration ConfigGenerator();

        /// <summary>
        /// Resets all the configuration values reloading as needed. This method will use
        /// the AWS_PROFILE environment variable if set to construct the instance. Otherwise 
        /// the default profile will be used.
        /// </summary>
        public static void Reset()
        {
            //Preload configurations that are fast or pull all the values at the same time. Slower configurations
            //should be called for specific values dynamically.
            InternalConfiguration environmentVariablesConfiguration =  new EnvironmentVariableInternalConfiguration();
            InternalConfiguration profileConfiguration = new InternalConfiguration();

            _cachedConfiguration = new InternalConfiguration();
            var standardGenerators = new List<ConfigGenerator>
            {
                () => environmentVariablesConfiguration,
                () => profileConfiguration,
            };

            //Find the priority first ordered config value for each property
            _cachedConfiguration.DefaultConfigurationModeName = SeekString(standardGenerators, c => c.DefaultConfigurationModeName, defaultValue: null);
            _cachedConfiguration.EndpointDiscoveryEnabled = SeekValue(standardGenerators, (c) => c.EndpointDiscoveryEnabled);
            _cachedConfiguration.RetryMode = SeekValue(standardGenerators, (c) => c.RetryMode);
            _cachedConfiguration.MaxAttempts = SeekValue(standardGenerators, (c) => c.MaxAttempts);
            _cachedConfiguration.EC2MetadataServiceEndpoint = SeekString(standardGenerators, (c) => c.EC2MetadataServiceEndpoint);
            _cachedConfiguration.EC2MetadataServiceEndpointMode = SeekValue(standardGenerators, (c) => c.EC2MetadataServiceEndpointMode);
            _cachedConfiguration.UseDualstackEndpoint = SeekValue(standardGenerators, (c) => c.UseDualstackEndpoint);
            _cachedConfiguration.UseFIPSEndpoint = SeekValue(standardGenerators, (c) => c.UseFIPSEndpoint);
            _cachedConfiguration.IgnoreConfiguredEndpointUrls = SeekValue(standardGenerators, (c) => c.IgnoreConfiguredEndpointUrls);

            _cachedConfiguration.DisableRequestCompression = SeekValue(standardGenerators, (c) => c.DisableRequestCompression);
            _cachedConfiguration.RequestMinCompressionSizeBytes = SeekValue(standardGenerators, (c) => c.RequestMinCompressionSizeBytes);
        }        
                
        private static T? SeekValue<T>(List<ConfigGenerator> generators, Func<InternalConfiguration, T?> getValue) where T : struct
        {
            //Look for the configuration value stopping at the first generator that returns the expected value.
            foreach (var generator in generators)
            {
                var configuration = generator();
                T? value = getValue(configuration);
                if (value.HasValue)
                {
                    return value;
                }
            }

            return null;
        }

        private static string SeekString(List<ConfigGenerator> generators, Func<InternalConfiguration, string> getValue, string defaultValue = "")
        {
            //Look for the configuration value stopping at the first generator that returns the expected value.
            foreach (var generator in generators)
            {
                var configuration = generator();
                string value = getValue(configuration);
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Flag that specifies if endpoint discovery is enabled, disabled, 
        /// or not set.
        /// </summary>
        public static bool? EndpointDiscoveryEnabled
        {
            get
            {
                return _cachedConfiguration.EndpointDiscoveryEnabled;
            }
        }

        /// <summary>
        /// Flag that specifies which retry mode to use or if retry mode has 
        /// not been set.
        /// </summary>
        public static RequestRetryMode? RetryMode
        {
            get
            {
                return _cachedConfiguration.RetryMode;
            }
        }

        /// <summary>
        /// Flag that specifies the max number of request attempts or if max 
        /// attempts has not been set.
        /// </summary>
        public static int? MaxAttempts
        {
            get
            {
                return _cachedConfiguration.MaxAttempts;
            }
        }

        /// <summary>
        /// Endpoint of the EC2 Instance Metadata Service
        /// </summary>
        public static string EC2MetadataServiceEndpoint
        {
            get
            {
                return _cachedConfiguration.EC2MetadataServiceEndpoint;
            }
        }

        /// <summary>
        /// Internet protocol version to be used for communicating with the EC2 Instance Metadata Service
        /// </summary>
        public static EC2MetadataServiceEndpointMode? EC2MetadataServiceEndpointMode
        { 
            get
            {
                return _cachedConfiguration.EC2MetadataServiceEndpointMode;
            }
        }

        /// <inheritdoc cref="InternalConfiguration.DefaultConfigurationModeName"/>
        public static string DefaultConfigurationModeName
        {
            get
            {
                return _cachedConfiguration.DefaultConfigurationModeName;
            }
        }

        /// <summary>
        /// Configures the endpoint calculation to go to a dual stack (ipv6 enabled) endpoint
        /// for the configured region.
        /// </summary>
        public static bool? UseDualStackEndpoint
        {
            get
            {
                return _cachedConfiguration.UseDualstackEndpoint;
            }
        }

        /// <summary>
        /// Configures the endpoint calculation to go to a FIPS (https://aws.amazon.com/compliance/fips/) endpoint
        /// for the configured region.
        /// </summary>
        public static bool? UseFIPSEndpoint
        {
            get
            {
                return _cachedConfiguration.UseFIPSEndpoint;
            }
        }
        /// <summary>
        /// Configures the SDK To ignore configured endpoint URLs in the configuration files or environment variables.
        /// The environment variable overrides the value set in the configuration file.
        /// </summary>
        public static bool? IgnoreConfiguredEndpointUrls
        {
            get
            {
                return _cachedConfiguration.IgnoreConfiguredEndpointUrls;
            }
        }

        /// <summary>
        /// Controls whether request payloads are automatically compressed for supported operations.
        /// This setting only applies to operations that support compression.
        /// The default value is "false". Set to "true" to disable compression.
        /// </summary>
        public static bool? DisableRequestCompression {
            get
            {
                return _cachedConfiguration.DisableRequestCompression;
            }
        }

        /// <summary>
        /// Minimum size in bytes that a request body should be to trigger compression.
        /// </summary>
        public static long? RequestMinCompressionSizeBytes {
            get
            {
                return _cachedConfiguration.RequestMinCompressionSizeBytes;
            }
        }
    }
}
