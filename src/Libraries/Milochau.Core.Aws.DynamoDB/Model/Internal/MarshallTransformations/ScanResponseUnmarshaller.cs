using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Runtime.Internal;
using System.Text.Json;
using Milochau.Core.Aws.Core.Runtime;
using System.Net;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for Scan operation
    /// </summary>  
    public class ScanResponseUnmarshaller : JsonResponseUnmarshaller
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
        {
            return JsonSerializer.Deserialize(context.Stream, ScanJsonSerializerContext.Default.ScanResponse)!; // @todo null?
        }

        /// <summary>
        /// Unmarshaller error response to exception.
        /// </summary>  
        public override AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, HttpStatusCode statusCode)
        {
            var errorResponse = JsonErrorResponseUnmarshaller.Instance.Unmarshall(context);
            errorResponse.StatusCode = statusCode;

            return new AmazonDynamoDBException(errorResponse.Message, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, errorResponse.StatusCode);
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static ScanResponseUnmarshaller Instance { get; } = new ScanResponseUnmarshaller();
    }
}
