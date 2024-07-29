using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Util;
using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Milochau.Core.Aws.Cognito.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// GetUser Request Marshaller
    /// </summary>       
    public static class GetUserRequestMarshaller
    {
        /// <summary>Creates an HTTP request message to call the service</summary>
        public static HttpRequestMessage CreateHttpRequestMessage(GetUserRequest publicRequest)
        {
            var serializedRequest = JsonSerializer.Serialize(publicRequest, GetUserJsonSerializerContext.Default.GetUserRequest);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://cognito-idp.{EnvironmentVariables.RegionName}.amazonaws.com/"),
                Content = new StringContent(serializedRequest, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/x-amz-json-1.1")),
            };
            httpRequestMessage.Headers.Add("X-Amz-Target", "AWSCognitoIdentityProviderService.GetUser");
            httpRequestMessage.Headers.Add(HeaderKeys.XAmzApiVersion, "2016-04-18");

            return httpRequestMessage;
        }
    }
}
