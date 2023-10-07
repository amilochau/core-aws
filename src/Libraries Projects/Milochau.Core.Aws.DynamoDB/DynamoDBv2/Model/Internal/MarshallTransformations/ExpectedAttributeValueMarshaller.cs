using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/ExpectedAttributeValueMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// ExpectedAttributeValue Marshaller
    /// </summary>
    public class ExpectedAttributeValueMarshaller : IRequestMarshaller<ExpectedAttributeValue>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public void Marshall(ExpectedAttributeValue requestObject, JsonWriter writer)
        {
            if (requestObject.IsSetAttributeValueList())
            {
                writer.WritePropertyName("AttributeValueList");
                writer.WriteArrayStart();
                foreach (var requestObjectAttributeValueListListValue in requestObject.AttributeValueList)
                {
                    writer.WriteObjectStart();

                    var marshaller = AttributeValueMarshaller.Instance;
                    marshaller.Marshall(requestObjectAttributeValueListListValue, writer);

                    writer.WriteObjectEnd();
                }
                writer.WriteArrayEnd();
            }

            if (requestObject.IsSetComparisonOperator())
            {
                writer.WritePropertyName("ComparisonOperator");
                writer.Write(requestObject.ComparisonOperator!.Value);
            }

            if (requestObject.Exists.HasValue)
            {
                writer.WritePropertyName("Exists");
                writer.Write(requestObject.Exists.Value);
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
        public readonly static ExpectedAttributeValueMarshaller Instance = new ExpectedAttributeValueMarshaller();
    }
}
