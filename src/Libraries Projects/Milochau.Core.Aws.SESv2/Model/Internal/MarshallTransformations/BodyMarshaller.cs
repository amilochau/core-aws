using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Milochau.Core.Aws.SESv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Body Marshaller
    /// </summary>
    public class BodyMarshaller : IRequestMarshaller<Body> 
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public void Marshall(Body requestObject, JsonWriter writer)
        {
            if(requestObject.IsSetHtml())
            {
                writer.WritePropertyName("Html");
                writer.WriteObjectStart();

                var marshaller = ContentMarshaller.Instance;
                marshaller.Marshall(requestObject.Html!, writer);

                writer.WriteObjectEnd();
            }

            if(requestObject.IsSetText())
            {
                writer.WritePropertyName("Text");
                writer.WriteObjectStart();

                var marshaller = ContentMarshaller.Instance;
                marshaller.Marshall(requestObject.Text!, writer);

                writer.WriteObjectEnd();
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static BodyMarshaller Instance = new BodyMarshaller();
    }
}