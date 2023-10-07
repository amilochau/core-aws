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

using Amazon.Lambda.RuntimeSupport.Helpers;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.Lambda.RuntimeSupport
{
    /// <summary>
    /// Client to call the AWS Lambda Runtime API.
    /// </summary>
    public class RuntimeApiClient : IRuntimeApiClient
    {
        private readonly IInternalRuntimeApiClient _internalClient;

        private readonly IConsoleLoggerWriter _consoleLoggerRedirector = new LogLevelLoggerWriter();

        internal LambdaEnvironment LambdaEnvironment { get; set; }

        internal RuntimeApiClient(HttpClient httpClient)
        {
            LambdaEnvironment = new LambdaEnvironment();
            var internalClient = new InternalRuntimeApiClient(httpClient);
            internalClient.BaseUrl = "http://" + LambdaEnvironment.RuntimeServerHostAndPort + internalClient.BaseUrl;
            _internalClient = internalClient;
        }

        /// <summary>
        /// Get the next function invocation from the Runtime API as an asynchronous operation.
        /// Completes when the next invocation is received.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to use to stop listening for the next invocation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task<InvocationRequest> GetNextInvocationAsync(CancellationToken cancellationToken = default)
        {
            SwaggerResponse<Stream> response = await _internalClient.NextAsync(cancellationToken);

            var headers = new RuntimeApiHeaders(response.Headers);
            _consoleLoggerRedirector.SetCurrentAwsRequestId(headers.AwsRequestId);

            var lambdaContext = new LambdaContext(headers, LambdaEnvironment, _consoleLoggerRedirector);
            return new InvocationRequest
            {
                InputStream = response.Result,
                LambdaContext = lambdaContext,
            };
        }

        /// <summary>
        /// Report an invocation error as an asynchronous operation.
        /// </summary>
        /// <param name="awsRequestId">The ID of the function request that caused the error.</param>
        /// <param name="exception">The exception to report.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public Task ReportInvocationErrorAsync(string awsRequestId, Exception exception, CancellationToken cancellationToken = default)
        {
            if (awsRequestId == null)
                throw new ArgumentNullException(nameof(awsRequestId));

            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            var exceptionInfo = ExceptionInfo.GetExceptionInfo(exception);

            var exceptionInfoJson = LambdaJsonExceptionWriter.WriteJson(exceptionInfo);
            var exceptionInfoXRayJson = LambdaXRayExceptionWriter.WriteJson(exceptionInfo);

            return _internalClient.ErrorWithXRayCauseAsync(awsRequestId, exceptionInfo.ErrorType, exceptionInfoJson, exceptionInfoXRayJson, cancellationToken);
        }


        /// <summary>
        /// Send a response to a function invocation to the Runtime API as an asynchronous operation.
        /// </summary>
        /// <param name="awsRequestId">The ID of the function request being responded to.</param>
        /// <param name="outputStream">The content of the response to the function invocation.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns></returns>
        public async Task SendResponseAsync(string awsRequestId, Stream outputStream, CancellationToken cancellationToken = default)
        {
            await _internalClient.ResponseAsync(awsRequestId, outputStream, cancellationToken);
        }
    }
}