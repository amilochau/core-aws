using Amazon.Runtime.Internal.Transform;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for ItemCollectionMetrics Object
    /// </summary>  
    public class ItemCollectionMetricsUnmarshaller : IUnmarshaller<ItemCollectionMetrics, JsonUnmarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public ItemCollectionMetrics Unmarshall(JsonUnmarshallerContext context)
        {
            context.Read();

            ItemCollectionMetrics unmarshalledObject = new ItemCollectionMetrics();

            int targetDepth = context.CurrentDepth;
            while (context.ReadAtDepth(targetDepth))
            {
                if (context.TestExpression("ItemCollectionKey", targetDepth))
                {
                    var unmarshaller = new DictionaryUnmarshaller<string, AttributeValue, StringUnmarshaller, AttributeValueUnmarshaller>(StringUnmarshaller.Instance, AttributeValueUnmarshaller.Instance);
                    unmarshalledObject.ItemCollectionKey = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("SizeEstimateRangeGB", targetDepth))
                {
                    var unmarshaller = new ListUnmarshaller<double, DoubleUnmarshaller>(DoubleUnmarshaller.Instance);
                    unmarshalledObject.SizeEstimateRangeGB = unmarshaller.Unmarshall(context);
                    continue;
                }
            }

            return unmarshalledObject;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static ItemCollectionMetricsUnmarshaller Instance { get; } = new ItemCollectionMetricsUnmarshaller();
    }
}
