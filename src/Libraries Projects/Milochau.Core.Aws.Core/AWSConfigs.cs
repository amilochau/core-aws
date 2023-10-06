﻿/*******************************************************************************
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

using System;
using System.ComponentModel;

using Amazon.Util;
using Amazon.Util.Internal;

namespace Amazon
{
    /// <summary>
    /// Configuration options that apply to the entire SDK.
    /// 
    /// These settings can be configured through app.config or web.config.
    /// Below is a full sample configuration that illustrates all the possible options.
    /// <code>
    /// &lt;configSections&gt;
    ///   &lt;section name="aws" type="Amazon.AWSSection, AWSSDK.Core"/&gt;
    /// &lt;/configSections&gt;
    /// &lt;aws region="us-west-2"&gt;
    ///   &lt;logging logTo="Log4Net, SystemDiagnostics" logResponses="Always" logMetrics="true" /&gt;
    ///   &lt;s3 useSignatureVersion4="true" /&gt;
    ///   &lt;proxy host="localhost" port="8888" username="1" password="1" /&gt;
    ///   
    ///   &lt;dynamoDB&gt;
    ///     &lt;dynamoDBContext tableNamePrefix="Prod-" metadataCachingMode="Default"&gt;
    /// 
    ///       &lt;tableAliases&gt;
    ///         &lt;alias fromTable="FakeTable" toTable="People" /&gt;
    ///         &lt;alias fromTable="Persons" toTable="People" /&gt;
    ///       &lt;/tableAliases&gt;
    /// 
    ///       &lt;mappings&gt;
    ///         &lt;map type="Sample.Tests.Author, SampleDLL" targetTable="People" /&gt;
    ///         &lt;map type="Sample.Tests.Editor, SampleDLL" targetTable="People"&gt;
    ///           &lt;property name="FullName" attribute="Name" /&gt;
    ///           &lt;property name="EmployeeId" attribute="Id" /&gt;
    ///           &lt;property name="ComplexData" converter="Sample.Tests.ComplexDataConverter, SampleDLL" /&gt;
    ///           &lt;property name="Version" version="true" /&gt;
    ///           &lt;property name="Password" ignore="true" /&gt;
    ///         &lt;/map&gt;
    ///       &lt;/mappings&gt;
    /// 
    ///     &lt;/dynamoDBContext&gt;
    ///   &lt;/dynamoDB&gt;
    /// &lt;/aws&gt;
    /// </code>
    /// </summary>
    public static partial class AWSConfigs
    {
        #region Private static members

        // Tests can override this DateTime source.
        internal static Func<DateTime> utcNowSource = () => DateTime.UtcNow;

        // New config section
        private static RootConfig _rootConfig = new RootConfig();
        #endregion

        #region Clock Skew

        /// <summary>
        /// Manual offset to apply to client clock.  This is a global setting that overrides 
        /// ClockOffset value calculated for all service endpoints.
        /// </summary>
        public static TimeSpan? ManualClockCorrection
        {
            get
            {
                return Runtime.CorrectClockSkew.GlobalClockCorrection;
            }
            set
            {
                Runtime.CorrectClockSkew.GlobalClockCorrection = value;
            }
        }

        /// <summary>
        /// Determines if the SDK should correct for client clock skew
        /// by determining the correct server time and reissuing the
        /// request with the correct time.
        /// Default value of this field is True.
        /// <seealso cref="ClockOffset"/> will be updated with the calculated
        /// offset even if this field is set to false, though requests
        /// will not be corrected or retried.
        /// Ignored if <seealso cref="ManualClockCorrection"/> is set.
        /// </summary>
        public static bool CorrectForClockSkew
        {
            get { return _rootConfig.CorrectForClockSkew; }
            set { _rootConfig.CorrectForClockSkew = value; }
        }

        /// <summary>
        /// The calculated clock skew correction, if there is one.
        /// This field will be set if a service call resulted in an exception
        /// and the SDK has determined that there is a difference between local
        /// and server times.
        /// 
        /// If <seealso cref="CorrectForClockSkew"/> is set to true, this
        /// value will be set to the correction, but it will not be used by the
        /// SDK and clock skew errors will not be retried.
        /// </summary>
        [Obsolete("This value is deprecated in favor of IClientConfig.ClockOffset")]
        public static TimeSpan ClockOffset
        {
            get;
            internal set;
        }
        #endregion

        #region AWS Config Sections

        /// <summary>
        /// Configuration for the Logging section of AWS configuration.
        /// Changes to some settings may not take effect until a new client is constructed.
        /// 
        /// Example section:
        /// <code>
        /// &lt;configSections&gt;
        ///   &lt;section name="aws" type="Amazon.AWSSection, AWSSDK.Core"/&gt;
        /// &lt;/configSections&gt;
        /// &lt;aws&gt;
        ///   &lt;logging logTo="Log4Net, SystemDiagnostics" logResponses="Always" logMetrics="true" /&gt;
        /// &lt;/aws&gt;
        /// </code>
        /// </summary>
        public static LoggingConfig LoggingConfig { get { return _rootConfig.Logging; } }

        /// <summary>
        /// Configuration for the Proxy section of AWS configuration.
        /// Changes to some settings may not take effect until a new client is constructed.
        /// 
        /// Example section:
        /// <code>
        /// &lt;configSections&gt;
        ///   &lt;section name="aws" type="Amazon.AWSSection, AWSSDK.Core"/&gt;
        /// &lt;/configSections&gt;
        /// &lt;aws&gt;
        ///   &lt;proxy host="localhost" port="8888" username="1" password="1" bypassList="addressexpr1;addressexpr2;..." bypassOnLocal="true" /&gt;
        /// &lt;/aws&gt;
        /// </code>
        /// </summary>
        public static ProxyConfig ProxyConfig { get { return _rootConfig.Proxy; } }

        /// <summary>
        /// When set to true, the service client will use the  x-amz-user-agent
        /// header instead of the User-Agent header to report version and
        /// environment information to the AWS service.
        ///
        /// Note: This is especially useful when using a platform like WebAssembly
        /// which doesn't allow to specify the User-Agent header.
        /// </summary>
        public static bool UseAlternateUserAgentHeader
        {
            get { return _rootConfig.UseAlternateUserAgentHeader; }
            set { _rootConfig.UseAlternateUserAgentHeader = value; }
        }

        #endregion

        #region Internal members

        internal const string LoggingDestinationProperty = "LogTo";

        internal static PropertyChangedEventHandler mPropertyChanged;
        /// <summary>
        /// Lock for SomeEvent delegate access.
        /// </summary>
        internal static readonly object propertyChangedLock = new object();

        internal static event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (propertyChangedLock)
                {
                    mPropertyChanged += value;
                }
            }
            remove
            {
                lock (propertyChangedLock)
                {
                    mPropertyChanged -= value;
                }
            }
        }

        internal static void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = mPropertyChanged;
            if (handler != null)
            {
                handler(null, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }

    /// <summary>
    /// Logging options.
    /// Can be combined to enable multiple loggers.
    /// </summary>
    [Flags]
    public enum LoggingOptions
    {
        /// <summary>
        /// No logging
        None = 0,
        /// </summary>

        /// <summary>
        /// Log using log4net
        /// </summary>
        Log4Net = 1,

        /// <summary>
        /// Log using System.Diagnostics
        /// </summary>
        SystemDiagnostics = 2,

        /// <summary>
        /// Log to the console
        /// </summary>
        Console = 16
    }

    /// <summary>
    /// Response logging option.
    /// </summary>
    public enum ResponseLoggingOption
    {
        /// <summary>
        /// Never log service response
        /// </summary>
        Never = 0,

        /// <summary>
        /// Only log service response when there's an error
        /// </summary>
        OnError = 1,

        /// <summary>
        /// Always log service response
        /// </summary>
        Always = 2
    }
}
