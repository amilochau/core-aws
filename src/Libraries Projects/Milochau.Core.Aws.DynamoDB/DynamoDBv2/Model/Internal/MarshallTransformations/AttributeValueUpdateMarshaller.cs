using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/AttributeValueUpdateMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// AttributeValueUpdate Marshaller
    /// </summary>
    public class AttributeValueUpdateMarshaller : IRequestMarshaller<AttributeValueUpdate>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public void Marshall(AttributeValueUpdate requestObject, JsonWriter writer)
        {
            if (requestObject.IsSetAction())
            {
                writer.WritePropertyName("Action");
                writer.Write(requestObject.Action!.Value);
            }

            if (requestObject.IsSetValue())
            {
                writer.WritePropertyName("Value");
                writer.WriteObjectStart();

                var marshaller = AttributeValueMarshaller.Instance;
                marshaller.Marshall(requestObject.Value!, writer);

                writer.WriteObjectEnd();
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static AttributeValueUpdateMarshaller Instance = new AttributeValueUpdateMarshaller();
    }
}
