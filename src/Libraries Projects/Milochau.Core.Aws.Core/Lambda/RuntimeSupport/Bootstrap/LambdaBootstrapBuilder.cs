/*
 * Copyright 2019 Amazon.com, Inc. or its affiliates. All Rights Reserved.
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
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

namespace Amazon.Lambda.RuntimeSupport
{
    /// <summary>
    /// Class to communicate with the Lambda Runtime API, handle initialization,
    /// and run the invoke loop for an AWS Lambda function
    /// </summary>
    public class LambdaBootstrapBuilder
    {
        private HandlerWrapper _handlerWrapper;
        private HttpClient _httpClient;
        private LambdaBootstrapInitializer _lambdaBootstrapInitializer;

        private LambdaBootstrapBuilder(HandlerWrapper handlerWrapper)
        {
            this._handlerWrapper = handlerWrapper;
        }

        /// <summary>
        /// Create a builder for creating the LambdaBootstrap.
        /// </summary>
        /// <param name="handler">The handler that will be called for each Lambda invocation</param>
        /// <returns></returns>
        public static LambdaBootstrapBuilder Create(Func<Stream, ILambdaContext, Task> handler)
        {
            return new LambdaBootstrapBuilder(HandlerWrapper.GetHandlerWrapper(handler));
        }

        /// <summary>
        /// Create a builder for creating the LambdaBootstrap.
        /// </summary>
        /// <param name="handler">The handler that will be called for each Lambda invocation</param>
        /// <returns></returns>
        public static LambdaBootstrapBuilder Create(Func<Stream, ILambdaContext, Task<Stream>> handler)
        {
            return new LambdaBootstrapBuilder(HandlerWrapper.GetHandlerWrapper(handler));
        }

        public LambdaBootstrap Build()
        {
            if(_httpClient == null)
            {
                return new LambdaBootstrap(_handlerWrapper, _lambdaBootstrapInitializer);
            }

            return new LambdaBootstrap(_httpClient, _handlerWrapper, _lambdaBootstrapInitializer);
        }
    }
}
