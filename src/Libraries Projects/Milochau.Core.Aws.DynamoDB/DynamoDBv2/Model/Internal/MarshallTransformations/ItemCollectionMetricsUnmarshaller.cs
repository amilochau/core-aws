using Amazon.Runtime.Internal.Transform;
using System;
using ThirdParty.Json.LitJson;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/ItemCollectionMetricsUnmarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for ItemCollectionMetrics Object
    /// </summary>  
    public class ItemCollectionMetricsUnmarshaller : IUnmarshaller<ItemCollectionMetrics, JsonUnmarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="context"></param>
        /// <returns></returns>
        public ItemCollectionMetrics Unmarshall(JsonUnmarshallerContext context)
        {
            context.Read();
            if (context.CurrentTokenType == JsonToken.Null)
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.

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


        private static ItemCollectionMetricsUnmarshaller _instance = new ItemCollectionMetricsUnmarshaller();

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static ItemCollectionMetricsUnmarshaller Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
