using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Core/Amazon.Runtime/AmazonWebServiceResponse.cs
namespace Milochau.Core.Aws.Core.Runtime
{
    public class AmazonWebServiceResponse
    {
        /// <summary>
        /// Contains additional information about the request, such as the 
        /// Request Id.
        /// </summary>
        public ResponseMetadata ResponseMetadata { get; set; } = null!;

        /// <summary>
        /// Returns the content length of the HTTP response.
        /// </summary>
        public long ContentLength { get; set; }

        /// <summary>
        /// Returns the status code of the HTTP response.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
