//-----------------------------------------------------------------------------
// <copyright file="AWSXRayRecorderBuilder.cs" company="Amazon.com">
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

using Amazon.XRay.Recorder.Core.Strategies;

namespace Amazon.XRay.Recorder.Core
{
    /// <summary>
    /// This class provides utilities to build an instance of <see cref="AWSXRayRecorder"/> with different configurations.
    /// </summary>
    public class AWSXRayRecorderBuilder
    {
        /// <summary>
        /// Build a instance of <see cref="AWSXRayRecorder"/> with existing configuration added to the builder.
        /// </summary>
        /// <returns>A new instance of <see cref="AWSXRayRecorder"/>.</returns>
        public AWSXRayRecorder Build()
        {
            var recorder = new AWSXRayRecorder();
            recorder.ContextMissingStrategy = ContextMissingStrategy.LOG_ERROR;
            return recorder;
        }
    }
}
