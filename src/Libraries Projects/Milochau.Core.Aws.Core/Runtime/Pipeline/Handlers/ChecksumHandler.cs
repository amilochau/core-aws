﻿/*
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

namespace Amazon.Runtime.Internal
{
    /// <summary>
    /// This handler manages the calculation of the checksum of the payload request when the checksum of a request is supported.
    /// It also sets the checksum header of the request.
    /// </summary>
    public class ChecksumHandler : PipelineHandler
    {
        /// <summary>
        /// Calls pre invoke logic before calling the next handler 
        /// in the pipeline.
        /// </summary>
        /// <typeparam name="T">The response type for the current request.</typeparam>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            PreInvoke(executionContext);
            return base.InvokeAsync<T>(executionContext);
        }

        /// <summary>
        /// Calculates the checksum of the payload of a request, and sets the checksum request header only once.
        /// </summary>
        /// <param name="executionContext">The execution context which contains both the
        /// request and response context.</param>
        protected virtual void PreInvoke(IExecutionContext executionContext)
        {
        }
    }
}
