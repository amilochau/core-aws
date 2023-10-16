using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Util;
using System.IO;
using System.Linq;
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
        public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
        {
            InvokeResponse response = new InvokeResponse();

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
        public override AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, HttpStatusCode statusCode)
        {
            var errorResponse = JsonErrorResponseUnmarshaller.Instance.Unmarshall(context);
            errorResponse.StatusCode = statusCode;

            return new AmazonLambdaException(errorResponse.Message, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, errorResponse.StatusCode);
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static InvokeResponseUnmarshaller Instance { get; } = new InvokeResponseUnmarshaller();
    }
}