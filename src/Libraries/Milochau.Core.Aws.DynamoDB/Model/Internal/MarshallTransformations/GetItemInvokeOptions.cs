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
using Milochau.Core.Aws.Core.Runtime.Internal;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// GetItem Request Marshaller
    /// </summary>
    internal class GetItemInvokeOptions : IInvokeOptions<GetItemRequest, GetItemResponse>
    {
        public string MonitoringOriginalRequestName { get; } = "GetItem";

        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage MarshallRequest(GetItemRequest publicRequest)
        {
            var serializedRequest = JsonSerializer.Serialize(publicRequest, GetItemJsonSerializerContext.Default.GetItemRequest);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://dynamodb.{EnvironmentVariables.RegionName}.amazonaws.com/"),
                Content = new StringContent(serializedRequest, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/x-amz-json-1.0")),
            };
            httpRequestMessage.Headers.Add("X-Amz-Target", "DynamoDB_20120810.GetItem");
            httpRequestMessage.Headers.Add(HeaderKeys.XAmzApiVersion, "2012-08-10");

            return httpRequestMessage;
        }

        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>
        public GetItemResponse UnmarshallResponse(JsonUnmarshallerContext context)
        {
            return JsonSerializer.Deserialize(context.Stream, GetItemJsonSerializerContext.Default.GetItemResponse)!; // @todo null?
        }

        /// <summary>
        /// Unmarshaller error response to exception.
        /// </summary>
        public AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, HttpStatusCode statusCode)
        {
            var errorResponse = JsonErrorResponseUnmarshaller.Instance.UnmarshallResponse(context);
            errorResponse.StatusCode = statusCode;

            return new AmazonDynamoDBException(errorResponse.Message, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, errorResponse.StatusCode);
        }
    }

    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(GetItemRequest))]
    [JsonSerializable(typeof(GetItemResponse))]
    internal partial class GetItemJsonSerializerContext : JsonSerializerContext
    {
    }
}
