using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Util;
using System;
using System.IO;
using System.Net;

namespace Milochau.Core.Aws.Lambda.Model.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for Invoke operation
    /// </summary>  
    public class InvokeResponseUnmarshaller : JsonResponseUnmarshaller
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="context"></param>
        /// <returns></returns>
        public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
        {
            InvokeResponse response = new InvokeResponse();

            var ms = new MemoryStream();
            AWSSDKUtils.CopyStream(context.Stream, ms);
            ms.Seek(0, SeekOrigin.Begin);
            response.Payload = ms;
            if (context.ResponseData.IsHeaderPresent("X-Amz-Executed-Version"))
                response.ExecutedVersion = context.ResponseData.GetHeaderValue("X-Amz-Executed-Version");
            if (context.ResponseData.IsHeaderPresent("X-Amz-Function-Error"))
                response.FunctionError = context.ResponseData.GetHeaderValue("X-Amz-Function-Error");
            if (context.ResponseData.IsHeaderPresent("X-Amz-Log-Result"))
                response.LogResult = context.ResponseData.GetHeaderValue("X-Amz-Log-Result");
            response.StatusCode = (int)context.ResponseData.StatusCode;

            return response;
        }

        /// <summary>
        /// Unmarshaller error response to exception.
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="innerException"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public override AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
        {
            var errorResponse = JsonErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
            errorResponse.InnerException = innerException;
            errorResponse.StatusCode = statusCode;

            return new AmazonLambdaException(errorResponse.Message, errorResponse.InnerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, errorResponse.StatusCode);
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static InvokeResponseUnmarshaller Instance { get; } = new InvokeResponseUnmarshaller();
    }
}