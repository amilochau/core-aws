using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Milochau.Core.Aws.SESv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Content Marshaller
    /// </summary>
    public class ContentMarshaller : IRequestMarshaller<Content> 
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public void Marshall(Content requestObject, JsonWriter writer)
        {
            if(requestObject.IsSetCharset())
            {
                writer.WritePropertyName("Charset");
                writer.Write(requestObject.Charset);
            }

            if(requestObject.IsSetData())
            {
                writer.WritePropertyName("Data");
                writer.Write(requestObject.Data);
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static ContentMarshaller Instance = new ContentMarshaller();
    }
}