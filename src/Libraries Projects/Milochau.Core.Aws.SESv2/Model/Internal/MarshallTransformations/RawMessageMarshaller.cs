using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using ThirdParty.Json.LitJson;

namespace Milochau.Core.Aws.SESv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// RawMessage Marshaller
    /// </summary>
    public class RawMessageMarshaller : IRequestMarshaller<RawMessage> 
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public void Marshall(RawMessage requestObject, JsonWriter writer)
        {
            if(requestObject.IsSetData())
            {
                writer.WritePropertyName("Data");
                writer.Write(StringUtils.FromMemoryStream(requestObject.Data));
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static RawMessageMarshaller Instance = new RawMessageMarshaller();
    }
}