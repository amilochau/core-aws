using Milochau.Core.Aws.Core.Runtime.Internal.Transform;

namespace Milochau.Core.Aws.Core.Runtime.Internal
{
    /// <summary>
    /// Class containing the members used to invoke service calls
    /// <para>
    /// This class is only intended for internal use inside the AWS client libraries.
    /// Callers shouldn't ever interact directly with objects of this class.
    /// </para>
    /// </summary>
    public class InvokeOptions
    {
        public IHttpRequestMessageMarshaller<AmazonWebServiceRequest> HttpRequestMessageMarshaller { get; set; }

        public JsonResponseUnmarshaller ResponseUnmarshaller { get; set; }
    }
}