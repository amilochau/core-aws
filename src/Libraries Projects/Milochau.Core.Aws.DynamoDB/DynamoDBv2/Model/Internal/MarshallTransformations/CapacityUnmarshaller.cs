using Amazon.Runtime.Internal.Transform;
using System;
using ThirdParty.Json.LitJson;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/CapacityUnmarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for Capacity Object
    /// </summary>  
    public class CapacityUnmarshaller : IUnmarshaller<Capacity, XmlUnmarshallerContext>, IUnmarshaller<Capacity, JsonUnmarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="context"></param>
        /// <returns></returns>
        Capacity IUnmarshaller<Capacity, XmlUnmarshallerContext>.Unmarshall(XmlUnmarshallerContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="context"></param>
        /// <returns></returns>
        public Capacity Unmarshall(JsonUnmarshallerContext context)
        {
            context.Read();
            if (context.CurrentTokenType == JsonToken.Null)
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.

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


        private static CapacityUnmarshaller _instance = new CapacityUnmarshaller();

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static CapacityUnmarshaller Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
