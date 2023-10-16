using System;
using System.Net;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Transform
{
    /// <summary>
    /// Interface for unmarshallers which unmarshall service responses.
    /// The Unmarshallers are stateless, and only encode the rules for what data 
    /// in the XML stream goes into what members of an object. 
    /// </summary>
    /// <typeparam name="T">The type of object the unmarshaller returns</typeparam>
    /// <typeparam name="R">The type of the XML unmashaller context, which contains the
    /// state of parsing the XML stream. Uaually an instance of 
    /// <c>Amazon.Runtime.Internal.Transform.UnmarshallerContext</c>.</typeparam>
    public interface IResponseUnmarshaller<T, R> : IUnmarshaller<T, R>
    {
        /// <summary>
        /// Extracts an exeption with data from an ErrorResponse.
        /// </summary>
        /// <param name="input">The XML parsing context.</param>
        /// <param name="statusCode">The HttpStatusCode from the ErrorResponse</param>
        /// <returns>Either an exception based on the ErrorCode from the ErrorResponse, or the 
        /// general service exception for the service in question.</returns>
        AmazonServiceException UnmarshallException(R input, HttpStatusCode statusCode);
    }
}
