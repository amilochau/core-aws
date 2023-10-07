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
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.Runtime.Internal.Auth
{
    public enum ClientProtocol { QueryStringProtocol, RestProtocol, Unknown }

    public abstract class AbstractAWSSigner
    {
        /// <summary>
        /// Signals to the <see cref="Signer"/> Pipeline Handler
        /// if a Signer requires valid <see cref="ImmutableCredentials"/> in order
        /// to correctly <see cref="Sign(IRequest,IClientConfig,ImmutableCredentials)"/>.
        /// </summary>
        public virtual bool RequiresCredentials { get; } = true;

        public abstract void Sign(IRequest request, IClientConfig clientConfig, string awsAccessKeyId, string awsSecretAccessKey);

        public virtual void Sign(IRequest request, IClientConfig clientConfig, ImmutableCredentials credentials)
        {
            Sign(request, clientConfig, credentials?.AccessKey, credentials?.SecretKey);
        }

        public virtual System.Threading.Tasks.Task SignAsync(
            IRequest request, 
            IClientConfig clientConfig,
            ImmutableCredentials credentials,
            CancellationToken token = default)
        {
            Sign(request, clientConfig, credentials);
            return Task.CompletedTask;
        }

        public abstract ClientProtocol Protocol { get; }
    }
}
