using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Milochau.Core.Aws.SESv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// ListManagementOptions Marshaller
    /// </summary>
    public class ListManagementOptionsMarshaller : IRequestMarshaller<ListManagementOptions> 
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public void Marshall(ListManagementOptions requestObject, JsonWriter writer)
        {
            if(requestObject.IsSetContactListName())
            {
                writer.WritePropertyName("ContactListName");
                writer.Write(requestObject.ContactListName);
            }

            if(requestObject.IsSetTopicName())
            {
                writer.WritePropertyName("TopicName");
                writer.Write(requestObject.TopicName);
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static ListManagementOptionsMarshaller Instance = new ListManagementOptionsMarshaller();
    }
}