using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Util;
using System.Globalization;
using System.IO;

namespace Milochau.Core.Aws.Lambda.Model.MarshallTransformations
{
    /// <summary>
    /// Invoke Request Marshaller
    /// </summary>       
    public class InvokeRequestMarshaller : IMarshaller<IRequest, InvokeRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
    {
        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="input"></param>
        /// <returns></returns>
        public IRequest Marshall(AmazonWebServiceRequest input)
        {
            return this.Marshall((InvokeRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <returns></returns>
        public IRequest Marshall(InvokeRequest publicRequest)
        {
            IRequest request = new DefaultRequest(publicRequest, "Amazon.Lambda")
            {
                HttpMethod = "POST"
            };
            request.Headers["Content-Type"] = "application/json";
            request.Headers[HeaderKeys.XAmzApiVersion] = "2015-03-31";

            if (publicRequest.FunctionName == null)
                throw new AmazonLambdaException("Request object does not have required field FunctionName set");
            request.AddPathResource("{FunctionName}", publicRequest.FunctionName);

            request.ResourcePath = "/2015-03-31/functions/{FunctionName}/invocations";

            request.ContentStream = publicRequest.PayloadStream ?? new MemoryStream();
            if (request.ContentStream.CanSeek)
            {
                request.ContentStream.Seek(0, SeekOrigin.Begin);
            }
            request.Headers[HeaderKeys.ContentLengthHeader] =
                request.ContentStream.Length.ToString(CultureInfo.InvariantCulture);
            request.Headers[HeaderKeys.ContentTypeHeader] = "binary/octet-stream";
            if (request.ContentStream != null && request.ContentStream.Length == 0)
            {
                request.Headers.Remove(HeaderKeys.ContentTypeHeader);
            }

            if (publicRequest.ClientContextBase64 != null)
            {
                request.Headers["X-Amz-Client-Context"] = publicRequest.ClientContextBase64;
            }

            if (publicRequest.InvocationType != null)
            {
                request.Headers["X-Amz-Invocation-Type"] = publicRequest.InvocationType.Value;
            }

            request.UseQueryString = true;

            return request;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static InvokeRequestMarshaller Instance { get; } = new InvokeRequestMarshaller();
    }
}