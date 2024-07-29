using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Util;
using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Runtime;
using System.Net;
using System.Text.Json.Serialization;
using Milochau.Core.Aws.Core.Runtime.Internal;

namespace Milochau.Core.Aws.Cognito.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// AdminUpdateUserAttributes Request Marshaller
    /// </summary>       
    internal class AdminUpdateUserAttributesInvokeOptions : IInvokeOptions<AdminUpdateUserAttributesRequest, AdminUpdateUserAttributesResponse>
    {
        public string MonitoringOriginalRequestName { get; } = "AdminUpdateUserAttributes";

        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage MarshallRequest(AdminUpdateUserAttributesRequest publicRequest)
        {
            var serializedRequest = JsonSerializer.Serialize(publicRequest, AdminUpdateUserAttributesJsonSerializerContext.Default.AdminUpdateUserAttributesRequest);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://cognito-idp.{EnvironmentVariables.RegionName}.amazonaws.com/"),
                Content = new StringContent(serializedRequest, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/x-amz-json-1.1")),
            };
            httpRequestMessage.Headers.Add("X-Amz-Target", "AWSCognitoIdentityProviderService.AdminUpdateUserAttributes");
            httpRequestMessage.Headers.Add(HeaderKeys.XAmzApiVersion, "2016-04-18");

            return httpRequestMessage;
        }

        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        public AdminUpdateUserAttributesResponse UnmarshallResponse(JsonUnmarshallerContext context)
        {
            return JsonSerializer.Deserialize(context.Stream, AdminUpdateUserAttributesJsonSerializerContext.Default.AdminUpdateUserAttributesResponse)!; // @todo null?
        }

        /// <summary>
        /// Unmarshaller error response to exception.
        /// </summary>  
        public AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, HttpStatusCode statusCode)
        {
            var errorResponse = JsonErrorResponseUnmarshaller.Instance.UnmarshallResponse(context);
            errorResponse.StatusCode = statusCode;

            return new AmazonCognitoIdentityProviderException(errorResponse.Message, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, errorResponse.StatusCode);
        }
    }

    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(AdminUpdateUserAttributesRequest))]
    [JsonSerializable(typeof(AdminUpdateUserAttributesResponse))]
    internal partial class AdminUpdateUserAttributesJsonSerializerContext : JsonSerializerContext
    {
    }
}
