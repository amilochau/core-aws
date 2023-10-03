using Amazon.Runtime.Internal.Transform;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/ItemCollectionSizeLimitExceededExceptionUnmarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for ItemCollectionSizeLimitExceededException Object
    /// </summary>  
    public class ItemCollectionSizeLimitExceededExceptionUnmarshaller : IErrorResponseUnmarshaller<ItemCollectionSizeLimitExceededException, JsonUnmarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="context"></param>
        /// <returns></returns>
        public ItemCollectionSizeLimitExceededException Unmarshall(JsonUnmarshallerContext context)
        {
            return this.Unmarshall(context, new Amazon.Runtime.Internal.ErrorResponse());
        }

        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="errorResponse"></param>
        /// <returns></returns>
        public ItemCollectionSizeLimitExceededException Unmarshall(JsonUnmarshallerContext context, Amazon.Runtime.Internal.ErrorResponse errorResponse)
        {
            context.Read();

            ItemCollectionSizeLimitExceededException unmarshalledObject = new ItemCollectionSizeLimitExceededException(errorResponse.Message, errorResponse.InnerException,
                errorResponse.Type, errorResponse.Code, errorResponse.RequestId, errorResponse.StatusCode);

            int targetDepth = context.CurrentDepth;
            while (context.ReadAtDepth(targetDepth))
            {
            }

            return unmarshalledObject;
        }

        private static ItemCollectionSizeLimitExceededExceptionUnmarshaller _instance = new ItemCollectionSizeLimitExceededExceptionUnmarshaller();

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static ItemCollectionSizeLimitExceededExceptionUnmarshaller Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
