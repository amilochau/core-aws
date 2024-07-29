using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using System;
using System.Net;
using System.Net.Http;

namespace Milochau.Core.Aws.Core.Runtime.Internal
{
    /// <summary>
    /// Class containing the members used to invoke service calls
    /// <para>
    /// This class is only intended for internal use inside the AWS client libraries.
    /// Callers shouldn't ever interact directly with objects of this class.
    /// </para>
    /// </summary>
    public class InvokeOptions<TRequest, TResponse>
        where TRequest: AmazonWebServiceRequest
        where TResponse: AmazonWebServiceResponse
    {
        /// <summary>Request marshaller</summary>
        public required Func<TRequest, HttpRequestMessage> RequestMarshaller { get; set; }

        /// <summary>Response unmarshaller</summary>
        public required Func<JsonUnmarshallerContext, TResponse> ResponseUnmarshaller { get; set; }

        /// <summary>RespoErrornse unmarshaller</summary>
        public required Func<JsonUnmarshallerContext, HttpStatusCode, AmazonServiceException> ExceptionUnmarshaller { get; set; }

        /// <summary>Original request name for monitoring</summary>
        /// <remarks>Should not end with "Request"</remarks>
        public required string MonitoringOriginalRequestName { get; set; }
    }
}