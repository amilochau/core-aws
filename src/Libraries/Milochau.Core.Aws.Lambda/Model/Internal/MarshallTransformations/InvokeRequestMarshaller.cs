using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Util;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System;

namespace Milochau.Core.Aws.Lambda.Model.MarshallTransformations
{
    /// <summary>
    /// Invoke Request Marshaller
    /// </summary>       
    public static class InvokeRequestMarshaller
    {
        /// <summary>Creates an HTTP request message to call the service</summary>
        public static HttpRequestMessage CreateHttpRequestMessage(InvokeRequest publicRequest)
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
    }
}