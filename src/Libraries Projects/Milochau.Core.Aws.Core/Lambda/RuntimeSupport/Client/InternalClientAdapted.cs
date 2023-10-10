using System.Text.Json;
using System.Net;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client
{
    internal partial interface IInternalRuntimeApiClient
    {
        /// <summary>Runtime makes this HTTP request when it is ready to receive and process a new invoke.</summary>
        /// <returns>This is an iterator-style blocking API call. Response contains event JSON document, specific to the invoking service.</returns>
        /// <exception cref="RuntimeApiClientException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<SwaggerResponse<System.IO.Stream>> NextAsync(System.Threading.CancellationToken cancellationToken);

        /// <summary>Runtime makes this request in order to submit a response.</summary>
        /// <returns>Accepted</returns>
        /// <exception cref="RuntimeApiClientException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<SwaggerResponse<StatusResponse>> ResponseAsync(string awsRequestId, System.IO.Stream outputStream, System.Threading.CancellationToken cancellationToken);

        /// <summary>
        /// Runtime makes this request in order to submit an error response. It can be either a function error, or a runtime error. Error will be served in response to the invoke.
        /// </summary>
        /// <param name="awsRequestId"></param>
        /// <param name="lambda_Runtime_Function_Error_Type"></param>
        /// <param name="errorJson"></param>
        /// <param name="xrayCause"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<SwaggerResponse<StatusResponse>> ErrorWithXRayCauseAsync(string awsRequestId, string lambda_Runtime_Function_Error_Type, string errorJson, string xrayCause, System.Threading.CancellationToken cancellationToken);
    }

    internal partial class InternalRuntimeApiClient : IInternalRuntimeApiClient
    {

        [JsonSerializable(typeof(StatusResponse))]
        [JsonSerializable(typeof(ErrorResponse))]
        public partial class RuntimeApiSerializationContext : JsonSerializerContext 
        { 
        }

        private const int MAX_HEADER_SIZE_BYTES = 1024 * 1024;

        private const string ErrorContentType = "application/vnd.aws.lambda.error+json";
        private readonly System.Net.Http.HttpClient _httpClient;

        public InternalRuntimeApiClient(System.Net.Http.HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string BaseUrl { get; set; } = "/2018-06-01";

        /// <summary>Runtime makes this HTTP request when it is ready to receive and process a new invoke.</summary>
        /// <returns>This is an iterator-style blocking API call. Response contains event JSON document, specific to the invoking service.</returns>
        /// <exception cref="RuntimeApiClientException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async System.Threading.Tasks.Task<SwaggerResponse<System.IO.Stream>> NextAsync(System.Threading.CancellationToken cancellationToken)
        {
            var client_ = _httpClient;
            using var request_ = new System.Net.Http.HttpRequestMessage();
            request_.Method = new System.Net.Http.HttpMethod("GET");
            request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

            var url_ = BaseUrl.TrimEnd('/') + "/runtime/invocation/next";
            request_.RequestUri = new System.Uri(url_, System.UriKind.Absolute);

            var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            try
            {
                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                foreach (var item_ in response_.Content.Headers)
                    headers_[item_.Key] = item_.Value;

                if (response_.StatusCode == HttpStatusCode.OK)
                {
                    var inputBuffer = response_.Content == null ? null : await response_.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                    return new SwaggerResponse<System.IO.Stream>(headers_, new System.IO.MemoryStream(inputBuffer));
                }
                else if (response_.StatusCode == HttpStatusCode.Forbidden)
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var result_ = default(ErrorResponse);
                    try
                    {
                        result_ = JsonSerializer.Deserialize(responseData_, RuntimeApiSerializationContext.Default.ErrorResponse);
                    }
                    catch (System.Exception exception_)
                    {
                        throw new RuntimeApiClientException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, exception_);
                    }
                    throw new RuntimeApiClientException<ErrorResponse>("Forbidden", (int)response_.StatusCode, responseData_, result_, null);
                }
                else if (response_.StatusCode == HttpStatusCode.InternalServerError)
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new RuntimeApiClientException("Container error. Non-recoverable state. Runtime should exit promptly.\n", (int)response_.StatusCode, responseData_, null);
                }
                else if (response_.StatusCode != HttpStatusCode.OK && response_.StatusCode != HttpStatusCode.NoContent)
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new RuntimeApiClientException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, null);
                }

                return new SwaggerResponse<System.IO.Stream>(headers_, new System.IO.MemoryStream(0));
            }
            finally
            {
                response_?.Dispose();
            }
        }

        /// <summary>Runtime makes this request in order to submit a response.</summary>
        /// <returns>Accepted</returns>
        /// <exception cref="RuntimeApiClientException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async System.Threading.Tasks.Task<SwaggerResponse<StatusResponse>> ResponseAsync(string awsRequestId, System.IO.Stream outputStream, System.Threading.CancellationToken cancellationToken)
        {
            if (awsRequestId == null)
                throw new System.ArgumentNullException(nameof(awsRequestId));

            var client_ = _httpClient;
            var request_ = new System.Net.Http.HttpRequestMessage();
            {
                var content_ = outputStream == null ?
                    (System.Net.Http.HttpContent)new System.Net.Http.StringContent(string.Empty) :
                    (System.Net.Http.HttpContent)new System.Net.Http.StreamContent(new NonDisposingStreamWrapper(outputStream));

                try
                {
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    var url_ = $"{BaseUrl.TrimEnd('/')}/runtime/invocation/{awsRequestId}/response";
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        if (response_.StatusCode == HttpStatusCode.Accepted)
                        {
                            return new SwaggerResponse<StatusResponse>(headers_, new StatusResponse());
                        }
                        else
                        if (response_.StatusCode == HttpStatusCode.BadRequest)
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            var result_ = default(ErrorResponse);
                            try
                            {
                                result_ = JsonSerializer.Deserialize(responseData_, RuntimeApiSerializationContext.Default.ErrorResponse);
                            }
                            catch (System.Exception exception_)
                            {
                                throw new RuntimeApiClientException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, exception_);
                            }
                            throw new RuntimeApiClientException<ErrorResponse>("Bad Request", (int)response_.StatusCode, responseData_, result_, null);
                        }
                        else
                        if (response_.StatusCode == HttpStatusCode.Forbidden)
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            var result_ = default(ErrorResponse);
                            try
                            {
                                result_ = JsonSerializer.Deserialize(responseData_, RuntimeApiSerializationContext.Default.ErrorResponse);
                            }
                            catch (System.Exception exception_)
                            {
                                throw new RuntimeApiClientException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, exception_);
                            }
                            throw new RuntimeApiClientException<ErrorResponse>("Forbidden", (int)response_.StatusCode, responseData_, result_, null);
                        }
                        else
                        if (response_.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            var result_ = default(ErrorResponse);
                            try
                            {
                                result_ = JsonSerializer.Deserialize(responseData_, RuntimeApiSerializationContext.Default.ErrorResponse);
                            }
                            catch (System.Exception exception_)
                            {
                                throw new RuntimeApiClientException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, exception_);
                            }
                            throw new RuntimeApiClientException<ErrorResponse>("Payload Too Large", (int)response_.StatusCode, responseData_, result_, null);
                        }
                        else
                        if (response_.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new RuntimeApiClientException("Container error. Non-recoverable state. Runtime should exit promptly.\n", (int)response_.StatusCode, responseData_, null);
                        }
                        else
                        if (response_.StatusCode != HttpStatusCode.OK && response_.StatusCode != HttpStatusCode.NoContent)
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new RuntimeApiClientException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, null);
                        }

                        return new SwaggerResponse<StatusResponse>(headers_, default);
                    }
                    finally
                    {
                        response_?.Dispose();
                    }
                }
                finally
                {
                    content_?.Dispose();
                }
            }
        }

        /// <summary>
        /// This is a copy of the generated Error2Async method but adds support for the unmodeled header `Lambda-Runtime-Function-XRay-Error-Cause`.
        /// </summary>
        /// <param name="awsRequestId"></param>
        /// <param name="lambda_Runtime_Function_Error_Type"></param>
        /// <param name="errorJson"></param>
        /// <param name="xrayCause"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<SwaggerResponse<StatusResponse>> ErrorWithXRayCauseAsync(string awsRequestId, string lambda_Runtime_Function_Error_Type, string errorJson, string xrayCause, System.Threading.CancellationToken cancellationToken)
        {
            if (awsRequestId == null)
                throw new System.ArgumentNullException(nameof(awsRequestId));

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/runtime/invocation/{AwsRequestId}/error");
            urlBuilder_.Replace("{AwsRequestId}", System.Uri.EscapeDataString(awsRequestId));

            var client_ = _httpClient;
            try
            {
                using var request_ = new System.Net.Http.HttpRequestMessage();
                if (lambda_Runtime_Function_Error_Type != null)
                    request_.Headers.TryAddWithoutValidation("Lambda-Runtime-Function-Error-Type", lambda_Runtime_Function_Error_Type);

                // This is the unmodeled X-Ray header to report back the cause of errors.
                if (xrayCause != null && System.Text.Encoding.UTF8.GetByteCount(xrayCause) < MAX_HEADER_SIZE_BYTES)
                {
                    // Headers can not have newlines. The X-Ray JSON writer should not have put any in but do a final check of newlines.
                    xrayCause = xrayCause.Replace("\r\n", "").Replace("\n", "");

                    try
                    {
                        request_.Headers.Add("Lambda-Runtime-Function-XRay-Error-Cause", xrayCause);
                    }
                    catch
                    {
                        // Don't prevent reporting errors to Lambda if there are any issues adding the X-Ray cause JSON as a header.
                    }
                }

                using var content_ = new System.Net.Http.StringContent(errorJson);
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(ErrorContentType);
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    if (response_.StatusCode == HttpStatusCode.Accepted)
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var result_ = default(StatusResponse);
                        try
                        {
                            result_ = JsonSerializer.Deserialize(responseData_, RuntimeApiSerializationContext.Default.StatusResponse);
                            return new SwaggerResponse<StatusResponse>(headers_, result_);
                        }
                        catch (System.Exception exception_)
                        {
                            throw new RuntimeApiClientException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, exception_);
                        }
                    }
                    else
                    if (response_.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var result_ = default(ErrorResponse);
                        try
                        {
                            result_ = JsonSerializer.Deserialize(responseData_, RuntimeApiSerializationContext.Default.ErrorResponse);
                        }
                        catch (System.Exception exception_)
                        {
                            throw new RuntimeApiClientException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, exception_);
                        }
                        throw new RuntimeApiClientException<ErrorResponse>("Bad Request", (int)response_.StatusCode, responseData_, result_, null);
                    }
                    else
                    if (response_.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var result_ = default(ErrorResponse);
                        try
                        {
                            result_ = JsonSerializer.Deserialize(responseData_, RuntimeApiSerializationContext.Default.ErrorResponse);
                        }
                        catch (System.Exception exception_)
                        {
                            throw new RuntimeApiClientException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, exception_);
                        }
                        throw new RuntimeApiClientException<ErrorResponse>("Forbidden", (int)response_.StatusCode, responseData_, result_, null);
                    }
                    else
                    if (response_.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new RuntimeApiClientException("Container error. Non-recoverable state. Runtime should exit promptly.\n", (int)response_.StatusCode, responseData_, null);
                    }
                    else
                    if (response_.StatusCode != HttpStatusCode.OK && response_.StatusCode != HttpStatusCode.NoContent)
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new RuntimeApiClientException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, null);
                    }

                    return new SwaggerResponse<StatusResponse>(headers_, default);
                }
                finally
                {
                    response_?.Dispose();
                }
            }
            finally
            {
            }
        }
    }

    internal partial class StatusResponse
    {
        public string status { get; set; }
    }

    internal partial class ErrorResponse
    {
        public string errorMessage { get; set; }
        public string errorType { get; set; }
    }

    internal partial class SwaggerResponse
    {
        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }

        public SwaggerResponse(System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> headers)
        {
            Headers = headers;
        }
    }

    internal partial class SwaggerResponse<TResult> : SwaggerResponse
    {
        public TResult Result { get; private set; }

        public SwaggerResponse(System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> headers, TResult result)
            : base(headers)
        {
            Result = result;
        }
    }

    public partial class RuntimeApiClientException : System.Exception
    {
        public string Response { get; private set; }

        public RuntimeApiClientException(string message, int statusCode, string response, System.Exception? innerException)
            : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + response.Substring(0, response.Length >= 512 ? 512 : response.Length), innerException)
        {
            Response = response;
        }

        public override string ToString()
        {
            return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }

    public partial class RuntimeApiClientException<TResult> : RuntimeApiClientException
    {
        public TResult Result { get; private set; }

        public RuntimeApiClientException(string message, int statusCode, string response, TResult result, System.Exception? innerException)
            : base(message, statusCode, response, innerException)
        {
            Result = result;
        }
    }

}