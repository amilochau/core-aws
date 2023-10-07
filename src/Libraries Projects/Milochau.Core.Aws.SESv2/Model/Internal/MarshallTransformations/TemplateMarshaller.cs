using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Milochau.Core.Aws.SESv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Template Marshaller
    /// </summary>
    public class TemplateMarshaller : IRequestMarshaller<Template> 
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public void Marshall(Template requestObject, JsonWriter writer)
        {
            if(requestObject.IsSetTemplateArn())
            {
                writer.WritePropertyName("TemplateArn");
                writer.Write(requestObject.TemplateArn);
            }

            if(requestObject.IsSetTemplateData())
            {
                writer.WritePropertyName("TemplateData");
                writer.Write(requestObject.TemplateData);
            }

            if(requestObject.IsSetTemplateName())
            {
                writer.WritePropertyName("TemplateName");
                writer.Write(requestObject.TemplateName);
            }

        }

        /// <summary>
        /// Singleton Marshaller.
        /// </summary>
        public readonly static TemplateMarshaller Instance = new TemplateMarshaller();
    }
}