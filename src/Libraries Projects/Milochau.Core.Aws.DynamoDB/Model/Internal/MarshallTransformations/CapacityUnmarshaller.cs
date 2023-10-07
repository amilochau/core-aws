using Amazon.Runtime.Internal.Transform;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for Capacity Object
    /// </summary>  
    public class CapacityUnmarshaller : IUnmarshaller<Capacity, JsonUnmarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public Capacity Unmarshall(JsonUnmarshallerContext context)
        {
            context.Read();

            Capacity unmarshalledObject = new Capacity();

            int targetDepth = context.CurrentDepth;
            while (context.ReadAtDepth(targetDepth))
            {
                if (context.TestExpression("CapacityUnits", targetDepth))
                {
                    var unmarshaller = DoubleUnmarshaller.Instance;
                    unmarshalledObject.CapacityUnits = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("ReadCapacityUnits", targetDepth))
                {
                    var unmarshaller = DoubleUnmarshaller.Instance;
                    unmarshalledObject.ReadCapacityUnits = unmarshaller.Unmarshall(context);
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
        public static CapacityUnmarshaller Instance { get; } = new CapacityUnmarshaller();
    }
}
