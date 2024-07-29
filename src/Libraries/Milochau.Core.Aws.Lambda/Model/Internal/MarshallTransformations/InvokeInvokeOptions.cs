using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Util;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Runtime;
using System.IO;
using System.Net;
using Milochau.Core.Aws.Core.Runtime.Internal;
using System.Linq;

namespace Milochau.Core.Aws.Lambda.Model.MarshallTransformations
{
    /// <summary>
    /// Invoke Request Marshaller
    /// </summary>       
    internal class InvokeInvokeOptions : IInvokeOptions<InvokeRequest, InvokeResponse>
    {
        public string MonitoringOriginalRequestName { get; } = "Invoke";

        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage MarshallRequest(InvokeRequest publicRequest)
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
        /// Unmarshaller the response from the service to the response class.
        /// </summary>
        public InvokeResponse UnmarshallResponse(JsonUnmarshallerContext context)
        {
            var response = new InvokeResponse();

            var ms = new MemoryStream();
            AWSSDKUtils.CopyStream(context.Stream, ms);
            ms.Seek(0, SeekOrigin.Begin);
            response.Payload = ms;
            if (context.ResponseData.Headers.Contains("X-Amz-Function-Error"))
                response.FunctionError = context.ResponseData.Headers.GetValues("X-Amz-Function-Error").FirstOrDefault();
            if (context.ResponseData.Headers.Contains("X-Amz-Log-Result"))
                response.LogResult = context.ResponseData.Headers.GetValues("X-Amz-Log-Result").FirstOrDefault();
            response.StatusCode = (int)context.ResponseData.StatusCode;

            return response;
        }

        /// <summary>
        /// Unmarshaller error response to exception.
        /// </summary>
        public AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, HttpStatusCode statusCode)
        {
            var errorResponse = JsonErrorResponseUnmarshaller.Instance.UnmarshallResponse(context);
            errorResponse.StatusCode = statusCode;

            return new AmazonLambdaException(errorResponse.Message, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, errorResponse.StatusCode);
        }
    }
}