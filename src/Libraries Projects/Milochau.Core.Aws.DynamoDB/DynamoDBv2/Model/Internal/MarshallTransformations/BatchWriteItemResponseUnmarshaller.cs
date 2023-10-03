using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/BatchWriteItemResponseUnmarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for BatchWriteItem operation
    /// </summary>  
    public class BatchWriteItemResponseUnmarshaller : JsonResponseUnmarshaller
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="context"></param>
        /// <returns></returns>
        public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
        {
            BatchWriteItemResponse response = new BatchWriteItemResponse();

            context.Read();
            int targetDepth = context.CurrentDepth;
            while (context.ReadAtDepth(targetDepth))
            {
                if (context.TestExpression("ConsumedCapacity", targetDepth))
                {
                    var unmarshaller = new ListUnmarshaller<ConsumedCapacity, ConsumedCapacityUnmarshaller>(ConsumedCapacityUnmarshaller.Instance);
                    response.ConsumedCapacity = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("ItemCollectionMetrics", targetDepth))
                {
                    var unmarshaller = new DictionaryUnmarshaller<string, List<ItemCollectionMetrics>, StringUnmarshaller, ListUnmarshaller<ItemCollectionMetrics, ItemCollectionMetricsUnmarshaller>>(StringUnmarshaller.Instance, new ListUnmarshaller<ItemCollectionMetrics, ItemCollectionMetricsUnmarshaller>(ItemCollectionMetricsUnmarshaller.Instance));
                    response.ItemCollectionMetrics = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("UnprocessedItems", targetDepth))
                {
                    var unmarshaller = new DictionaryUnmarshaller<string, List<WriteRequest>, StringUnmarshaller, ListUnmarshaller<WriteRequest, WriteRequestUnmarshaller>>(StringUnmarshaller.Instance, new ListUnmarshaller<WriteRequest, WriteRequestUnmarshaller>(WriteRequestUnmarshaller.Instance));
                    response.UnprocessedItems = unmarshaller.Unmarshall(context);
                    continue;
                }
            }

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

            var responseBodyBytes = context.GetResponseBodyBytes();

            using (var streamCopy = new MemoryStream(responseBodyBytes))
            using (var contextCopy = new JsonUnmarshallerContext(streamCopy, false, null))
            {
                if (errorResponse.Code != null && errorResponse.Code.Equals("InternalServerError"))
                {
                    return InternalServerErrorExceptionUnmarshaller.Instance.Unmarshall(contextCopy, errorResponse);
                }
                if (errorResponse.Code != null && errorResponse.Code.Equals("ItemCollectionSizeLimitExceededException"))
                {
                    return ItemCollectionSizeLimitExceededExceptionUnmarshaller.Instance.Unmarshall(contextCopy, errorResponse);
                }
                if (errorResponse.Code != null && errorResponse.Code.Equals("ProvisionedThroughputExceededException"))
                {
                    return ProvisionedThroughputExceededExceptionUnmarshaller.Instance.Unmarshall(contextCopy, errorResponse);
                }
                if (errorResponse.Code != null && errorResponse.Code.Equals("RequestLimitExceeded"))
                {
                    return RequestLimitExceededExceptionUnmarshaller.Instance.Unmarshall(contextCopy, errorResponse);
                }
                if (errorResponse.Code != null && errorResponse.Code.Equals("ResourceNotFoundException"))
                {
                    return ResourceNotFoundExceptionUnmarshaller.Instance.Unmarshall(contextCopy, errorResponse);
                }
            }
            return new AmazonDynamoDBException(errorResponse.Message, errorResponse.InnerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, errorResponse.StatusCode);
        }

        private static BatchWriteItemResponseUnmarshaller _instance = new BatchWriteItemResponseUnmarshaller();

        internal static BatchWriteItemResponseUnmarshaller GetInstance()
        {
            return _instance;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static BatchWriteItemResponseUnmarshaller Instance
        {
            get
            {
                return _instance;
            }
        }

    }
}
