using Amazon.Runtime.Internal.Transform;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for ConsumedCapacity Object
    /// </summary>  
    public class ConsumedCapacityUnmarshaller : IUnmarshaller<ConsumedCapacity, JsonUnmarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public ConsumedCapacity Unmarshall(JsonUnmarshallerContext context)
        {
            context.Read();

            ConsumedCapacity unmarshalledObject = new ConsumedCapacity();

            int targetDepth = context.CurrentDepth;
            while (context.ReadAtDepth(targetDepth))
            {
                if (context.TestExpression("CapacityUnits", targetDepth))
                {
                    var unmarshaller = DoubleUnmarshaller.Instance;
                    unmarshalledObject.CapacityUnits = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("GlobalSecondaryIndexes", targetDepth))
                {
                    var unmarshaller = new DictionaryUnmarshaller<string, Capacity, StringUnmarshaller, CapacityUnmarshaller>(StringUnmarshaller.Instance, CapacityUnmarshaller.Instance);
                    unmarshalledObject.GlobalSecondaryIndexes = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("LocalSecondaryIndexes", targetDepth))
                {
                    var unmarshaller = new DictionaryUnmarshaller<string, Capacity, StringUnmarshaller, CapacityUnmarshaller>(StringUnmarshaller.Instance, CapacityUnmarshaller.Instance);
                    unmarshalledObject.LocalSecondaryIndexes = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("ReadCapacityUnits", targetDepth))
                {
                    var unmarshaller = DoubleUnmarshaller.Instance;
                    unmarshalledObject.ReadCapacityUnits = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("Table", targetDepth))
                {
                    var unmarshaller = CapacityUnmarshaller.Instance;
                    unmarshalledObject.Table = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("TableName", targetDepth))
                {
                    var unmarshaller = StringUnmarshaller.Instance;
                    unmarshalledObject.TableName = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("WriteCapacityUnits", targetDepth))
                {
                    var unmarshaller = DoubleUnmarshaller.Instance;
                    unmarshalledObject.WriteCapacityUnits = unmarshaller.Unmarshall(context);
                    continue;
                }
            }

            return unmarshalledObject;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static ConsumedCapacityUnmarshaller Instance { get; } = new ConsumedCapacityUnmarshaller();
    }
}
