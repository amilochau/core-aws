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
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

using Amazon.Runtime;
using ThirdParty.Json.LitJson;
using System.Globalization;
using Amazon.Runtime.Internal.Util;
using AWSSDK.Runtime.Internal.Util;
using Amazon.Runtime.Internal;

namespace Amazon.Util
{
    /// <summary>
    /// Provides access to EC2 instance metadata when running on an EC2 instance.
    /// If this class is used on a non-EC2 instance, the properties in this class
    /// will return null.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Amazon EC2 instances can access instance-specific metadata, as well as data supplied when launching the instances, using a specific URI.
    /// </para>
    /// <para>
    /// You can use this data to build more generic AMIs that can be modified by configuration files supplied at launch time. 
    /// For example, if you run web servers for various small businesses, they can all use the same AMI and retrieve their content from the 
    /// Amazon S3 bucket you specify at launch. To add a new customer at any time, simply create a bucket for the customer, add their content, 
    /// and launch your AMI.
    /// </para>
    /// <para>
    /// More information about EC2 Metadata <see href="http://docs.aws.amazon.com/AWSEC2/latest/UserGuide/AESDG-chapter-instancedata.html"/>
    /// </para>
    /// </remarks>
    public static class EC2InstanceMetadata
    {
        [Obsolete("EC2_METADATA_SVC is obsolete, refer to ServiceEndpoint instead to respect environment and profile overrides.")]
        public static readonly string EC2_METADATA_SVC = "http://169.254.169.254";

        [Obsolete("EC2_METADATA_ROOT is obsolete, refer to EC2MetadataRoot instead to respect environment and profile overrides.")]
        public static readonly string EC2_METADATA_ROOT = EC2_METADATA_SVC + LATEST + "/meta-data";

        [Obsolete("EC2_USERDATA_ROOT is obsolete, refer to EC2UserDataRoot instead to respect environment and profile overrides.")]
        public static readonly string EC2_USERDATA_ROOT = EC2_METADATA_SVC + LATEST + "/user-data";

        [Obsolete("EC2_DYNAMICDATA_ROOT is obsolete, refer to EC2DynamicDataRoot instead to respect environment and profile overrides.")]
        public static readonly string EC2_DYNAMICDATA_ROOT = EC2_METADATA_SVC + LATEST + "/dynamic";

        [Obsolete("EC2_APITOKEN_URL is obsolete, refer to EC2ApiTokenUrl instead to respect environment and profile overrides.")]
        public static readonly string EC2_APITOKEN_URL = EC2_METADATA_SVC + LATEST + "/api/token";

        public static readonly string
            LATEST = "/latest",
            AWS_EC2_METADATA_DISABLED = "AWS_EC2_METADATA_DISABLED";

        private static int
            DEFAULT_RETRIES = 3,
            MIN_PAUSE_MS = 250,
            DEFAULT_APITOKEN_TTL = 21600;

        private static Dictionary<string, string> _cache = new Dictionary<string, string>();

        private static bool useNullToken = false;

        private static ReaderWriterLockSlim metadataLock = new ReaderWriterLockSlim(); // Lock to control getting metadata across multiple threads.
        private static readonly TimeSpan metadataLockTimeout = TimeSpan.FromMilliseconds(5000);

        /// <summary>
        /// Base endpoint of the instance metadata service. Returns the endpoint configured first 
        /// via environment variable AWS_EC2_METADATA_SERVICE_ENDPOINT then the current profile's 
        /// ec2_metadata_service_endpoint value. If a specific endpoint is not configured, it selects a pre-determined
        /// endpoint based on environment variable AWS_EC2_METADATA_SERVICE_ENDPOINT_MODE then the 
        /// current profile's ec2_metadata_service_endpoint_mode setting.
        /// </summary>
        public static string ServiceEndpoint
        {
            get
            {
                if (!string.IsNullOrEmpty(FallbackInternalConfigurationFactory.EC2MetadataServiceEndpoint))
                {
                    return FallbackInternalConfigurationFactory.EC2MetadataServiceEndpoint;
                }
                else if (FallbackInternalConfigurationFactory.EC2MetadataServiceEndpointMode == EC2MetadataServiceEndpointMode.IPv6)
                {
                    return "http://[fd00:ec2::254]";
                }
                else // either explicit IPv4 or default behavior
                {
                    return "http://169.254.169.254";
                }
            }
        }

        /// <summary>
        /// Root URI to retrieve instance metadata
        /// </summary>
        public static string EC2MetadataRoot => ServiceEndpoint + LATEST + "/meta-data";

        /// <summary>
        /// Root URI to retrieve dynamic instance data
        /// </summary>
        public static string EC2DynamicDataRoot => ServiceEndpoint + LATEST + "/dynamic";

        /// <summary>
        /// URI to retrieve the IMDS API token
        /// </summary>
        public static string EC2ApiTokenUrl => ServiceEndpoint + LATEST + "/api/token";

        /// <summary>
        /// Returns whether requesting the EC2 Instance Metadata Service is 
        /// enabled via the AWS_EC2_METADATA_DISABLED environment variable.
        /// </summary>
        public static bool IsIMDSEnabled
        {
            get
            {
                const string True = "true";
                string value = string.Empty;
                try
                {
                    value = System.Environment.GetEnvironmentVariable(AWS_EC2_METADATA_DISABLED);
                } catch { };
                return !True.Equals(value, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Allows to configure the proxy used for HTTP requests. The default value is null.
        /// </summary>
        public static IWebProxy Proxy
        {
            get; set;
        }

        /// <summary>
        /// The region in which the instance is running, extracted from the identity
        /// document data.
        /// </summary>
        public static RegionEndpoint Region
        {
            get
            {
                var identityDocument = IdentityDocument;
                if (!string.IsNullOrEmpty(identityDocument))
                {
                    try
                    {
                        var jsonDocument = JsonMapper.ToObject(identityDocument.ToString());
                        var regionName = jsonDocument["region"];
                        if (regionName != null)
                            return RegionEndpoint.GetBySystemName(regionName.ToString());
                    }
                    catch (Exception e)
                    {
                        var logger = Logger.GetLogger(typeof(EC2InstanceMetadata));
                        logger.Error(e, "Error attempting to read region from instance metadata identity document");
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// JSON containing instance attributes, such as instance-id, private IP 
        /// address, etc
        /// </summary>
        public static string IdentityDocument
        {
            get
            {
                return GetData(EC2DynamicDataRoot + "/instance-identity/document");
            }
        }

        /// <summary>
        /// Return the metadata at the path
        /// </summary>
        /// <param name="path">Path at which to query the metadata; may be relative or absolute.</param>
        /// <returns>Data returned by the metadata service</returns>
        public static string GetData(string path)
        {
            return GetData(path, DEFAULT_RETRIES);
        }

        /// <summary>
        /// Return the metadata at the path
        /// </summary>
        /// <param name="path">Path at which to query the metadata; may be relative or absolute.</param>
        /// <param name="tries">Number of attempts to make</param>
        /// <returns>Data returned by the metadata service</returns>
        public static string GetData(string path, int tries)
        {
            var items = GetItems(path, tries, true);
            if (items != null && items.Count > 0)
                return items[0];
            return null;
        }

        /// <summary>
        /// Fetches the api token to use with metadata requests.
        /// </summary>
        /// <param name="tries">The number of tries to fetch the api token before giving up and throwing the web exception</param>
        /// <returns>The API token or null if an API token couldn't be obtained and doesn't need to be used</returns>
        private static string FetchApiToken(int tries)
        {
            for (int retry = 1; retry <= tries; retry++)
            {
                if (!IsIMDSEnabled || useNullToken)
                {
                    return null;
                }

                try
                {
                    var uriForToken = new Uri(EC2ApiTokenUrl);

                    var headers = new Dictionary<string, string>();
                    headers.Add(HeaderKeys.XAwsEc2MetadataTokenTtlSeconds, DEFAULT_APITOKEN_TTL.ToString(CultureInfo.InvariantCulture));
                    var content = AWSSDKUtils.ExecuteHttpRequest(uriForToken, "PUT", null, TimeSpan.FromSeconds(5), Proxy, headers);
                    return content.Trim();
                }
                catch (Exception e)
                {
                    HttpStatusCode? httpStatusCode = ExceptionUtils.DetermineHttpStatusCode(e);

                    if (httpStatusCode == HttpStatusCode.NotFound 
                        || httpStatusCode == HttpStatusCode.MethodNotAllowed
                        || httpStatusCode == HttpStatusCode.Forbidden)
                    {
                        useNullToken = true;
                        return null;
                    }

                    if (retry >= tries)
                    {
                        if (httpStatusCode == HttpStatusCode.BadRequest)
                        {
                            Logger.GetLogger(typeof(EC2InstanceMetadata)).Error(e, "Unable to contact EC2 Metadata service to obtain a metadata token.");
                            throw;
                        }

                        Logger.GetLogger(typeof(EC2InstanceMetadata)).Error(e, "Unable to contact EC2 Metadata service to obtain a metadata token. Attempting to access IMDS without a token.");

                        //If there isn't a status code, it was a failure to contact the server which would be
                        //a request failure, a network issue, or a timeout. Cache this response and fallback
                        //to IMDS flow without a token. If the non token IMDS flow returns unauthorized, the 
                        //useNullToken flag will be cleared and the IMDS flow will attempt to obtain another 
                        //token.
                        if (httpStatusCode == null)
                        {
                            useNullToken = true;
                        }

                        //Return null to fallback to the IMDS flow without using a token.                    
                        return null;
                    }

                    PauseExponentially(retry - 1);
                }                
            }

            return null;
        }

        public static void ClearTokenFlag()
        {    
            useNullToken = false;
        }

        private static List<string> GetItems(string relativeOrAbsolutePath, int tries, bool slurp)
        {
            return GetItems(relativeOrAbsolutePath, tries, slurp, null);
        }

        private static List<string> GetItems(string relativeOrAbsolutePath, int tries, bool slurp, string token)
        {
            var items = new List<string>();
            //For all meta-data queries we need to fetch an api token to use. In the event a 
            //token cannot be obtained we will fallback to not using a token.
            Dictionary<string, string> headers = null;
            if(token == null)
            {
                token = FetchApiToken(DEFAULT_RETRIES);    
            }

            if (!string.IsNullOrEmpty(token))
            {
                headers = new Dictionary<string, string>();
                headers.Add(HeaderKeys.XAwsEc2MetadataToken, token);
            }

            try
            {
                if (!IsIMDSEnabled)
                {
                    throw new IMDSDisabledException();
                }

                // if we are given a relative path, we assume the data we need exists under the
                // main metadata root
                var uri = relativeOrAbsolutePath.StartsWith(ServiceEndpoint, StringComparison.Ordinal)
                            ? new Uri(relativeOrAbsolutePath)
                            : new Uri(EC2MetadataRoot + relativeOrAbsolutePath);
                
                var content = AWSSDKUtils.ExecuteHttpRequest(uri, "GET", null, TimeSpan.FromSeconds(5), Proxy, headers);
                using (var stream = new StringReader(content))
                {
                    if (slurp)
                        items.Add(stream.ReadToEnd());
                    else
                    {
                        string line;
                        do
                        {
                            line = stream.ReadLine();
                            if (line != null)
                                items.Add(line.Trim());
                        }
                        while (line != null);
                    }
                }
            }            
            catch (IMDSDisabledException)
            {
                // Keep this behavior identical to when HttpStatusCode.NotFound is returned.
                return null;
            }
            catch (Exception e)
            {
                HttpStatusCode? httpStatusCode = ExceptionUtils.DetermineHttpStatusCode(e);

                if (httpStatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else if (httpStatusCode == HttpStatusCode.Unauthorized)
                {
                    ClearTokenFlag();
                    Logger.GetLogger(typeof(EC2InstanceMetadata)).Error(e, "EC2 Metadata service returned unauthorized for token based secure data flow.");
                    throw;
                }

                if (tries <= 1)
                {
                    Logger.GetLogger(typeof(EC2InstanceMetadata)).Error(e, "Unable to contact EC2 Metadata service.");
                    return null;
                }

                PauseExponentially(DEFAULT_RETRIES - tries);
                return GetItems(relativeOrAbsolutePath, tries - 1, slurp, token);
            }

            return items;
        }
                
        /// <summary>
        /// Exponentially sleeps based on the current retry value. A lower 
        /// value will sleep shorter than a larger value
        /// </summary>
        /// <param name="retry">Base 0 retry index</param>
        private static void PauseExponentially(int retry)
        {
            var pause = (int)(Math.Pow(2, retry) * MIN_PAUSE_MS);
            Thread.Sleep(pause < MIN_PAUSE_MS ? MIN_PAUSE_MS : pause);
        }

        private class IMDSDisabledException : InvalidOperationException { };
    }
}
