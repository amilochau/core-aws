using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/DeleteRequestMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// DeleteRequest Marshaller
    /// </summary>
    public class DeleteRequestMarshaller : IRequestMarshaller<DeleteRequest>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>
        /// <returns></returns>
        public void Marshall(DeleteRequest requestObject, JsonWriter writer)
        {
            if (requestObject.IsSetKey())
            {
                writer.WritePropertyName("Key");
                writer.WriteObjectStart();
                foreach (var requestObjectKeyKvp in requestObject.Key)
                {
                    writer.WritePropertyName(requestObjectKeyKvp.Key);
                    var requestObjectKeyValue = requestObjectKeyKvp.Value;

                    writer.WriteObjectStart();

                    var marshaller = AttributeValueMarshaller.Instance;
                    marshaller.Marshall(requestObjectKeyValue, writer);

                    writer.WriteObjectEnd();
                }
                writer.WriteObjectEnd();
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static DeleteRequestMarshaller Instance = new DeleteRequestMarshaller();

    }
}
