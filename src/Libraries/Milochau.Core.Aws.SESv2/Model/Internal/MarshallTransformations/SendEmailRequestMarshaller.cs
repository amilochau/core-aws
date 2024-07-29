using Milochau.Core.Aws.Core.References;
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
    public static class SendEmailRequestMarshaller
    {
        /// <summary>Creates an HTTP request message to call the service</summary>
        public static HttpRequestMessage CreateHttpRequestMessage(SendEmailRequest publicRequest)
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
    }
}