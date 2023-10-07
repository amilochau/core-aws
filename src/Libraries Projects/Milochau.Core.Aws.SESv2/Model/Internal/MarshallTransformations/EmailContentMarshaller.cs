using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Milochau.Core.Aws.SESv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// EmailContent Marshaller
    /// </summary>
    public class EmailContentMarshaller : IRequestMarshaller<EmailContent> 
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public void Marshall(EmailContent requestObject, JsonWriter writer)
        {
            if(requestObject.IsSetRaw())
            {
                writer.WritePropertyName("Raw");
                writer.WriteObjectStart();

                var marshaller = RawMessageMarshaller.Instance;
                marshaller.Marshall(requestObject.Raw!, writer);

                writer.WriteObjectEnd();
            }

            if(requestObject.IsSetSimple())
            {
                writer.WritePropertyName("Simple");
                writer.WriteObjectStart();

                var marshaller = MessageMarshaller.Instance;
                marshaller.Marshall(requestObject.Simple!, writer);

                writer.WriteObjectEnd();
            }

            if(requestObject.IsSetTemplate())
            {
                writer.WritePropertyName("Template");
                writer.WriteObjectStart();

                var marshaller = TemplateMarshaller.Instance;
                marshaller.Marshall(requestObject.Template!, writer);

                writer.WriteObjectEnd();
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static EmailContentMarshaller Instance = new EmailContentMarshaller();
    }
}