using Amazon.Runtime.Internal.Transform;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/WriteRequestMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// WriteRequest Marshaller
    /// </summary>
    public class WriteRequestMarshaller : IRequestMarshaller<WriteRequest, JsonMarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="requestObject"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void Marshall(WriteRequest requestObject, JsonMarshallerContext context)
        {
            if (requestObject.DeleteRequest != null)
            {
                context.Writer.WritePropertyName("DeleteRequest");
                context.Writer.WriteObjectStart();

                var marshaller = DeleteRequestMarshaller.Instance;
                marshaller.Marshall(requestObject.DeleteRequest, context);

                context.Writer.WriteObjectEnd();
            }

            if (requestObject.PutRequest != null)
            {
                context.Writer.WritePropertyName("PutRequest");
                context.Writer.WriteObjectStart();

                var marshaller = PutRequestMarshaller.Instance;
                marshaller.Marshall(requestObject.PutRequest, context);

                context.Writer.WriteObjectEnd();
            }
        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static WriteRequestMarshaller Instance = new WriteRequestMarshaller();
    }
}
