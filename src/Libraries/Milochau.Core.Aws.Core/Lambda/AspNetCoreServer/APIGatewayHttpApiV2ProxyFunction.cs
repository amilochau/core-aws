using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
using Milochau.Core.Aws.Core.Lambda.AspNetCoreServer.Internal;
using Milochau.Core.Aws.Core.Lambda.Core;
using Milochau.Core.Aws.Core.Lambda.Events;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Linq;

namespace Milochau.Core.Aws.Core.Lambda.AspNetCoreServer
{
    /// <summary>Base class for ASP.NET Core Lambda functions that are getting request from API Gateway HTTP API V2 payload format</summary>
    public class APIGatewayHttpApiV2ProxyFunction(IServiceProvider serviceProvider, LambdaServer lambdaServer)
    {
        private readonly ILogger logger = serviceProvider.GetRequiredService<ILogger<APIGatewayHttpApiV2ProxyFunction>>();

        public string[] ResponseContentEncodingForContentTypeBase64 { get; set; } = ["application/octet-stream", "image/png", "image/gif", "image/jpeg", "image/jpg", "image/x-icon", "application/zip", "application/pdf", "application/x-protobuf", "application/wasm"];
        public string[] ResponseContentEncodingForContentEncodingBase64 { get; set; } = ["gzip", "deflate", "br"];

        /// <summary>
        /// If true, information about unhandled exceptions thrown during request processing
        /// will be included in the HTTP response.
        /// </summary>
        public bool IncludeUnhandledExceptionDetailInResponse { get; set; }

        /// <summary>Creates a context object using the <see cref="LambdaServer"/> field in the class</summary>
        protected object CreateContext(IFeatureCollection features)
        {
            return lambdaServer!.Application!.CreateContext(features);
        }

        /// <summary>Gets the response content encoding for a content type</summary>
        public ResponseContentEncoding GetAPIGatewayHttpApiV2ProxyResponseContentEncodingForContentType(string? contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return ResponseContentEncoding.Default;
            }

            // ASP.NET Core will typically return content type with encoding like this "application/json; charset=utf-8"
            // To find the content type in the dictionary we need to strip the encoding off.
            var contentTypeWithoutEncoding = contentType.Split(';')[0].Trim();
            return ResponseContentEncodingForContentTypeBase64.Contains(contentTypeWithoutEncoding)
                ? ResponseContentEncoding.Base64
                : ResponseContentEncoding.Default;
        }

        /// <summary>Gets the response content encoding for a content encoding</summary>
        public ResponseContentEncoding GetAPIGatewayHttpApiV2ProxyResponseContentEncodingForContentEncoding(string? contentEncoding)
        {
            return ResponseContentEncodingForContentEncodingBase64.Contains(contentEncoding)
                ? ResponseContentEncoding.Base64
                : ResponseContentEncoding.Default;
        }

        /// <summary>Formats an Exception into a string, including all inner exceptions</summary>
        protected static string ErrorReport(Exception e)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{e.GetType().Name}:\n{e}");

            Exception? inner = e;
            while (inner != null)
            {
                // Append the messages to the StringBuilder.
                sb.AppendLine($"{inner.GetType().Name}:\n{inner}");
                inner = inner.InnerException;
            }

            return sb.ToString();
        }

        public virtual async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandlerAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext lambdaContext, CancellationToken cancellationToken)
        {
            var features = new InvokeFeatures(request, serviceProvider, lambdaContext);

            var scope = serviceProvider.CreateScope();
            try
            {
                features.RequestServices = scope.ServiceProvider;

                var context = CreateContext(features);
                var response = await ProcessRequest(context, features);

                return response;
            }
            finally
            {
                scope.Dispose();
            }
        }

        /// <summary>Processes the current request</summary>
        /// <param name="context">The hosting application request context object.</param>
        /// <param name="features">An <see cref="InvokeFeatures"/> instance.</param>
        /// <param name="rethrowUnhandledError">
        /// If specified, an unhandled exception will be rethrown for custom error handling.
        /// Ensure that the error handling code calls 'this.MarshallResponse(features, 500);' after handling the error to return a the typed Lambda object to the user.
        /// </param>
        protected async Task<APIGatewayHttpApiV2ProxyResponse> ProcessRequest(object context, InvokeFeatures features, bool rethrowUnhandledError = false)
        {
            var defaultStatusCode = 200;
            Exception? ex = null;
            try
            {
                try
                {
                    await lambdaServer.Application!.ProcessRequestAsync(context);
                }
                catch (AggregateException e)
                {
                    ex = e;
                    logger.LogError(e, $"Caught AggregateException: '{e}'");
                    var sb = new StringBuilder();
                    foreach (var newEx in e.InnerExceptions)
                    {
                        sb.AppendLine(ErrorReport(newEx));
                    }

                    logger.LogError(sb.ToString());
                    features.StatusCode = 500;
                }
                catch (ReflectionTypeLoadException e)
                {
                    ex = e;
                    logger.LogError(e, $"Caught ReflectionTypeLoadException: '{e}'");
                    var sb = new StringBuilder();
                    foreach (var loaderException in e.LoaderExceptions)
                    {
                        if (loaderException is FileNotFoundException fileNotFoundException && !string.IsNullOrEmpty(fileNotFoundException.FileName))
                        {
                            sb.AppendLine($"Missing file: {fileNotFoundException.FileName}");
                        }
                        else if (loaderException != null)
                        {
                            sb.AppendLine(ErrorReport(loaderException));
                        }
                    }

                    logger.LogError(sb.ToString());
                    features.StatusCode = 500;
                }
                catch (Exception e)
                {
                    ex = e;
                    if (rethrowUnhandledError) throw;
                    logger.LogError(e, $"Unknown error responding to request: {ErrorReport(e)}");
                    features.StatusCode = 500;
                }

                if (features.ResponseStartingEvents != null)
                {
                    await features.ResponseStartingEvents.ExecuteAsync();
                }
                var response = MarshallResponse(features, defaultStatusCode);

                if (ex != null && IncludeUnhandledExceptionDetailInResponse)
                {
                    InternalCustomResponseExceptionHandling(response, ex);
                }

                if (features.ResponseCompletedEvents != null)
                {
                    await features.ResponseCompletedEvents.ExecuteAsync();
                }

                return response;
            }
            finally
            {
                lambdaServer!.Application!.DisposeContext(context, ex);
            }
        }


        private protected static void InternalCustomResponseExceptionHandling(APIGatewayHttpApiV2ProxyResponse apiGatewayResponse, Exception ex)
        {
            apiGatewayResponse.SetHeaderValues("ErrorType", [ex.GetType().Name], false);
        }

        /// <summary>
        /// Convert the response coming from ASP.NET Core into APIGatewayProxyResponse which is
        /// serialized into the JSON object that API Gateway expects.
        /// </summary>
        /// <param name="responseFeatures"></param>
        /// <param name="statusCodeIfNotSet">Sometimes the ASP.NET server doesn't set the status code correctly when successful, so this parameter will be used when the value is 0.</param>
        protected APIGatewayHttpApiV2ProxyResponse MarshallResponse<TFeature>(TFeature responseFeatures, int statusCodeIfNotSet = 200)
            where TFeature : IHttpResponseFeature, IHttpResponseBodyFeature
        {
            var response = new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = responseFeatures.StatusCode != 0 ? responseFeatures.StatusCode : statusCodeIfNotSet
            };

            string? contentType = null;
            string? contentEncoding = null;
            if (responseFeatures.Headers != null)
            {
                foreach (var kvp in responseFeatures.Headers)
                {
                    if (kvp.Key.Equals(HeaderNames.SetCookie, StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Cookies must be passed through the proxy response property and not as a 
                        // header to be able to pass back multiple cookies in a single request.
                        response.Cookies = kvp.Value.ToArray();
                        continue;
                    }

                    response.SetHeaderValues(kvp.Key, kvp.Value.ToArray(), false);

                    // Remember the Content-Type for possible later use
                    if (kvp.Key.Equals("Content-Type", StringComparison.CurrentCultureIgnoreCase) && response.Headers[kvp.Key]?.Length > 0)
                    {
                        contentType = response.Headers[kvp.Key];
                    }
                    else if (kvp.Key.Equals("Content-Encoding", StringComparison.CurrentCultureIgnoreCase) && response.Headers[kvp.Key]?.Length > 0)
                    {
                        contentEncoding = response.Headers[kvp.Key];
                    }
                }
            }

            if (contentType == null)
            {
                response.Headers["Content-Type"] = null;
            }

            if (responseFeatures.Stream != null)
            {
                // Figure out how we should treat the response content, check encoding first to see if body is compressed, then check content type
                var rcEncoding = GetAPIGatewayHttpApiV2ProxyResponseContentEncodingForContentEncoding(contentEncoding);
                if (rcEncoding != ResponseContentEncoding.Base64)
                {
                    rcEncoding = GetAPIGatewayHttpApiV2ProxyResponseContentEncodingForContentType(contentType);
                }

                (response.Body, response.IsBase64Encoded) = Utilities.ConvertAspNetCoreBodyToLambdaBody(responseFeatures.Stream, rcEncoding);
            }

            return response;
        }
    }
}
