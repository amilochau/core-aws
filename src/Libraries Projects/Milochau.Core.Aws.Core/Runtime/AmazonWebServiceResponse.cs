using System.Net;

namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// Abstract class for Response objects, contains only metadata, 
    /// and no result information.
    /// </summary>
    public class AmazonWebServiceResponse
    {
        private ResponseMetadata responseMetadataField;
        private long contentLength;
        private HttpStatusCode httpStatusCode;

        /// <summary>
        /// Contains additional information about the request, such as the 
        /// Request Id.
        /// </summary>
        public ResponseMetadata ResponseMetadata
        {
            get { return  responseMetadataField; }
            set { responseMetadataField = value; }
        }

        /// <summary>
        /// Returns the content length of the HTTP response.
        /// </summary>
        public long ContentLength
        {
            get { return this.contentLength; }
            set { this.contentLength = value; }
        }

        /// <summary>
        /// Returns the status code of the HTTP response.
        /// </summary>
        public HttpStatusCode HttpStatusCode
        {
            get { return this.httpStatusCode; }
            set { this.httpStatusCode = value; }
        }
    }
}
