using System;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Milochau.Core.Aws.ReferenceProjects.LambdaFunction;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals
{
    public class InternalRuntimeApiClient
    {
        private readonly HttpClient httpClient;

        private readonly string baseUrl;

        public InternalRuntimeApiClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            baseUrl = "http://" + Environment.GetEnvironmentVariable("AWS_LAMBDA_RUNTIME_API") + "/2018-06-01";
        }

        public async Task<SwaggerResponse<System.IO.Stream?>> NextAsync()
        {
            var urlBuilder = new System.Text.StringBuilder().Append(baseUrl).Append("/runtime/invocation/next");

            using var request = new HttpRequestMessage();
            request.Method = new HttpMethod("GET");
            request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

            request.RequestUri = new Uri(urlBuilder.ToString(), UriKind.Absolute);

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            var headers = Enumerable.ToDictionary(response.Headers, h_ => h_.Key, h_ => h_.Value);
            foreach (var item_ in response.Content.Headers)
                headers[item_.Key] = item_.Value;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                byte[]? inputBuffer = null;
                System.IO.MemoryStream? result = null;
                if (response.Content != null)
                {
                    inputBuffer = await response.Content.ReadAsByteArrayAsync();
                    result = new System.IO.MemoryStream(inputBuffer);
                }
                return new SwaggerResponse<System.IO.Stream?>((int)response.StatusCode, headers, result);
            }
            else if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
            {
                throw new Exception($"Error (code: {response.StatusCode})");
            }

            return new SwaggerResponse<System.IO.Stream?>((int)response.StatusCode, headers, new System.IO.MemoryStream(0));
        }

        public async Task<SwaggerResponse<StatusResponse?>> ErrorWithXRayCauseAsync(string awsRequestId, string lambda_Runtime_Function_Error_Type, string errorJson)
        {
            var urlBuilder = new System.Text.StringBuilder().Append(baseUrl).Append("/runtime/invocation/{AwsRequestId}/error").Replace("{AwsRequestId}", Uri.EscapeDataString(awsRequestId));

            using var request = new HttpRequestMessage();
            if (lambda_Runtime_Function_Error_Type != null)
                request.Headers.TryAddWithoutValidation("Lambda-Runtime-Function-Error-Type", lambda_Runtime_Function_Error_Type);

            using var content = new StringContent(errorJson);
            content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/vnd.aws.lambda.error+json");
            request.Content = content;
            request.Method = new HttpMethod("POST");
            request.RequestUri = new Uri(urlBuilder.ToString(), UriKind.RelativeOrAbsolute);
            request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            var headers = Enumerable.ToDictionary(response.Headers, h_ => h_.Key, h_ => h_.Value);
            if (response.Content != null && response.Content.Headers != null)
            {
                foreach (var item_ in response.Content.Headers)
                    headers[item_.Key] = item_.Value;
            }

            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                string? responseData = null;
                StatusResponse? result = default;
                if (response.Content != null)
                {
                    responseData = await response.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize(responseData, ApplicationJsonSerializerContext.Default.StatusResponse);
                }
                try
                {
                    return new SwaggerResponse<StatusResponse?>((int)response.StatusCode, headers, result);
                }
                catch (Exception exception)
                {
                    throw new Exception($"Could not deserialize the response body (code: {response.StatusCode})", exception);
                }
            }
            else if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
            {
                throw new Exception($"Error (code: {response.StatusCode})");
            }

            return new SwaggerResponse<StatusResponse?>((int)response.StatusCode, headers, default);
        }

        /// <summary>Runtime makes this request in order to submit a response.</summary>
        /// <returns>Accepted</returns>
        /// <exception cref="RuntimeApiClientException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task<SwaggerResponse<StatusResponse?>> ResponseAsync(string awsRequestId, System.IO.Stream? outputStream)
        {
            var urlBuilder = new System.Text.StringBuilder().Append(baseUrl).Append("/runtime/invocation/{AwsRequestId}/response").Replace("{AwsRequestId}", Uri.EscapeDataString(awsRequestId));

            using var request = new HttpRequestMessage();
            using HttpContent content = outputStream == null ? new StringContent(string.Empty) : new StreamContent(new NonDisposingStreamWrapper(outputStream));

            content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
            request.Content = content;
            request.Method = new HttpMethod("POST");
            request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
            request.RequestUri = new Uri(urlBuilder.ToString(), UriKind.RelativeOrAbsolute);

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            var headers = Enumerable.ToDictionary(response.Headers, h_ => h_.Key, h_ => h_.Value);
            if (response.Content != null && response.Content.Headers != null)
            {
                foreach (var item_ in response.Content.Headers)
                    headers[item_.Key] = item_.Value;
            }

            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                return new SwaggerResponse<StatusResponse?>((int)response.StatusCode, headers, new StatusResponse());
            }
            else if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
            {
                throw new Exception($"Error (code: {response.StatusCode})");
            }

            return new SwaggerResponse<StatusResponse?>((int)response.StatusCode, headers, default);
        }

    }
}
