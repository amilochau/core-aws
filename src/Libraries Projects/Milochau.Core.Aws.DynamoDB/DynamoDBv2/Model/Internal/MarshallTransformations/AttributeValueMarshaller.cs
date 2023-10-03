using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/AttributeValueMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// AttributeValue Marshaller
    /// </summary>
    public class AttributeValueMarshaller : IRequestMarshaller<AttributeValue, JsonMarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="requestObject"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void Marshall(AttributeValue requestObject, JsonMarshallerContext context)
        {
            if (requestObject.IsSetB())
            {
                context.Writer.WritePropertyName("B");
                context.Writer.Write(StringUtils.FromMemoryStream(requestObject.B));
            }

            if (requestObject.BOOL.HasValue)
            {
                context.Writer.WritePropertyName("BOOL");
                context.Writer.Write(requestObject.BOOL.Value);
            }

            if (requestObject.IsSetBS())
            {
                context.Writer.WritePropertyName("BS");
                context.Writer.WriteArrayStart();
                foreach (var requestObjectBSListValue in requestObject.BS!)
                {
                    context.Writer.Write(StringUtils.FromMemoryStream(requestObjectBSListValue));
                }
                context.Writer.WriteArrayEnd();
            }

            if (requestObject.IsSetL())
            {
                context.Writer.WritePropertyName("L");
                context.Writer.WriteArrayStart();
                foreach (var requestObjectLListValue in requestObject.L!)
                {
                    context.Writer.WriteObjectStart();

                    var marshaller = AttributeValueMarshaller.Instance;
                    marshaller.Marshall(requestObjectLListValue, context);

                    context.Writer.WriteObjectEnd();
                }
                context.Writer.WriteArrayEnd();
            }

            if (requestObject.IsSetM())
            {
                context.Writer.WritePropertyName("M");
                context.Writer.WriteObjectStart();
                foreach (var requestObjectMKvp in requestObject.M!)
                {
                    context.Writer.WritePropertyName(requestObjectMKvp.Key);
                    var requestObjectMValue = requestObjectMKvp.Value;

                    context.Writer.WriteObjectStart();

                    var marshaller = AttributeValueMarshaller.Instance;
                    marshaller.Marshall(requestObjectMValue, context);

                    context.Writer.WriteObjectEnd();
                }
                context.Writer.WriteObjectEnd();
            }

            if (requestObject.IsSetN())
            {
                context.Writer.WritePropertyName("N");
                context.Writer.Write(requestObject.N);
            }

            if (requestObject.IsSetNS())
            {
                context.Writer.WritePropertyName("NS");
                context.Writer.WriteArrayStart();
                foreach (var requestObjectNSListValue in requestObject.NS!)
                {
                    context.Writer.Write(requestObjectNSListValue);
                }
                context.Writer.WriteArrayEnd();
            }

            if (requestObject.NULL.HasValue)
            {
                context.Writer.WritePropertyName("NULL");
                context.Writer.Write(requestObject.NULL.Value);
            }

            if (requestObject.IsSetS())
            {
                context.Writer.WritePropertyName("S");
                context.Writer.Write(requestObject.S);
            }

            if (requestObject.IsSetSS())
            {
                context.Writer.WritePropertyName("SS");
                context.Writer.WriteArrayStart();
                foreach (var requestObjectSSListValue in requestObject.SS!)
                {
                    context.Writer.Write(requestObjectSSListValue);
                }
                context.Writer.WriteArrayEnd();
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static AttributeValueMarshaller Instance = new AttributeValueMarshaller();

    }
}
