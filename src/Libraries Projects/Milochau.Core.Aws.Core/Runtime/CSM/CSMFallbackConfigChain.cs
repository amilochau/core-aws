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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Util;
using Amazon.Util.Internal;

namespace Amazon.Runtime.Internal
{
    /// <summary>
    /// This class gets the CSM configuration if set.
    /// The priority goes
    /// 1. App.config / instantiate on the AWSConfigs class
    /// 2. Environment variable
    /// 3. Shared Profile
    /// </summary>
    public class CSMFallbackConfigChain
    {
        private readonly ILogger LOGGER = Logger.GetLogger(typeof(CSMFallbackConfigChain));

        public delegate void ConfigurationSource();
        public List<ConfigurationSource> AllGenerators { get; set; }

        internal bool IsConfigSet { get; set; }
        public string ConfigSource { get; set; }
        public CSMConfiguration CSMConfiguration { get; internal set; }

        public CSMFallbackConfigChain()
        {
            CSMConfiguration = new CSMConfiguration();
            AllGenerators = new List<ConfigurationSource>
            {
                () => new AppConfigCSMConfigs(this),
                () => new EnvironmentVariableCSMConfigs(this),
            };
        }

        public CSMConfiguration GetCSMConfig()
        {
            foreach (var generator in AllGenerators)
            {
                try
                {
                    generator();
                    if (IsConfigSet)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    LOGGER.Error(e, "Error looking for CSM configuration");
                }
            }
            return CSMConfiguration;
        }
    }

    /// <summary>
    /// Determine CSM configs from environment variables.
    /// </summary>
    public class EnvironmentVariableCSMConfigs
    {
        private const string CSM_ENABLED = "AWS_CSM_ENABLED";
        private const string CSM_CLIENTID = "AWS_CSM_CLIENT_ID";
        private const string CSM_HOST = "AWS_CSM_HOST";
        private const string CSM_PORT = "AWS_CSM_PORT";

        private IEnvironmentVariableRetriever environmentRetriever { get; set; } = EnvironmentVariableSource.Instance.EnvironmentVariableRetriever;

        public EnvironmentVariableCSMConfigs(IEnvironmentVariableRetriever environmentRetriever, CSMFallbackConfigChain cSMFallbackConfigChain)
        {
            this.environmentRetriever = environmentRetriever;
            SetupConfiguration(cSMFallbackConfigChain);
        }
        public EnvironmentVariableCSMConfigs(CSMFallbackConfigChain cSMFallbackConfigChain)
        {
            SetupConfiguration(cSMFallbackConfigChain);
        }

        private void SetupConfiguration(CSMFallbackConfigChain cSMFallbackConfigChain)
        {
            var csmConfiguration = cSMFallbackConfigChain.CSMConfiguration;
            var logger = Logger.GetLogger(typeof(EnvironmentVariableCSMConfigs));
            var enabled = environmentRetriever.GetEnvironmentVariable(CSM_ENABLED);
            if (string.IsNullOrEmpty(enabled))
            {
                return;
            }
            cSMFallbackConfigChain.IsConfigSet = true;
            cSMFallbackConfigChain.ConfigSource = "environment variable";
            bool csm_enabled;
            if (bool.TryParse(enabled, out csm_enabled))
            {
                csmConfiguration.Enabled = csm_enabled;
            }
            else
            {
                throw new AmazonClientException("Unexpected environment variable value type " + CSM_ENABLED + ". Set a valid boolean value.");
            }
            if (csmConfiguration.Enabled)
            {
                csmConfiguration.ClientId = environmentRetriever.GetEnvironmentVariable(CSM_CLIENTID) ?? csmConfiguration.ClientId;
                string portValue = environmentRetriever.GetEnvironmentVariable(CSM_PORT);
                if (!string.IsNullOrEmpty(portValue))
                {
                    int port;
                    if (int.TryParse(portValue, out port))
                    {
                        csmConfiguration.Port = port;
                    }
                    else
                    {
                        throw new AmazonClientException("Unexpected environment variable value type " + CSM_PORT + ". Set a valid integer value.");
                    }
                }

                string hostValue = environmentRetriever.GetEnvironmentVariable(CSM_HOST);
                if (!string.IsNullOrEmpty(hostValue))
                {
                    csmConfiguration.Host = hostValue;
                }
            }

            logger.InfoFormat(string.Format(CultureInfo.InvariantCulture,
                "CSM configurations found using environment variable. values are CSM enabled = {0}, host = {1}, port = {2}, clientid = {3}",
                csmConfiguration.Enabled, csmConfiguration.Host, csmConfiguration.Port, csmConfiguration.ClientId));
        }
    }

    /// <summary>
    /// Determine CSM configs from AWSConfigs class.
    /// </summary>
    public class AppConfigCSMConfigs
    {
        public AppConfigCSMConfigs(CSMFallbackConfigChain cSMFallbackConfigChain)
        {
            var csmConfiguration = cSMFallbackConfigChain.CSMConfiguration;
            var logger = Logger.GetLogger(typeof(AppConfigCSMConfigs));
            if (!AWSConfigs.CSMConfig.CSMEnabled.HasValue)
            {
                return;
            }
            cSMFallbackConfigChain.IsConfigSet = true;
            if (!AWSConfigs.CSMConfig.CSMEnabled.GetValueOrDefault())
            {
                return;
            }
            cSMFallbackConfigChain.ConfigSource = "app config";
            csmConfiguration.Enabled = AWSConfigs.CSMConfig.CSMEnabled.GetValueOrDefault();
            if (csmConfiguration.Enabled)
            {
                if (!string.IsNullOrEmpty(AWSConfigs.CSMConfig.CSMClientId))
                {
                    csmConfiguration.ClientId = AWSConfigs.CSMConfig.CSMClientId;
                }
                csmConfiguration.Host = AWSConfigs.CSMConfig.CSMHost;
                csmConfiguration.Port = AWSConfigs.CSMConfig.CSMPort;

                logger.InfoFormat(string.Format(CultureInfo.InvariantCulture,
                    "CSM configurations found using application configuration file. values are CSM enabled = {0}, host = {1}, port = {2}, clientid = {3}",
                    csmConfiguration.Enabled, csmConfiguration.Host, csmConfiguration.Port, csmConfiguration.ClientId));
            }
        }
    }
}
