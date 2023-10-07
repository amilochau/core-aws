using Amazon.Runtime.Internal.Transform;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/ExpectedAttributeValueMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// ExpectedAttributeValue Marshaller
    /// </summary>
    public class ExpectedAttributeValueMarshaller : IRequestMarshaller<ExpectedAttributeValue, JsonMarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="requestObject"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void Marshall(ExpectedAttributeValue requestObject, JsonMarshallerContext context)
        {
            if (requestObject.IsSetAttributeValueList())
            {
                context.Writer.WritePropertyName("AttributeValueList");
                context.Writer.WriteArrayStart();
                foreach (var requestObjectAttributeValueListListValue in requestObject.AttributeValueList)
                {
                    context.Writer.WriteObjectStart();

                    var marshaller = AttributeValueMarshaller.Instance;
                    marshaller.Marshall(requestObjectAttributeValueListListValue, context);

                    context.Writer.WriteObjectEnd();
                }
                context.Writer.WriteArrayEnd();
            }

            if (requestObject.IsSetComparisonOperator())
            {
                context.Writer.WritePropertyName("ComparisonOperator");
                context.Writer.Write(requestObject.ComparisonOperator!.Value);
            }

            if (requestObject.Exists.HasValue)
            {
                context.Writer.WritePropertyName("Exists");
                context.Writer.Write(requestObject.Exists.Value);
            }

            if (requestObject.IsSetValue())
            {
                context.Writer.WritePropertyName("Value");
                context.Writer.WriteObjectStart();

                var marshaller = AttributeValueMarshaller.Instance;
                marshaller.Marshall(requestObject.Value!, context);

                context.Writer.WriteObjectEnd();
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static ExpectedAttributeValueMarshaller Instance = new ExpectedAttributeValueMarshaller();
    }
}
