using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/PutRequestMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// PutRequest Marshaller
    /// </summary>
    public class PutRequestMarshaller : IRequestMarshaller<PutRequest>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>
        /// <returns></returns>
        public void Marshall(PutRequest requestObject, JsonWriter writer)
        {
            if (requestObject.IsSetItem())
            {
                writer.WritePropertyName("Item");
                writer.WriteObjectStart();
                foreach (var requestObjectItemKvp in requestObject.Item)
                {
                    writer.WritePropertyName(requestObjectItemKvp.Key);
                    var requestObjectItemValue = requestObjectItemKvp.Value;

                    writer.WriteObjectStart();

                    var marshaller = AttributeValueMarshaller.Instance;
                    marshaller.Marshall(requestObjectItemValue, writer);

                    writer.WriteObjectEnd();
                }
                writer.WriteObjectEnd();
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static PutRequestMarshaller Instance = new PutRequestMarshaller();

    }
}
