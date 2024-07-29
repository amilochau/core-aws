using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using System.Net;
using System.Net.Http;

namespace Milochau.Core.Aws.Core.Runtime.Internal
{
    /// <summary>Invoke options</summary>
    public interface IInvokeOptions
    {
        /// <summary>Origin request name for monitoring</summary>
        /// <remarks>Should not end with "Request"</remarks>
        string MonitoringOriginalRequestName { get; }

        /// <summary>Unmarshaller error response to exception</summary>
        AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, HttpStatusCode statusCode);
    }

    /// <summary>Invoke options</summary>
    public interface IInvokeOptions<TRequest, TResponse> : IInvokeOptions
        where TRequest : AmazonWebServiceRequest
        where TResponse : AmazonWebServiceResponse, new()
    {
        /// <summary>Creates an HTTP request message to call the service</summary>
        HttpRequestMessage MarshallRequest(TRequest publicRequest);

        /// <summary>Unmarshaller the response from the service to the response class</summary>
        TResponse UnmarshallResponse(JsonUnmarshallerContext context);
    }
}