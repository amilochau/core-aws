using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/WriteRequestMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// WriteRequest Marshaller
    /// </summary>
    public class WriteRequestMarshaller : IRequestMarshaller<WriteRequest>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public void Marshall(WriteRequest requestObject, JsonWriter writer)
        {
            if (requestObject.DeleteRequest != null)
            {
                writer.WritePropertyName("DeleteRequest");
                writer.WriteObjectStart();

                var marshaller = DeleteRequestMarshaller.Instance;
                marshaller.Marshall(requestObject.DeleteRequest, writer);

                writer.WriteObjectEnd();
            }

            if (requestObject.PutRequest != null)
            {
                writer.WritePropertyName("PutRequest");
                writer.WriteObjectStart();

                var marshaller = PutRequestMarshaller.Instance;
                marshaller.Marshall(requestObject.PutRequest, writer);

                writer.WriteObjectEnd();
            }
        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static WriteRequestMarshaller Instance = new WriteRequestMarshaller();
    }
}
