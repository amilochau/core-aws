using Amazon.Runtime.Internal.Transform;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/TransactionConflictExceptionUnmarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for TransactionConflictException Object
    /// </summary>  
    public class TransactionConflictExceptionUnmarshaller : IErrorResponseUnmarshaller<TransactionConflictException, JsonUnmarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="context"></param>
        /// <returns></returns>
        public TransactionConflictException Unmarshall(JsonUnmarshallerContext context)
        {
            return this.Unmarshall(context, new Amazon.Runtime.Internal.ErrorResponse());
        }

        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="errorResponse"></param>
        /// <returns></returns>
        public TransactionConflictException Unmarshall(JsonUnmarshallerContext context, Amazon.Runtime.Internal.ErrorResponse errorResponse)
        {
            context.Read();

            TransactionConflictException unmarshalledObject = new TransactionConflictException(errorResponse.Message, errorResponse.InnerException,
                errorResponse.Type, errorResponse.Code, errorResponse.RequestId, errorResponse.StatusCode);

            int targetDepth = context.CurrentDepth;
            while (context.ReadAtDepth(targetDepth))
            {
            }

            return unmarshalledObject;
        }

        private static TransactionConflictExceptionUnmarshaller _instance = new TransactionConflictExceptionUnmarshaller();

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static TransactionConflictExceptionUnmarshaller Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
