using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using ThirdParty.Json.LitJson;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// AttributeValue Marshaller
    /// </summary>
    public class AttributeValueMarshaller : IRequestMarshaller<AttributeValue>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public void Marshall(AttributeValue requestObject, JsonWriter writer)
        {
            if (requestObject.IsSetB())
            {
                writer.WritePropertyName("B");
                writer.Write(StringUtils.FromMemoryStream(requestObject.B));
            }

            if (requestObject.BOOL.HasValue)
            {
                writer.WritePropertyName("BOOL");
                writer.Write(requestObject.BOOL.Value);
            }

            if (requestObject.IsSetBS())
            {
                writer.WritePropertyName("BS");
                writer.WriteArrayStart();
                foreach (var requestObjectBSListValue in requestObject.BS!)
                {
                    writer.Write(StringUtils.FromMemoryStream(requestObjectBSListValue));
                }
                writer.WriteArrayEnd();
            }

            if (requestObject.IsSetL())
            {
                writer.WritePropertyName("L");
                writer.WriteArrayStart();
                foreach (var requestObjectLListValue in requestObject.L!)
                {
                    writer.WriteObjectStart();

                    var marshaller = AttributeValueMarshaller.Instance;
                    marshaller.Marshall(requestObjectLListValue, writer);

                    writer.WriteObjectEnd();
                }
                writer.WriteArrayEnd();
            }

            if (requestObject.IsSetM())
            {
                writer.WritePropertyName("M");
                writer.WriteObjectStart();
                foreach (var requestObjectMKvp in requestObject.M!)
                {
                    writer.WritePropertyName(requestObjectMKvp.Key);
                    var requestObjectMValue = requestObjectMKvp.Value;

                    writer.WriteObjectStart();

                    var marshaller = AttributeValueMarshaller.Instance;
                    marshaller.Marshall(requestObjectMValue, writer);

                    writer.WriteObjectEnd();
                }
                writer.WriteObjectEnd();
            }

            if (requestObject.IsSetN())
            {
                writer.WritePropertyName("N");
                writer.Write(requestObject.N);
            }

            if (requestObject.IsSetNS())
            {
                writer.WritePropertyName("NS");
                writer.WriteArrayStart();
                foreach (var requestObjectNSListValue in requestObject.NS!)
                {
                    writer.Write(requestObjectNSListValue);
                }
                writer.WriteArrayEnd();
            }

            if (requestObject.NULL.HasValue)
            {
                writer.WritePropertyName("NULL");
                writer.Write(requestObject.NULL.Value);
            }

            if (requestObject.IsSetS())
            {
                writer.WritePropertyName("S");
                writer.Write(requestObject.S);
            }

            if (requestObject.IsSetSS())
            {
                writer.WritePropertyName("SS");
                writer.WriteArrayStart();
                foreach (var requestObjectSSListValue in requestObject.SS!)
                {
                    writer.Write(requestObjectSSListValue);
                }
                writer.WriteArrayEnd();
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static AttributeValueMarshaller Instance = new AttributeValueMarshaller();
    }
}
