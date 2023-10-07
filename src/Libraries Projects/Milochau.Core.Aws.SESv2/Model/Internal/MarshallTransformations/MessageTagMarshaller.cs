using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Milochau.Core.Aws.SESv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// MessageTag Marshaller
    /// </summary>
    public class MessageTagMarshaller : IRequestMarshaller<MessageTag> 
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public void Marshall(MessageTag requestObject, JsonWriter writer)
        {
            if(requestObject.IsSetName())
            {
                writer.WritePropertyName("Name");
                writer.Write(requestObject.Name);
            }

            if(requestObject.IsSetValue())
            {
                writer.WritePropertyName("Value");
                writer.Write(requestObject.Value);
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static MessageTagMarshaller Instance = new MessageTagMarshaller();
    }
}