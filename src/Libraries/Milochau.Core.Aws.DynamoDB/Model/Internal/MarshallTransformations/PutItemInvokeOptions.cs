using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Util;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System;
using System.Text.Json;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Runtime;
using System.Net;
using Milochau.Core.Aws.Core.Runtime.Internal;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// PutItem Request Marshaller
    /// </summary>
    internal class PutItemInvokeOptions : IInvokeOptions<PutItemRequest, PutItemResponse>
    {
        public string MonitoringOriginalRequestName { get; } = "PutItem";

        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage MarshallRequest(PutItemRequest publicRequest)
        {
            var serializedRequest = JsonSerializer.Serialize(publicRequest, PutItemJsonSerializerContext.Default.PutItemRequest);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://dynamodb.{EnvironmentVariables.RegionName}.amazonaws.com/"),
                Content = new StringContent(serializedRequest, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/x-amz-json-1.0")),
            };
            httpRequestMessage.Headers.Add("X-Amz-Target", "DynamoDB_20120810.PutItem");
            httpRequestMessage.Headers.Add(HeaderKeys.XAmzApiVersion, "2012-08-10");

            return httpRequestMessage;
        }

        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>
        public PutItemResponse UnmarshallResponse(JsonUnmarshallerContext context)
        {
            return JsonSerializer.Deserialize(context.Stream, PutItemJsonSerializerContext.Default.PutItemResponse)!; // @todo null?
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
    [JsonSerializable(typeof(PutItemRequest))]
    [JsonSerializable(typeof(PutItemResponse))]
    internal partial class PutItemJsonSerializerContext : JsonSerializerContext
    {
    }
}
