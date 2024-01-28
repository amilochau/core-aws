using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Runtime.Internal;
using System;
using System.Text;
using System.Net.Http;
using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Util;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Milochau.Core.Aws.Cognito.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// InitiateAuth Request Marshaller
    /// </summary>       
    public class InitiateAuthRequestMarshaller : IHttpRequestMessageMarshaller<AmazonWebServiceRequest>
    {
        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage CreateHttpRequestMessage(AmazonWebServiceRequest input)
        {
            return CreateHttpRequestMessage((InitiateAuthRequest)input);
        }

        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage CreateHttpRequestMessage(InitiateAuthRequest publicRequest)
        {
            var serializedRequest = JsonSerializer.Serialize(publicRequest, InitiateAuthJsonSerializerContext.Default.InitiateAuthRequest);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://cognito-idp.{EnvironmentVariables.RegionName}.amazonaws.com/"),
                Content = new StringContent(serializedRequest, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/x-amz-json-1.1")),
            };
            httpRequestMessage.Headers.Add("X-Amz-Target", "AWSCognitoIdentityProviderService.InitiateAuth");
            httpRequestMessage.Headers.Add(HeaderKeys.XAmzApiVersion, "2016-04-18");

            return httpRequestMessage;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        public static InitiateAuthRequestMarshaller Instance { get; } = new InitiateAuthRequestMarshaller();
    }
}
