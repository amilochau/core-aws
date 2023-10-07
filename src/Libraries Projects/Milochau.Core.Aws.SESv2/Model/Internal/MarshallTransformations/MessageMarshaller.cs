using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Milochau.Core.Aws.SESv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Message Marshaller
    /// </summary>
    public class MessageMarshaller : IRequestMarshaller<Message> 
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public void Marshall(Message requestObject, JsonWriter writer)
        {
            if(requestObject.IsSetBody())
            {
                writer.WritePropertyName("Body");
                writer.WriteObjectStart();

                var marshaller = BodyMarshaller.Instance;
                marshaller.Marshall(requestObject.Body!, writer);

                writer.WriteObjectEnd();
            }

            if(requestObject.IsSetSubject())
            {
                writer.WritePropertyName("Subject");
                writer.WriteObjectStart();

                var marshaller = ContentMarshaller.Instance;
                marshaller.Marshall(requestObject.Subject!, writer);

                writer.WriteObjectEnd();
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static MessageMarshaller Instance = new MessageMarshaller();
    }
}