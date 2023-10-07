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

namespace Amazon.Runtime.Internal
{
    /// <summary>
    /// This handler retrieved the AWS credentials to be used for the current call.
    /// </summary>
    public class CredentialsRetriever : PipelineHandler
    {
        /// <summary>
        /// The constructor for CredentialsRetriever.
        /// </summary>
        /// <param name="credentials">An AWSCredentials instance.</param>
        public CredentialsRetriever(AWSCredentials credentials)
        {
            this.Credentials = credentials;
        }

        protected AWSCredentials Credentials
        {
            get;
            private set;
        }

        /// <summary>
        /// Calls pre invoke logic before calling the next handler 
        /// in the pipeline.
        /// </summary>
        /// <typeparam name="T">The response type for the current request.</typeparam>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            executionContext.RequestContext.ImmutableCredentials = await Credentials.GetCredentialsAsync().ConfigureAwait(false);

            return await base.InvokeAsync<T>(executionContext).ConfigureAwait(false);
        }
    }
}
