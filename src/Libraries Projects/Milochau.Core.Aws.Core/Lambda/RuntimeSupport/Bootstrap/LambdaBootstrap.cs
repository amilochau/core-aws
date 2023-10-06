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
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.RuntimeSupport.Helpers;

namespace Amazon.Lambda.RuntimeSupport
{
    public delegate Task<InvocationResponse> LambdaBootstrapHandler(InvocationRequest invocation);
    public delegate Task<bool> LambdaBootstrapInitializer();

    /// <summary>
    /// Class to communicate with the Lambda Runtime API, handle initialization,
    /// and run the invoke loop for an AWS Lambda function
    /// </summary>
    public class LambdaBootstrap : IDisposable
    {
        /// <summary>
        /// The Lambda container freezes the process at a point where an HTTP request is in progress.
        /// We need to make sure we don't timeout waiting for the next invocation.
        /// </summary>
        private static readonly TimeSpan RuntimeApiHttpTimeout = TimeSpan.FromHours(12);

        private LambdaBootstrapHandler _handler;

        private HttpClient _httpClient;
        internal IRuntimeApiClient Client { get; set; }

        /// <summary>
        /// Create a LambdaBootstrap that will call the given initializer and handler.
        /// </summary>
        /// <param name="handlerWrapper">The HandlerWrapper to call for each invocation of the Lambda function.</param>
        /// <param name="initializer">Delegate called to initialize the Lambda function.  If not provided the initialization step is skipped.</param>
        /// <returns></returns>
        public LambdaBootstrap(HandlerWrapper handlerWrapper)
            : this(handlerWrapper.Handler)
        { }

        /// <summary>
        /// Create a LambdaBootstrap that will call the given initializer and handler.
        /// </summary>
        /// <param name="handler">Delegate called for each invocation of the Lambda function.</param>
        /// <returns></returns>
        public LambdaBootstrap(LambdaBootstrapHandler handler)
        {
            _httpClient = ConstructHttpClient();
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _httpClient.Timeout = RuntimeApiHttpTimeout;
            Client = new RuntimeApiClient(_httpClient);
        }

        /// <summary>
        /// Run the initialization Func if provided.
        /// Then run the invoke loop, calling the handler for each invocation.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A Task that represents the operation.</returns>
        public async Task RunAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await InvokeOnceAsync(cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // Loop cancelled
                }
            }
        }

        internal async Task InvokeOnceAsync(CancellationToken cancellationToken = default)
        {
            using (var invocation = await Client.GetNextInvocationAsync(cancellationToken))
            {
                InvocationResponse response = null;
                bool invokeSucceeded = false;

                try
                {
                    response = await _handler(invocation);
                    invokeSucceeded = true;
                }
                catch (Exception exception)
                {
                    WriteUnhandledExceptionToLog(exception);
                    await Client.ReportInvocationErrorAsync(invocation.LambdaContext.AwsRequestId, exception);
                }

                if (invokeSucceeded)
                {
                    try
                    {
                        await Client.SendResponseAsync(invocation.LambdaContext.AwsRequestId, response?.OutputStream);
                    }
                    finally
                    {
                        if (response != null && response.DisposeOutputStream)
                        {
                            response.OutputStream?.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Utility method for creating an HttpClient used by LambdaBootstrap to interact with the Lambda Runtime API.
        /// </summary>
        /// <returns></returns>
        public static HttpClient ConstructHttpClient()
        {
            var dotnetRuntimeVersion = new DirectoryInfo(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()).Name;
            if (dotnetRuntimeVersion == "/")
            {
                dotnetRuntimeVersion = "unknown";
            }
            var amazonLambdaRuntimeSupport = typeof(LambdaBootstrap).Assembly.GetName().Version;

            // Create the SocketsHttpHandler directly to avoid spending cold start time creating the wrapper HttpClientHandler
            var handler = new SocketsHttpHandler
            {
                // Fix for https://github.com/aws/aws-lambda-dotnet/issues/1231. HttpClient by default supports only ASCII characters in headers. Changing it to allow UTF8 characters.
                RequestHeaderEncodingSelector = delegate { return System.Text.Encoding.UTF8; }
            };

            // If we are running in an AOT environment, mark it as such.
            var userAgentString = $"aws-lambda-dotnet/{dotnetRuntimeVersion}-{amazonLambdaRuntimeSupport}-aot";

            var client = new HttpClient(handler);

            client.DefaultRequestHeaders.Add("User-Agent", userAgentString);
            return client;
        }

        private void WriteUnhandledExceptionToLog(Exception exception)
        {
            // Console.Error.WriteLine are redirected to the IConsoleLoggerWriter which
            // will take care of writing to the function's log stream.
            Console.Error.WriteLine(exception);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _httpClient?.Dispose();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
