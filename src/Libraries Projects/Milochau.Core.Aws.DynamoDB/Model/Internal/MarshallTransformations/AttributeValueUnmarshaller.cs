using Amazon.Runtime.Internal.Transform;
using System.IO;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for AttributeValue Object
    /// </summary>  
    public class AttributeValueUnmarshaller : IUnmarshaller<AttributeValue, JsonUnmarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="context"></param>
        /// <returns></returns>
        public AttributeValue Unmarshall(JsonUnmarshallerContext context)
        {
            context.Read();

            AttributeValue unmarshalledObject = new AttributeValue();

            int targetDepth = context.CurrentDepth;
            while (context.ReadAtDepth(targetDepth))
            {
                if (context.TestExpression("B", targetDepth))
                {
                    var unmarshaller = MemoryStreamUnmarshaller.Instance;
                    unmarshalledObject.B = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("BOOL", targetDepth))
                {
                    var unmarshaller = BoolUnmarshaller.Instance;
                    unmarshalledObject.BOOL = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("BS", targetDepth))
                {
                    var unmarshaller = new ListUnmarshaller<MemoryStream, MemoryStreamUnmarshaller>(MemoryStreamUnmarshaller.Instance);
                    unmarshalledObject.BS = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("L", targetDepth))
                {
                    var unmarshaller = new ListUnmarshaller<AttributeValue, AttributeValueUnmarshaller>(AttributeValueUnmarshaller.Instance);
                    unmarshalledObject.L = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("M", targetDepth))
                {
                    var unmarshaller = new DictionaryUnmarshaller<string, AttributeValue, StringUnmarshaller, AttributeValueUnmarshaller>(StringUnmarshaller.Instance, AttributeValueUnmarshaller.Instance);
                    unmarshalledObject.M = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("N", targetDepth))
                {
                    var unmarshaller = StringUnmarshaller.Instance;
                    unmarshalledObject.N = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("NS", targetDepth))
                {
                    var unmarshaller = new ListUnmarshaller<string, StringUnmarshaller>(StringUnmarshaller.Instance);
                    unmarshalledObject.NS = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("NULL", targetDepth))
                {
                    var unmarshaller = BoolUnmarshaller.Instance;
                    unmarshalledObject.NULL = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("S", targetDepth))
                {
                    var unmarshaller = StringUnmarshaller.Instance;
                    unmarshalledObject.S = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("SS", targetDepth))
                {
                    var unmarshaller = new ListUnmarshaller<string, StringUnmarshaller>(StringUnmarshaller.Instance);
                    unmarshalledObject.SS = unmarshaller.Unmarshall(context);
                    continue;
                }
            }

            return unmarshalledObject;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static AttributeValueUnmarshaller Instance { get; } = new AttributeValueUnmarshaller();
    }
}
