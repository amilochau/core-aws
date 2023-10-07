using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Milochau.Core.Aws.SESv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Destination Marshaller
    /// </summary>
    public class DestinationMarshaller : IRequestMarshaller<Destination> 
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public void Marshall(Destination requestObject, JsonWriter writer)
        {
            if(requestObject.IsSetBccAddresses())
            {
                writer.WritePropertyName("BccAddresses");
                writer.WriteArrayStart();
                foreach(var requestObjectBccAddressesListValue in requestObject.BccAddresses!)
                {
                        writer.Write(requestObjectBccAddressesListValue);
                }
                writer.WriteArrayEnd();
            }

            if(requestObject.IsSetCcAddresses())
            {
                writer.WritePropertyName("CcAddresses");
                writer.WriteArrayStart();
                foreach(var requestObjectCcAddressesListValue in requestObject.CcAddresses!)
                {
                        writer.Write(requestObjectCcAddressesListValue);
                }
                writer.WriteArrayEnd();
            }

            if(requestObject.IsSetToAddresses())
            {
                writer.WritePropertyName("ToAddresses");
                writer.WriteArrayStart();
                foreach(var requestObjectToAddressesListValue in requestObject.ToAddresses!)
                {
                        writer.Write(requestObjectToAddressesListValue);
                }
                writer.WriteArrayEnd();
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static DestinationMarshaller Instance = new DestinationMarshaller();
    }
}