using System;
using ExecutionContext = Amazon.Runtime.Internal.ExecutionContext;
using Amazon.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Pipeline;
using Milochau.Core.Aws.Core.Runtime.Pipeline.ErrorHandler;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Internal.Auth;
using System.Net.Http;
using Milochau.Core.Aws.Core.XRayRecorder.Handlers.AwsSdk.Internal;
using Microsoft.Extensions.Http;
using Polly.Extensions.Http;
using Polly;
using Milochau.Core.Aws.Core.Util;
using Milochau.Core.Aws.Core.References;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Runtime
{
    public abstract class AmazonServiceClient
    {
        private static readonly IAsyncPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(Math.Min(Math.Pow(2, retryAttempt), 20))); // Max 20 seconds
        private static readonly SocketsHttpHandler socketHandler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15), // Recreate every 15 minutes
        };
        private static PolicyHttpMessageHandler pollyHandler = new PolicyHttpMessageHandler(retryPolicy)
        {
            InnerHandler = socketHandler,
        };
        private static HttpClient httpClient { get; } = new HttpClient(pollyHandler);

        public IClientConfig Config { get; }

        protected AmazonServiceClient(ClientConfig config)
        {
            Config = config;
            Signer = new AWSSigner();
        }

        protected AWSSigner Signer { get; private set; }

        protected async Task<TResponse> InvokeAsync<TResponse>(
            AmazonWebServiceRequest request,
            InvokeOptions options,
            System.Threading.CancellationToken cancellationToken)
            where TResponse : AmazonWebServiceResponse, new()
        {
            var executionContext = new ExecutionContext(
                new RequestContext(Signer, Config, options.HttpRequestMessageMarshaller, options.ResponseUnmarshaller, request, cancellationToken),
                new ResponseContext()
            );

            // 0. Marshall request
            MarshallRequest(executionContext);

            // 1. Start request monitoring
            var xRayPipelineHandler = new XRayPipelineHandler();
            xRayPipelineHandler.ProcessBeginRequest(executionContext);

            TResponse? response = null;
            try
            {
                // 2. Sign request
                await SignRequestAsync(executionContext.RequestContext);
                ConfigureRequest(executionContext.RequestContext);

                // 3. Send request
                HttpResponseMessage responseMessage = await httpClient.SendAsync(executionContext.RequestContext.HttpRequestMessage, HttpCompletionOption.ResponseHeadersRead, executionContext.RequestContext.CancellationToken).ConfigureAwait(continueOnCapturedContext: false);

                // 4.1 Handle errors
                if (!responseMessage.IsSuccessStatusCode)
                {
                    // For all responses other than HTTP 2xx, return an exception.
                    var exception = new HttpErrorResponseException(responseMessage);
                    var exceptionHandler = new HttpErrorResponseExceptionHandler();
                    await exceptionHandler.HandleAsync(executionContext, exception).ConfigureAwait(false);
                    throw exception;
                }

                // 4.2 Manage response
                executionContext.ResponseContext.HttpResponse = responseMessage;
                await UnmarshallResponseAsync(executionContext).ConfigureAwait(false);
                response = (TResponse)executionContext.ResponseContext.Response;
            }
            catch (Exception e)
            {
                xRayPipelineHandler.PopulateException(e);
                throw;
            }
            finally
            {
                xRayPipelineHandler.ProcessEndRequest(executionContext);
                executionContext.RequestContext.HttpRequestMessage?.Dispose();
            }

            return response;
        }

        /// <summary>
        /// Configures a request as per the request context.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        private static void ConfigureRequest(IRequestContext requestContext)
        {
            // Configure the Expect 100-continue header
            if (requestContext != null && requestContext.OriginalRequest != null)
            {
                requestContext.HttpRequestMessage.Headers.ExpectContinue = false;
            }

            if (!requestContext.HttpRequestMessage.Headers.Contains(HeaderKeys.AmzSdkInvocationId))
            {
                requestContext.HttpRequestMessage.Headers.Add(HeaderKeys.AmzSdkInvocationId, requestContext.InvocationId.ToString());
            }
        }

        /// <summary>
        /// Signs the request.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        private static async Task SignRequestAsync(IRequestContext requestContext)
        {
            if (!requestContext.IsSigned)
            {
                if (EnvironmentVariables.UseToken)
                {
                    requestContext.HttpRequestMessage.Headers.Add(HeaderKeys.XAmzSecurityTokenHeader, EnvironmentVariables.Token);
                }

                await requestContext.Signer.SignAsync(requestContext);
                requestContext.IsSigned = true;
            }
        }

        /// <summary>
        /// Marshalls the request before calling invoking the next handler.
        /// </summary>
        /// <param name="executionContext">The execution context, it contains the request and response context.</param>
        private static void MarshallRequest(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;
            requestContext.HttpRequestMessage = requestContext.HttpRequestMessageMarshaller.CreateHttpRequestMessage(requestContext.OriginalRequest);

            if (EnvironmentVariables.TryGetEnvironmentVariable(EnvironmentVariables.Key_TraceId, out string? amznTraceId))
            {
                requestContext.HttpRequestMessage.Headers.Add(HeaderKeys.XAmznTraceIdHeader, AWSSDKUtils.EncodeTraceIdHeaderValue(amznTraceId));
            }
        }

        /// <summary>
        /// Unmarshalls the HTTP response.
        /// </summary>
        /// <param name="executionContext">The execution context, it contains the request and response context.</param>
        private static async System.Threading.Tasks.Task UnmarshallResponseAsync(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;
            var responseContext = executionContext.ResponseContext;
            var unmarshaller = requestContext.Unmarshaller;
            try
            {
                var responseStream = await responseContext.HttpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var context = unmarshaller.CreateContext(responseContext.HttpResponse,
                    false,
                    responseStream,
                    false);

                var response = unmarshaller.UnmarshallResponse(context);
                context.ValidateCRC32IfAvailable();

                responseContext.Response = response;
            }
            finally
            {
                responseContext.HttpResponse?.Dispose();
            }
        }
    }
}
