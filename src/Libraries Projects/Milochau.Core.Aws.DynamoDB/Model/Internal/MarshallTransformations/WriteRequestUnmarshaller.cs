using Amazon.Runtime.Internal.Transform;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for WriteRequest Object
    /// </summary>  
    public class WriteRequestUnmarshaller : IUnmarshaller<WriteRequest, JsonUnmarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public WriteRequest Unmarshall(JsonUnmarshallerContext context)
        {
            context.Read();

            WriteRequest unmarshalledObject = new WriteRequest();

            int targetDepth = context.CurrentDepth;
            while (context.ReadAtDepth(targetDepth))
            {
                if (context.TestExpression("DeleteRequest", targetDepth))
                {
                    var unmarshaller = DeleteRequestUnmarshaller.Instance;
                    unmarshalledObject.DeleteRequest = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("PutRequest", targetDepth))
                {
                    var unmarshaller = PutRequestUnmarshaller.Instance;
                    unmarshalledObject.PutRequest = unmarshaller.Unmarshall(context);
                    continue;
                }
            }

            return unmarshalledObject;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static WriteRequestUnmarshaller Instance { get; } = new WriteRequestUnmarshaller();
    }
}
