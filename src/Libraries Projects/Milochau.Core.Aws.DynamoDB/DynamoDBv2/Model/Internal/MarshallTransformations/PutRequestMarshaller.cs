using Amazon.Runtime.Internal.Transform;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/PutRequestMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// PutRequest Marshaller
    /// </summary>
    public class PutRequestMarshaller : IRequestMarshaller<PutRequest, JsonMarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="requestObject"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void Marshall(PutRequest requestObject, JsonMarshallerContext context)
        {
            if (requestObject.IsSetItem())
            {
                context.Writer.WritePropertyName("Item");
                context.Writer.WriteObjectStart();
                foreach (var requestObjectItemKvp in requestObject.Item)
                {
                    context.Writer.WritePropertyName(requestObjectItemKvp.Key);
                    var requestObjectItemValue = requestObjectItemKvp.Value;

                    context.Writer.WriteObjectStart();

                    var marshaller = AttributeValueMarshaller.Instance;
                    marshaller.Marshall(requestObjectItemValue, context);

                    context.Writer.WriteObjectEnd();
                }
                context.Writer.WriteObjectEnd();
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static PutRequestMarshaller Instance = new PutRequestMarshaller();

    }
}
