using System.IO;
using System.Linq;
using System.Net.Http;
using Milochau.Core.Aws.Core.Runtime.Internal.Util;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Transform
{
    /// <summary>
    /// Wraps a json string for unmarshalling.
    /// </summary>
    public class JsonUnmarshallerContext : UnmarshallerContext
    {
        /// <summary>
        /// Get the base stream of the jsonStream.
        /// </summary>
        public Stream Stream { get; }

        /// <summary>
        /// Wrap the jsonstring for unmarshalling.
        /// </summary>
        /// <param name="responseStream">Stream that contains the JSON for unmarshalling</param>
        /// <param name="maintainResponseBody"> If set to true, maintains a copy of the complete response body constraint to log response size as the stream is being read.</param>
        /// <param name="responseData">Response data coming back from the request</param>
        /// <param name="isException">If set to true, maintains a copy of the complete response body as the stream is being read.</param>
        public JsonUnmarshallerContext(
            Stream responseStream,
            bool maintainResponseBody,
            HttpResponseMessage responseData,
            bool isException)
        {
            if (isException || maintainResponseBody)
            {
                WrappingStream = new CachingWrapperStream(responseStream);
                responseStream = WrappingStream;
            }

            ResponseData = responseData;
            MaintainResponseBody = maintainResponseBody;
            IsException = isException;

            // Temporary work around checking Content-Encoding for an issue with NetStandard on Linux returning Content-Length for a gzipped response.
            // Causing the SDK to attempt a CRC check over the gzipped response data with a CRC value for the uncompressed value. 
            // The Content-Encoding check can be removed with the following github issue is shipped.
            // https://github.com/dotnet/corefx/issues/6796
            var contentLength = responseData.Content.Headers.ContentLength;
            if (contentLength.HasValue && string.IsNullOrEmpty(responseData.Content.Headers.ContentEncoding.FirstOrDefault()))
            {
                SetupCRCStream(responseData, responseStream, contentLength.Value);
            }

            Stream = CrcStream ?? responseStream;
        }
    }
}
