using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Util;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System;

namespace Milochau.Core.Aws.Lambda.Model.MarshallTransformations
{
    /// <summary>
    /// Invoke Request Marshaller
    /// </summary>       
    public class InvokeRequestMarshaller : IMarshaller<IRequest, InvokeRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>, IHttpRequestMessageMarshaller<AmazonWebServiceRequest>
    {
        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="input"></param>
        /// <returns></returns>
        public IRequest Marshall(AmazonWebServiceRequest input)
        {
            return Marshall((InvokeRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <returns></returns>
        public IRequest Marshall(InvokeRequest publicRequest)
        {
            IRequest request = new DefaultRequest(publicRequest, "Amazon.Lambda")
            {
                HttpMethod = "POST",
                ResourcePath = "/2015-03-31/functions/{FunctionName}/invocations",
                UseQueryString = true,
            };
            request.Headers["Content-Type"] = "application/json";
            request.Headers[HeaderKeys.XAmzApiVersion] = "2015-03-31";

            request.AddPathResource("{FunctionName}", publicRequest.FunctionName);

            request.ContentStream = publicRequest.PayloadStream ?? new MemoryStream();
            if (request.ContentStream.CanSeek)
            {
                request.ContentStream.Seek(0, SeekOrigin.Begin);
            }
            request.Headers[HeaderKeys.ContentLengthHeader] =
                request.ContentStream.Length.ToString(CultureInfo.InvariantCulture);
            request.Headers[HeaderKeys.ContentTypeHeader] = "binary/octet-stream";
            if (request.ContentStream != null && request.ContentStream.Length == 0)
            {
                request.Headers.Remove(HeaderKeys.ContentTypeHeader);
            }

            if (publicRequest.ClientContextBase64 != null)
            {
                request.Headers["X-Amz-Client-Context"] = publicRequest.ClientContextBase64;
            }

            if (publicRequest.InvocationType != null)
            {
                request.Headers["X-Amz-Invocation-Type"] = publicRequest.InvocationType.ToString();
            }

            return request;
        }

        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage CreateHttpRequestMessage(AmazonWebServiceRequest input)
        {
            return CreateHttpRequestMessage((InvokeRequest)input);
        }

        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage CreateHttpRequestMessage(InvokeRequest publicRequest)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://lambda.{EnvironmentVariables.RegionName}.amazonaws.com/2015-03-31/functions/{publicRequest.FunctionName}/invocations"),
                Content = new StringContent(publicRequest.Payload!, Encoding.UTF8, MediaTypeHeaderValue.Parse("binary/octet-stream"))
            };
            httpRequestMessage.Headers.Add(HeaderKeys.XAmzApiVersion, "2015-03-31");

            if (publicRequest.ClientContextBase64 != null)
            {
                httpRequestMessage.Headers.Add("X-Amz-Client-Context", publicRequest.ClientContextBase64);
            }
            if (publicRequest.InvocationType != null)
            {
                httpRequestMessage.Headers.Add("X-Amz-Invocation-Type", publicRequest.InvocationType.ToString());
            }

            return httpRequestMessage;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static InvokeRequestMarshaller Instance { get; } = new InvokeRequestMarshaller();
    }
}