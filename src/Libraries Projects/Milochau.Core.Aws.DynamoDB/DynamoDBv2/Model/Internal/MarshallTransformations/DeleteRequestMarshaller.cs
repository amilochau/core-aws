using Amazon.Runtime.Internal.Transform;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/DeleteRequestMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// DeleteRequest Marshaller
    /// </summary>
    public class DeleteRequestMarshaller : IRequestMarshaller<DeleteRequest, JsonMarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="requestObject"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void Marshall(DeleteRequest requestObject, JsonMarshallerContext context)
        {
            if (requestObject.IsSetKey())
            {
                context.Writer.WritePropertyName("Key");
                context.Writer.WriteObjectStart();
                foreach (var requestObjectKeyKvp in requestObject.Key)
                {
                    context.Writer.WritePropertyName(requestObjectKeyKvp.Key);
                    var requestObjectKeyValue = requestObjectKeyKvp.Value;

                    context.Writer.WriteObjectStart();

                    var marshaller = AttributeValueMarshaller.Instance;
                    marshaller.Marshall(requestObjectKeyValue, context);

                    context.Writer.WriteObjectEnd();
                }
                context.Writer.WriteObjectEnd();
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static DeleteRequestMarshaller Instance = new DeleteRequestMarshaller();

    }
}
