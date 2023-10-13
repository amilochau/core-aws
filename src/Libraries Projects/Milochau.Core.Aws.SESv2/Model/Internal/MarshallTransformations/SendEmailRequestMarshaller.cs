using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Util;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Milochau.Core.Aws.SESv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// SendEmail Request Marshaller
    /// </summary>       
    public class SendEmailRequestMarshaller : IMarshaller<IRequest, SendEmailRequest> , IMarshaller<IRequest,AmazonWebServiceRequest>, IHttpRequestMessageMarshaller<AmazonWebServiceRequest>
    {
        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        public IRequest Marshall(AmazonWebServiceRequest input)
        {
            return Marshall((SendEmailRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        public IRequest Marshall(SendEmailRequest publicRequest)
        {
            var serializedRequest = JsonSerializer.Serialize(publicRequest, SendEmailJsonSerializerContext.Default.SendEmailRequest);

            IRequest request = new DefaultRequest(publicRequest, "Amazon.SimpleEmailV2")
            {
                HttpMethod = "POST",
                ResourcePath = "/v2/email/outbound-emails",
                Content = System.Text.Encoding.UTF8.GetBytes(serializedRequest)
            };
            request.Headers["Content-Type"] = "application/json";
            request.Headers[HeaderKeys.XAmzApiVersion] = "2019-09-27";

            return request;
        }

        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage CreateHttpRequestMessage(AmazonWebServiceRequest input)
        {
            return CreateHttpRequestMessage((SendEmailRequest)input);
        }

        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage CreateHttpRequestMessage(SendEmailRequest publicRequest)
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
        /// Gets the singleton.
        /// </summary>  
        public static SendEmailRequestMarshaller Instance { get; } = new SendEmailRequestMarshaller();
    }
}