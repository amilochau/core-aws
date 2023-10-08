using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime;
using System;
using System.IO;
using System.Net;
using System.Text.Json;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for UpdateItem operation
    /// </summary>  
    public class UpdateItemResponseUnmarshaller : JsonResponseUnmarshaller
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
        {
            return JsonSerializer.Deserialize(context.Stream, AwsJsonSerializerContext.Default.UpdateItemResponse)!; // @todo null?
        }

        /// <summary>
        /// Unmarshaller error response to exception.
        /// </summary>  
        /// <returns></returns>
        public override AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
        {
            var errorResponse = JsonErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
            errorResponse.InnerException = innerException;
            errorResponse.StatusCode = statusCode;

            return new AmazonDynamoDBException(errorResponse.Message, errorResponse.InnerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, errorResponse.StatusCode);
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static UpdateItemResponseUnmarshaller Instance { get; } = new UpdateItemResponseUnmarshaller();
    }
}
