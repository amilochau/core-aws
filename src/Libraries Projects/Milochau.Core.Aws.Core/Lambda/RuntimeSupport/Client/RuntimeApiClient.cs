using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Context;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.ExceptionHandling;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Helpers;
using Milochau.Core.Aws.Core.References;
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
    public class RuntimeApiClient : IRuntimeApiClient
    {
        private readonly IInternalRuntimeApiClient _internalClient;

        private readonly IConsoleLoggerWriter _consoleLoggerRedirector = new LogLevelLoggerWriter();

        internal RuntimeApiClient(HttpClient httpClient)
        {
            _internalClient = new InternalRuntimeApiClient(httpClient);
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

            var lambdaContext = new LambdaContext(headers, _consoleLoggerRedirector);
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
        public async Task SendResponseAsync(string awsRequestId, Stream outputStream, CancellationToken cancellationToken = default)
        {
            await _internalClient.ResponseAsync(awsRequestId, outputStream, cancellationToken);
        }
    }
}