using System.Net;
using Milochau.Core.Aws.Core.References;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Net.Http.Headers;
using System.IO;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client
{
    internal interface IInternalRuntimeApiClient
    {
        /// <summary>Runtime makes this HTTP request when it is ready to receive and process a new invoke.</summary>
        /// <returns>This is an iterator-style blocking API call. Response contains event JSON document, specific to the invoking service.</returns>
        /// <exception cref="RuntimeApiClientException">A server side error occurred.</exception>
        Task<HttpResponseMessage> NextAsync(CancellationToken cancellationToken);

        /// <summary>Runtime makes this request in order to submit a response.</summary>
        /// <returns>Accepted</returns>
        /// <exception cref="RuntimeApiClientException">A server side error occurred.</exception>
        Task ResponseAsync(string awsRequestId, Stream outputStream, CancellationToken cancellationToken);

        /// <summary>
        /// Runtime makes this request in order to submit an error response. It can be either a function error, or a runtime error. Error will be served in response to the invoke.
        /// </summary>
        Task ErrorWithXRayCauseAsync(string awsRequestId, string lambda_Runtime_Function_Error_Type, string errorJson, string xrayCause, CancellationToken cancellationToken);
    }

    internal class InternalRuntimeApiClient : IInternalRuntimeApiClient
    {
        private const int MAX_HEADER_SIZE_BYTES = 1024 * 1024;

        private const string ErrorContentType = "application/vnd.aws.lambda.error+json";


        public string BaseUrl { get; } = "http://" + EnvironmentVariables.RuntimeServerHostAndPort + "/2018-06-01";

        /// <summary>Runtime makes this HTTP request when it is ready to receive and process a new invoke.</summary>
        /// <returns>This is an iterator-style blocking API call. Response contains event JSON document, specific to the invoking service.</returns>
        /// <exception cref="RuntimeApiClientException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task<HttpResponseMessage> NextAsync(CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(BaseUrl + "/runtime/invocation/next", UriKind.Absolute),
            };
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

            var response = await HttpClients.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false); // Do not dispose in this method!
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
            {
                return response;
            }

            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var message = response.StatusCode switch
            {
                HttpStatusCode.Forbidden => "Forbidden",
                HttpStatusCode.InternalServerError => "Container error. Non-recoverable state. Runtime should exit promptly.\n",
                _ => "The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").",
            };
            throw new RuntimeApiClientException(message, (int)response.StatusCode, responseData, null);
        }

        /// <summary>Runtime makes this request in order to submit a response.</summary>
        /// <returns>Accepted</returns>
        /// <exception cref="RuntimeApiClientException">A server side error occurred.</exception>
        public async Task ResponseAsync(string awsRequestId, Stream outputStream, CancellationToken cancellationToken)
        {
            if (awsRequestId == null)
                throw new ArgumentNullException(nameof(awsRequestId));

            using var content = new StreamContent(outputStream);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseUrl}/runtime/invocation/{awsRequestId}/response", UriKind.RelativeOrAbsolute),
                Method = HttpMethod.Post,
                Content = content,
            };
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

            using var response = await HttpClients.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
            {
                return;
            }

            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var message = response.StatusCode switch
            {
                HttpStatusCode.BadRequest => "Bad Request",
                HttpStatusCode.Forbidden => "Forbidden",
                HttpStatusCode.RequestEntityTooLarge => "Payload Too Large",
                HttpStatusCode.InternalServerError => "Container error. Non-recoverable state. Runtime should exit promptly.\n",
                _ => "The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").",
            };
            throw new RuntimeApiClientException(message, (int)response.StatusCode, responseData, null);
        }

        /// <summary>
        /// This is a copy of the generated Error2Async method but adds support for the unmodeled header `Lambda-Runtime-Function-XRay-Error-Cause`.
        /// </summary>
        public async Task ErrorWithXRayCauseAsync(string awsRequestId, string lambda_Runtime_Function_Error_Type, string errorJson, string xrayCause, CancellationToken cancellationToken)
        {
            if (awsRequestId == null)
                throw new ArgumentNullException(nameof(awsRequestId));

            using var content = new StringContent(errorJson);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse(ErrorContentType);

            using var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseUrl}/runtime/invocation/{Uri.EscapeDataString(awsRequestId)}/error", UriKind.RelativeOrAbsolute),
                Method = HttpMethod.Post,
                Content = content,
            };
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

            if (lambda_Runtime_Function_Error_Type != null)
            {
                request.Headers.TryAddWithoutValidation("Lambda-Runtime-Function-Error-Type", lambda_Runtime_Function_Error_Type);
            }

            // This is the unmodeled X-Ray header to report back the cause of errors.
            if (xrayCause != null && System.Text.Encoding.UTF8.GetByteCount(xrayCause) < MAX_HEADER_SIZE_BYTES)
            {
                // Headers can not have newlines. The X-Ray JSON writer should not have put any in but do a final check of newlines.
                xrayCause = xrayCause.Replace("\r\n", "").Replace("\n", "");

                try
                {
                    request.Headers.Add("Lambda-Runtime-Function-XRay-Error-Cause", xrayCause);
                }
                catch
                {
                    // Don't prevent reporting errors to Lambda if there are any issues adding the X-Ray cause JSON as a header.
                }
            }

            using var response = await HttpClients.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
            {
                return;
            }

            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var message = response.StatusCode switch
            {
                HttpStatusCode.BadRequest => "Bad Request",
                HttpStatusCode.Forbidden => "Forbidden",
                HttpStatusCode.InternalServerError => "Container error. Non-recoverable state. Runtime should exit promptly.\n",
                _ => "The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").",
            };
            throw new RuntimeApiClientException(message, (int)response.StatusCode, responseData, null);
        }
    }

    internal class RuntimeApiClientException : Exception
    {
        public string Response { get; private set; }

        public RuntimeApiClientException(string message, int statusCode, string response, Exception? innerException)
            : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + response.Substring(0, response.Length >= 512 ? 512 : response.Length), innerException)
        {
            Response = response;
        }

        public override string ToString()
        {
            return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }
}