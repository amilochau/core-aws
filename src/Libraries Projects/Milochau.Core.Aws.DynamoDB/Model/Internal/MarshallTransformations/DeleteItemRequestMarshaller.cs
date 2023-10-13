using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Util;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System;
using System.Text.Json;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// DeleteItem Request Marshaller
    /// </summary>       
    public class DeleteItemRequestMarshaller : IMarshaller<IRequest, DeleteItemRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>, IHttpRequestMessageMarshaller<AmazonWebServiceRequest>
    {
        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <returns></returns>
        /// @todo remove this method?
        public IRequest Marshall(AmazonWebServiceRequest input)
        {
            return Marshall((DeleteItemRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <returns></returns>
        public IRequest Marshall(DeleteItemRequest publicRequest)
        {
            var serializedRequest = JsonSerializer.Serialize(publicRequest, DeleteItemJsonSerializerContext.Default.DeleteItemRequest);

            IRequest request = new DefaultRequest(publicRequest, "Amazon.DynamoDBv2")
            {
                HttpMethod = "POST",
                ResourcePath = "/",
                Content = System.Text.Encoding.UTF8.GetBytes(serializedRequest)
            };
            request.Headers["X-Amz-Target"] = "DynamoDB_20120810.DeleteItem";
            request.Headers["Content-Type"] = "application/x-amz-json-1.0";
            request.Headers[HeaderKeys.XAmzApiVersion] = "2012-08-10";
           
            return request;
        }

        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage CreateHttpRequestMessage(AmazonWebServiceRequest input)
        {
            return CreateHttpRequestMessage((DeleteItemRequest)input);
        }

        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage CreateHttpRequestMessage(DeleteItemRequest publicRequest)
        {
            var serializedRequest = JsonSerializer.Serialize(publicRequest, DeleteItemJsonSerializerContext.Default.DeleteItemRequest);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://dynamodb.{EnvironmentVariables.RegionName}.amazonaws.com/"),
                Content = new StringContent(serializedRequest, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/x-amz-json-1.0")),
            };
            httpRequestMessage.Headers.Add("X-Amz-Target", "DynamoDB_20120810.DeleteItem");
            httpRequestMessage.Headers.Add(HeaderKeys.XAmzApiVersion, "2012-08-10");

            return httpRequestMessage;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static DeleteItemRequestMarshaller Instance { get; } = new DeleteItemRequestMarshaller();
    }
}
