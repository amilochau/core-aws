using Amazon.Runtime.Internal.Transform;
using System;
using ThirdParty.Json.LitJson;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/WriteRequestUnmarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for WriteRequest Object
    /// </summary>  
    public class WriteRequestUnmarshaller : IUnmarshaller<WriteRequest, JsonUnmarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="context"></param>
        /// <returns></returns>
        public WriteRequest Unmarshall(JsonUnmarshallerContext context)
        {
            context.Read();
            if (context.CurrentTokenType == JsonToken.Null)
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.

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


        private static WriteRequestUnmarshaller _instance = new WriteRequestUnmarshaller();

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static WriteRequestUnmarshaller Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
