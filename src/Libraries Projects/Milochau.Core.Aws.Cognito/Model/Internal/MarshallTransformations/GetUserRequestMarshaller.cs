﻿using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Cognito.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// GetUser Request Marshaller
    /// </summary>       
    public class GetUserRequestMarshaller : IHttpRequestMessageMarshaller<AmazonWebServiceRequest>
    {
        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage CreateHttpRequestMessage(AmazonWebServiceRequest input)
        {
            return CreateHttpRequestMessage((GetUserRequest)input);
        }

        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage CreateHttpRequestMessage(GetUserRequest publicRequest)
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

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        public static GetUserRequestMarshaller Instance { get; } = new GetUserRequestMarshaller();
    }
}
