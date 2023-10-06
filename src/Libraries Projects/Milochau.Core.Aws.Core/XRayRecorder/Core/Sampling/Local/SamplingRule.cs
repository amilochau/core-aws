//-----------------------------------------------------------------------------
// <copyright file="SamplingRule.cs" company="Amazon.com">
//      Copyright 2016 Amazon.com, Inc. or its affiliates. All Rights Reserved.
//
//      Licensed under the Apache License, Version 2.0 (the "License").
//      You may not use this file except in compliance with the License.
//      A copy of the License is located at
//
//      http://aws.amazon.com/apache2.0
//
//      or in the "license" file accompanying this file. This file is distributed
//      on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
//      express or implied. See the License for the specific language governing
//      permissions and limitations under the License.
// </copyright>
//-----------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Amazon.XRay.Recorder.Core.Internal.Utils;

[module: SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes", Scope = "type", Target = "Amazon.XRay.Recorder.Core.Sampling.Local.SamplingRule", Justification = "Only used for sorting")]

namespace Amazon.XRay.Recorder.Core.Sampling.Local
{
    /// <summary>
    /// It represents the Rules used for sampling.
    /// </summary>    
    public class SamplingRule
    {
        private int _fixedTarget;

        /// <summary>
        /// Gets or sets the host of the rule
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the service name from V1 sampling rule json file.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the http method of the rule
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// Gets or sets the url path of the rule
        /// </summary>
        public string UrlPath { get; set; }

        /// <summary>
        /// Gets or sets the fixed target rate of the rule in the unit of traces/second
        /// </summary>
        public int FixedTarget
        {
            get
            {
                return _fixedTarget;
            }

            set
            {
                _fixedTarget = value;
                RateLimiter = new RateLimiter(value);
            }
        }

        /// <summary>
        /// Gets the rate limiter which had the limit set to fixed target rate
        /// </summary>
        public RateLimiter RateLimiter { get; private set; }

        /// <summary>
        /// Gets or sets the sampling rate
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Given the service name, http method and url path of a http request, check whether the rule matches the request 
        /// </summary>
        /// <param name="hostToMatch">host name of the request</param>
        /// <param name="urlPathToMatch">url path of the request</param>
        /// <param name="httpMethodToMatch">http method of the request</param>
        /// <returns>It returns true if the rule matches the request, otherwise it returns false.</returns>
        public bool IsMatch(string hostToMatch, string urlPathToMatch, string httpMethodToMatch)
        {
            try
            {
                return StringExtension.IsMatch(hostToMatch, Host) && StringExtension.IsMatch(urlPathToMatch, UrlPath) && StringExtension.IsMatch(httpMethodToMatch, HttpMethod);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
