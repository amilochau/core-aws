using Amazon.Runtime.Internal.Transform;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for DeleteRequest Object
    /// </summary>  
    public class DeleteRequestUnmarshaller : IUnmarshaller<DeleteRequest, JsonUnmarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public DeleteRequest Unmarshall(JsonUnmarshallerContext context)
        {
            context.Read();

            DeleteRequest unmarshalledObject = new DeleteRequest();

            int targetDepth = context.CurrentDepth;
            while (context.ReadAtDepth(targetDepth))
            {
                if (context.TestExpression("Key", targetDepth))
                {
                    var unmarshaller = new DictionaryUnmarshaller<string, AttributeValue, StringUnmarshaller, AttributeValueUnmarshaller>(StringUnmarshaller.Instance, AttributeValueUnmarshaller.Instance);
                    unmarshalledObject.Key = unmarshaller.Unmarshall(context);
                    continue;
                }
            }

            return unmarshalledObject;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static DeleteRequestUnmarshaller Instance { get; } = new DeleteRequestUnmarshaller();
    }
}
