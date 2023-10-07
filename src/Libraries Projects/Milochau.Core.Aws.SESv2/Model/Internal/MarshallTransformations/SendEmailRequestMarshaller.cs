using System.Globalization;
using System.IO;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using ThirdParty.Json.LitJson;

namespace Milochau.Core.Aws.SESv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// SendEmail Request Marshaller
    /// </summary>       
    public class SendEmailRequestMarshaller : IMarshaller<IRequest, SendEmailRequest> , IMarshaller<IRequest,AmazonWebServiceRequest>
    {
        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="input"></param>
        /// <returns></returns>
        public IRequest Marshall(AmazonWebServiceRequest input)
        {
            return this.Marshall((SendEmailRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="publicRequest"></param>
        /// <returns></returns>
        public IRequest Marshall(SendEmailRequest publicRequest)
        {
            IRequest request = new DefaultRequest(publicRequest, "Amazon.SimpleEmailV2");
            request.Headers["Content-Type"] = "application/json";
            request.Headers[Amazon.Util.HeaderKeys.XAmzApiVersion] = "2019-09-27";
            request.HttpMethod = "POST";

            request.ResourcePath = "/v2/email/outbound-emails";
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonWriter(stringWriter);
                writer.WriteObjectStart();
                if(publicRequest.IsSetConfigurationSetName())
                {
                    writer.WritePropertyName("ConfigurationSetName");
                    writer.Write(publicRequest.ConfigurationSetName);
                }

                if(publicRequest.IsSetContent())
                {
                    writer.WritePropertyName("Content");
                    writer.WriteObjectStart();

                    var marshaller = EmailContentMarshaller.Instance;
                    marshaller.Marshall(publicRequest.Content!, writer);

                    writer.WriteObjectEnd();
                }

                if(publicRequest.IsSetDestination())
                {
                    writer.WritePropertyName("Destination");
                    writer.WriteObjectStart();

                    var marshaller = DestinationMarshaller.Instance;
                    marshaller.Marshall(publicRequest.Destination!, writer);

                    writer.WriteObjectEnd();
                }

                if(publicRequest.IsSetEmailTags())
                {
                    writer.WritePropertyName("EmailTags");
                    writer.WriteArrayStart();
                    foreach(var publicRequestEmailTagsListValue in publicRequest.EmailTags!)
                    {
                        writer.WriteObjectStart();

                        var marshaller = MessageTagMarshaller.Instance;
                        marshaller.Marshall(publicRequestEmailTagsListValue, writer);

                        writer.WriteObjectEnd();
                    }
                    writer.WriteArrayEnd();
                }

                if(publicRequest.IsSetFeedbackForwardingEmailAddress())
                {
                    writer.WritePropertyName("FeedbackForwardingEmailAddress");
                    writer.Write(publicRequest.FeedbackForwardingEmailAddress);
                }

                if(publicRequest.IsSetFeedbackForwardingEmailAddressIdentityArn())
                {
                    writer.WritePropertyName("FeedbackForwardingEmailAddressIdentityArn");
                    writer.Write(publicRequest.FeedbackForwardingEmailAddressIdentityArn);
                }

                if(publicRequest.IsSetFromEmailAddress())
                {
                    writer.WritePropertyName("FromEmailAddress");
                    writer.Write(publicRequest.FromEmailAddress);
                }

                if(publicRequest.IsSetFromEmailAddressIdentityArn())
                {
                    writer.WritePropertyName("FromEmailAddressIdentityArn");
                    writer.Write(publicRequest.FromEmailAddressIdentityArn);
                }

                if(publicRequest.IsSetListManagementOptions())
                {
                    writer.WritePropertyName("ListManagementOptions");
                    writer.WriteObjectStart();

                    var marshaller = ListManagementOptionsMarshaller.Instance;
                    marshaller.Marshall(publicRequest.ListManagementOptions!, writer);

                    writer.WriteObjectEnd();
                }

                if(publicRequest.IsSetReplyToAddresses())
                {
                    writer.WritePropertyName("ReplyToAddresses");
                    writer.WriteArrayStart();
                    foreach(var publicRequestReplyToAddressesListValue in publicRequest.ReplyToAddresses!)
                    {
                            writer.Write(publicRequestReplyToAddressesListValue);
                    }
                    writer.WriteArrayEnd();
                }

                writer.WriteObjectEnd();
                string snippet = stringWriter.ToString();
                request.Content = System.Text.Encoding.UTF8.GetBytes(snippet);
            }


            return request;
        }

        internal static SendEmailRequestMarshaller GetInstance()
        {
            return Instance;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static SendEmailRequestMarshaller Instance { get; } = new SendEmailRequestMarshaller();
    }
}