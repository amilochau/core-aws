using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Util;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Milochau.Core.Aws.Core.Runtime.Internal;

namespace Milochau.Core.Aws.SESv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// SendEmail Request Marshaller
    /// </summary>       
    internal class SendEmailInvokeOptions : IInvokeOptions<SendEmailRequest, SendEmailResponse>
    {
        public string MonitoringOriginalRequestName { get; } = "SendEmail";

        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage MarshallRequest(SendEmailRequest publicRequest)
        {
            var serializedRequest = JsonSerializer.Serialize(publicRequest, SendEmailJsonSerializerContext.Default.SendEmailRequest);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://email.{EnvironmentVariables.RegionName}.amazonaws.com/v2/email/outbound-emails"),
                Content = new StringContent(serializedRequest, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json")),
            };
            httpRequestMessage.Headers.Add(HeaderKeys.XAmzApiVersion, "2019-09-27");

            return httpRequestMessage;
        }

        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>
        public SendEmailResponse UnmarshallResponse(JsonUnmarshallerContext context)
        {
            return JsonSerializer.Deserialize(context.Stream, SendEmailJsonSerializerContext.Default.SendEmailResponse)!; // @todo null?
        }

        /// <summary>
        /// Unmarshaller error response to exception.
        /// </summary>
        public AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, HttpStatusCode statusCode)
        {
            var errorResponse = JsonErrorResponseUnmarshaller.Instance.UnmarshallResponse(context);
            errorResponse.StatusCode = statusCode;

            return new AmazonSimpleEmailServiceV2Exception(errorResponse.Message, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, errorResponse.StatusCode);
        }
    }

    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(SendEmailRequest))]
    [JsonSerializable(typeof(SendEmailResponse))]
    internal partial class SendEmailJsonSerializerContext : JsonSerializerContext
    {
    }
}