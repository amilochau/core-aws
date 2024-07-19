using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Context;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.ExceptionHandling;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client
{
    /// <summary>
    /// Client to call the AWS Lambda Runtime API.
    /// </summary>
    internal class RuntimeApiClient
    {
        private readonly InternalRuntimeApiClient internalClient;

        internal RuntimeApiClient()
        {
            internalClient = new InternalRuntimeApiClient();
        }

        /// <summary>
        /// Get the next function invocation from the Runtime API as an asynchronous operation.
        /// Completes when the next invocation is received.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to use to stop listening for the next invocation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task<InvocationRequest> GetNextInvocationAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await internalClient.NextAsync(cancellationToken);

            var headers = new RuntimeApiHeaders(response.Headers);
            var lambdaContext = new LambdaContext(headers);
            return new InvocationRequest(response)
            {
                InputStream = await response.Content.ReadAsStreamAsync(),
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
        public Task ReportInvocationErrorAsync(string awsRequestId, Exception exception, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(awsRequestId);

            var exceptionInfo = ExceptionInfo.GetExceptionInfo(exception);

            var exceptionInfoJson = LambdaJsonExceptionWriter.WriteJson(exceptionInfo);
            var exceptionInfoXRayJson = LambdaXRayExceptionWriter.WriteJson(exceptionInfo);

            return internalClient.ErrorWithXRayCauseAsync(awsRequestId, exceptionInfo.ErrorType, exceptionInfoJson, exceptionInfoXRayJson, cancellationToken);
        }

        /// <summary>
        /// Send a response to a function invocation to the Runtime API as an asynchronous operation.
        /// </summary>
        /// <param name="awsRequestId">The ID of the function request being responded to.</param>
        /// <param name="outputStream">The content of the response to the function invocation.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        public async Task SendResponseAsync(string awsRequestId, Stream outputStream, CancellationToken cancellationToken)
        {
            await internalClient.ResponseAsync(awsRequestId, outputStream, cancellationToken);
        }
    }
}