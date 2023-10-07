using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Condition Marshaller
    /// </summary>
    public class ConditionMarshaller : IRequestMarshaller<Condition>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public void Marshall(Condition requestObject, JsonWriter writer)
        {
            if (requestObject.IsSetAttributeValueList())
            {
                writer.WritePropertyName("AttributeValueList");
                writer.WriteArrayStart();
                foreach (var requestObjectAttributeValueListListValue in requestObject.AttributeValueList!)
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

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static ConditionMarshaller Instance = new ConditionMarshaller();
    }
}
