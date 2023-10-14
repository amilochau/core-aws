using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Util;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// GetItem Request Marshaller
    /// </summary>
    public class GetItemRequestMarshaller : IHttpRequestMessageMarshaller<AmazonWebServiceRequest>
    {
        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage CreateHttpRequestMessage(AmazonWebServiceRequest input)
        {
            return CreateHttpRequestMessage((GetItemRequest)input);
        }

        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage CreateHttpRequestMessage(GetItemRequest publicRequest)
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
        /// Gets the singleton.
        /// </summary>
        public static GetItemRequestMarshaller Instance { get; } = new GetItemRequestMarshaller();
    }
}
