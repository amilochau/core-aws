//-----------------------------------------------------------------------------
// <copyright file="AppSettings.netcore.cs" company="Amazon.com">
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
namespace Amazon.XRay.Recorder.Core.Internal.Utils
{
    /// <summary>
    /// This is a class for storing configuration of IConfiguration instance.
    /// </summary>
    public class XRayOptions
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public XRayOptions()
        {
        }

        /// <summary>
        /// Sampling rule file path.
        /// </summary>
        public string SamplingRuleManifest { get; set; }

        /// <summary>
        /// Tracing disabled value, either true or false.
        /// </summary>
        public bool IsXRayTracingDisabled { get; set; }
    }
}
