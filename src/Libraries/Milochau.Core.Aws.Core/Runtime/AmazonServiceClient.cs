using System;
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
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
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
            InvokeOptions<TRequest, TResponse> options,
            CancellationToken cancellationToken)
            where TRequest : AmazonWebServiceRequest
            where TResponse : AmazonWebServiceResponse, new()
        {
            // 0. Create and marshall request
            var requestContext = new RequestContext(config, request, options.MonitoringOriginalRequestName)
            {
                HttpRequestMessage = MarshallRequest(options.RequestMarshaller, request),
                ImmutableCredentials = await credentials.GetCredentialsAsync(),
            };
            var responseContext = new ResponseContext();

            // 1. Start request monitoring
            var facadeSegment = new FacadeSegment();
            var subsegment = XRayPipelineHandler.ProcessBeginRequest(facadeSegment, requestContext);

            TResponse? response = null;
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
                    throw await HttpErrorResponseExceptionHandler.HandleAsync(responseContext.HttpResponse, options.ExceptionUnmarshaller).ConfigureAwait(false); // HttpResponseMessage.Content.ReadAsStreamAsync
                }

                // 4.2 Manage response
                response = await UnmarshallResponseAsync(responseContext.HttpResponse, options.ResponseUnmarshaller).ConfigureAwait(false);
                responseContext.Response = response;
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
        private static HttpRequestMessage MarshallRequest<TRequest>(Func<TRequest, HttpRequestMessage> marshaller, TRequest originalRequest)
            where TRequest : AmazonWebServiceRequest
        {
            var httpRequestMessage = marshaller.Invoke(originalRequest);

            if (EnvironmentVariables.TryGetEnvironmentVariable(EnvironmentVariables.Key_TraceId, out string? amznTraceId))
            {
                httpRequestMessage.Headers.Add(HeaderKeys.XAmznTraceIdHeader, AWSSDKUtils.EncodeTraceIdHeaderValue(amznTraceId));
            }

            return httpRequestMessage;
        }

        /// <summary>
        /// Unmarshalls the HTTP response.
        /// </summary>
        private static async Task<TResponse> UnmarshallResponseAsync<TResponse>(HttpResponseMessage httpResponseMessage, Func<JsonUnmarshallerContext, TResponse> unmarshaller)
            where TResponse: AmazonWebServiceResponse
        {
            var responseStream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var context = new JsonUnmarshallerContext(responseStream,
                maintainResponseBody: false,
                httpResponseMessage,
                isException: false);

            var response = unmarshaller.Invoke(context);
            response.HttpStatusCode = context.ResponseData.StatusCode;
            context.ValidateCRC32IfAvailable();

            return response;
        }
    }
}
