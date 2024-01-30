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
        /// <summary>Request marshaller</summary>
        public required IHttpRequestMessageMarshaller<AmazonWebServiceRequest> HttpRequestMessageMarshaller { get; set; }

        /// <summary>Response unmarshaller</summary>
        public required JsonResponseUnmarshaller ResponseUnmarshaller { get; set; }

        /// <summary>Original request name for monitoring</summary>
        /// <remarks>Should not end with "Request"</remarks>
        public required string MonitoringOriginalRequestName { get; set; }
    }
}