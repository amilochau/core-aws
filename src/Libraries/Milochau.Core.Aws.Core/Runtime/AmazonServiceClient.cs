﻿using System;
using Amazon.Runtime.Internal;
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
using Milochau.Core.Aws.Core.Runtime.Credentials;

namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>Amazon service client</summary>
    /// <remarks>Constructor</remarks>
    public abstract class AmazonServiceClient(IAWSCredentials credentials, ClientConfig config)
    {
        private readonly ClientConfig config = config;

        /// <summary>Invoke</summary>
        protected async Task<TResponse> InvokeAsync<TRequest, TResponse>(
            TRequest request,
            IInvokeOptions<TRequest, TResponse> invokeOptions,
            CancellationToken cancellationToken)
            where TRequest : AmazonWebServiceRequest
            where TResponse : AmazonWebServiceResponse, new()
        {
            // 0. Create and marshall request
            var requestContext = new RequestContext(config, request, invokeOptions.MonitoringOriginalRequestName)
            {
                HttpRequestMessage = MarshallRequest(invokeOptions, request),
                ImmutableCredentials = await credentials.GetCredentialsAsync(),
            };
            var responseContext = new ResponseContext();

            // 1. Start request monitoring
            var xrayPipelineHandler = new XRayPipelineHandler();
            xrayPipelineHandler.ProcessBeginRequest(requestContext.ClientConfig.MonitoringServiceName);
            xrayPipelineHandler.AddHttpRequestHeader(requestContext.HttpRequestMessage);

            TResponse response;
            try
            {
                // 2. Sign request
                await SignRequestAsync(requestContext);
                ConfigureRequest(requestContext.HttpRequestMessage);

                // 3. Send request
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(100)); // HttpClients.HttpClient has an infinite timeout
                using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);
                responseContext.HttpResponse = await HttpClients.HttpClient.SendAsync(requestContext.HttpRequestMessage, HttpCompletionOption.ResponseHeadersRead, combinedCts.Token).ConfigureAwait(continueOnCapturedContext: false);

                // 4.1 Handle errors
                if (!responseContext.HttpResponse.IsSuccessStatusCode)
                {
                    throw await HttpErrorResponseExceptionHandler.HandleAsync(responseContext.HttpResponse, invokeOptions).ConfigureAwait(false); // HttpResponseMessage.Content.ReadAsStreamAsync
                }

                // 4.2 Manage response
                response = await UnmarshallResponseAsync(responseContext.HttpResponse, invokeOptions).ConfigureAwait(false);
                responseContext.Response = response;
            }
            catch (Exception e)
            {
                xrayPipelineHandler.PopulateException(e);
                throw;
            }
            finally
            {
                xrayPipelineHandler.ProcessEndRequest(requestContext, responseContext);
                requestContext.HttpRequestMessage?.Dispose();
            }

            return response;
        }

        /// <summary>
        /// Configures a request as per the request context.
        /// </summary>
        private static void ConfigureRequest(HttpRequestMessage httpRequestMessage)
        {
            // Configure the Expect 100-continue header
            httpRequestMessage.Headers.ExpectContinue = false;

            var invocationId = Guid.NewGuid().ToString();
            httpRequestMessage.Headers.Add(HeaderKeys.AmzSdkInvocationId, invocationId);
        }

        /// <summary>
        /// Signs the request.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        private static async Task SignRequestAsync(RequestContext requestContext)
        {
            var immutableCredentials = requestContext.ImmutableCredentials;

            if (immutableCredentials.UseToken)
            {
                requestContext.HttpRequestMessage.Headers.Add(HeaderKeys.XAmzSecurityTokenHeader, immutableCredentials.Token);
            }

            await AWSSigner.SignAsync(requestContext, immutableCredentials);
        }

        /// <summary>
        /// Marshalls the request before calling invoking the next handler.
        /// </summary>
        private static HttpRequestMessage MarshallRequest<TRequest, TResponse>(IInvokeOptions<TRequest, TResponse> invokeOptions, TRequest originalRequest)
            where TRequest : AmazonWebServiceRequest
            where TResponse : AmazonWebServiceResponse, new()
        {
            var httpRequestMessage = invokeOptions.MarshallRequest(originalRequest);

            var amznTraceId = EnvironmentVariables.TraceId;
            if (amznTraceId != null)
            {
                httpRequestMessage.Headers.Add(HeaderKeys.XAmznTraceIdHeader, AWSSDKUtils.EncodeTraceIdHeaderValue(amznTraceId));
            }

            return httpRequestMessage;
        }

        /// <summary>
        /// Unmarshalls the HTTP response.
        /// </summary>
        private static async Task<TResponse> UnmarshallResponseAsync<TRequest, TResponse>(HttpResponseMessage httpResponseMessage, IInvokeOptions<TRequest, TResponse> invokeOptions)
            where TRequest : AmazonWebServiceRequest
            where TResponse : AmazonWebServiceResponse, new()
        {
            var responseStream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var context = new JsonUnmarshallerContext(responseStream,
                maintainResponseBody: false,
                httpResponseMessage,
                isException: false);

            var response = invokeOptions.UnmarshallResponse(context);
            response.HttpStatusCode = context.ResponseData.StatusCode;
            context.ValidateCRC32IfAvailable();

            return response;
        }
    }
}
