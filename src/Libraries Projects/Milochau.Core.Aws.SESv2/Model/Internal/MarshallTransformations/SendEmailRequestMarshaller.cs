using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Util;
using System.Text.Json;

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
        /// <returns></returns>
        public IRequest Marshall(AmazonWebServiceRequest input)
        {
            return Marshall((SendEmailRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <returns></returns>
        public IRequest Marshall(SendEmailRequest publicRequest)
        {
            var serializedRequest = JsonSerializer.Serialize(publicRequest, AwsJsonSerializerContext.Default.SendEmailRequest);

            IRequest request = new DefaultRequest(publicRequest, "Amazon.SimpleEmailV2")
            {
                HttpMethod = "POST",
                ResourcePath = "/v2/email/outbound-emails",
                Content = System.Text.Encoding.UTF8.GetBytes(serializedRequest)
            };
            request.Headers["Content-Type"] = "application/json";
            request.Headers[HeaderKeys.XAmzApiVersion] = "2019-09-27";

            return request;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static SendEmailRequestMarshaller Instance { get; } = new SendEmailRequestMarshaller();
    }
}