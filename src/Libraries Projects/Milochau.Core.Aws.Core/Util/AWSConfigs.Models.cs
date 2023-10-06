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

using System.Collections.Generic;

namespace Amazon.Util
{

    #region Basic sections

    /// <summary>
    /// Settings for configuring a proxy for the SDK to use.
    /// </summary>
    public partial class ProxyConfig
    {
        /// <summary>
        /// The username to authenticate with the proxy server.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password to authenticate with the proxy server.
        /// </summary>
        public string Password { get; set; }

        internal ProxyConfig()
        {
        }
    }

    /// <summary>
    /// Settings for logging in the SDK.
    /// </summary>
    public partial class LoggingConfig
    {
        // Default limit for response logging is 1 KB.
        public static readonly int DefaultLogResponsesSizeLimit = 1024; 

        private LoggingOptions _logTo;

        /// <summary>
        /// Logging destination.
        /// </summary>
        public LoggingOptions LogTo
        {
            get { return _logTo; }
            set
            {
                _logTo = value;
                AWSConfigs.OnPropertyChanged(AWSConfigs.LoggingDestinationProperty);
            }
        }

        /// <summary>        
        /// Gets or sets the size limit in bytes for logged responses.
        /// If logging for response body is enabled, logged response
        /// body is limited to this size. The default limit is 1KB.
        /// </summary>
        public int LogResponsesSizeLimit { get; set; }

        internal LoggingConfig()
        {
            LogTo = LoggingOptions.None;
            LogResponsesSizeLimit = DefaultLogResponsesSizeLimit;
        }
    }
    #endregion

}
