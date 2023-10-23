using System;
using Amazon.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Pipeline;
using Milochau.Core.Aws.Core.Runtime.Pipeline.ErrorHandler;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Internal.Auth;
using System.Net.Http;
using Milochau.Core.Aws.Core.XRayRecorder.Handlers.AwsSdk.Internal;
using Milochau.Core.Aws.Core.Util;
using Milochau.Core.Aws.Core.References;
using System.Threading.Tasks;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using System.Threading;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;

namespace Milochau.Core.Aws.Core.Runtime
{
    public abstract class AmazonServiceClient
    {
        private readonly IClientConfig config;
        private readonly AWSSigner signer = new AWSSigner();

        protected AmazonServiceClient(ClientConfig config)
        {
            this.config = config;
        }

        protected async Task<TResponse> InvokeAsync<TResponse>(
            AmazonWebServiceRequest request,
            InvokeOptions options,
            CancellationToken cancellationToken)
            where TResponse : AmazonWebServiceResponse, new()
        {
            var requestContext = new RequestContext(config, options.HttpRequestMessageMarshaller, options.ResponseUnmarshaller, request);
            IResponseContext? responseContext = null;

            // 0. Marshall request
            MarshallRequest(requestContext);

            // 1. Start request monitoring
            var facadeSegment = FacadeSegment.Create();
            var subsegment = XRayPipelineHandler.ProcessBeginRequest(facadeSegment, requestContext);

            TResponse? response = null;
            try
            {
                // 2. Sign request
                await SignRequestAsync(requestContext);
                ConfigureRequest(requestContext);

                // 3. Send request
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(100)); // HttpClients.HttpClient has an infinite timeout
                using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);
                var httpResponseMessage = await HttpClients.HttpClient.SendAsync(requestContext.HttpRequestMessage, HttpCompletionOption.ResponseHeadersRead, combinedCts.Token).ConfigureAwait(continueOnCapturedContext: false);
                responseContext = new ResponseContext { HttpResponse = httpResponseMessage };
                var executionContext = new Amazon.Runtime.Internal.ExecutionContext(requestContext, responseContext);

                // 4.1 Handle errors
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    throw await HttpErrorResponseExceptionHandler.HandleAsync(executionContext).ConfigureAwait(false); // HttpResponseMessage.Content.ReadAsStreamAsync
                }

                // 4.2 Manage response
                await UnmarshallResponseAsync(executionContext).ConfigureAwait(false);
                response = (TResponse)responseContext.Response;
            }
            catch (Exception e)
            {
                XRayPipelineHandler.PopulateException(subsegment, e);
                throw;
            }
            finally
            {
                XRayPipelineHandler.ProcessEndRequest(subsegment, requestContext, responseContext);
                requestContext.HttpRequestMessage?.Dispose();
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
            requestContext.HttpRequestMessage.Headers.ExpectContinue = false;

            var invocationId = Guid.NewGuid().ToString();
            requestContext.HttpRequestMessage.Headers.Add(HeaderKeys.AmzSdkInvocationId, invocationId);
        }

        /// <summary>
        /// Signs the request.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        private async Task SignRequestAsync(IRequestContext requestContext)
        {
            if (EnvironmentVariables.UseToken)
            {
                requestContext.HttpRequestMessage.Headers.Add(HeaderKeys.XAmzSecurityTokenHeader, EnvironmentVariables.Token);
            }

            await signer.SignAsync(requestContext);
        }

        /// <summary>
        /// Marshalls the request before calling invoking the next handler.
        /// </summary>
        private static void MarshallRequest(IRequestContext requestContext)
        {
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
        private static async Task UnmarshallResponseAsync(IExecutionContext executionContext)
        {
            var responseContext = executionContext.ResponseContext;

            var responseStream = await responseContext.HttpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var context = new JsonUnmarshallerContext(responseStream,
                maintainResponseBody: false,
                responseContext.HttpResponse,
                isException: false);

            var response = executionContext.RequestContext.Unmarshaller.UnmarshallResponse(context);
            context.ValidateCRC32IfAvailable();

            responseContext.Response = response;
        }
    }
}
