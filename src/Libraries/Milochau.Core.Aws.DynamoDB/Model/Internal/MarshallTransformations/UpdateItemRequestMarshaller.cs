using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Util;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System;
using System.Text.Json;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// UpdateItem Request Marshaller
    /// </summary>
    public static class UpdateItemRequestMarshaller
    {
        /// <summary>Creates an HTTP request message to call the service</summary>
        public static HttpRequestMessage CreateHttpRequestMessage(UpdateItemRequest publicRequest)
        {
            var serializedRequest = JsonSerializer.Serialize(publicRequest, UpdateItemJsonSerializerContext.Default.UpdateItemRequest);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://dynamodb.{EnvironmentVariables.RegionName}.amazonaws.com/"),
                Content = new StringContent(serializedRequest, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/x-amz-json-1.0")),
            };
            httpRequestMessage.Headers.Add("X-Amz-Target", "DynamoDB_20120810.UpdateItem");
            httpRequestMessage.Headers.Add(HeaderKeys.XAmzApiVersion, "2012-08-10");

            return httpRequestMessage;
        }
    }
}
