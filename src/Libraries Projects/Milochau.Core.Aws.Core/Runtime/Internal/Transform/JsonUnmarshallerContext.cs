using System.IO;
using System.Linq;
using System.Net.Http;
using Milochau.Core.Aws.Core.Runtime.Internal.Util;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Transform
{
    /// <summary>
    /// Wraps a json string for unmarshalling.
    /// 
    /// Each <c>Read()</c> operation gets the next token.
    /// <c>TestExpression()</c> is used to match the current key-chain
    /// to an xpath expression. The general pattern looks like this:
    /// <code>
    /// JsonUnmarshallerContext context = new JsonUnmarshallerContext(jsonString);
    /// while (context.Read())
    /// {
    ///     if (context.IsKey)
    ///     {
    ///         if (context.TestExpresion("path/to/element"))
    ///         {
    ///             myObject.stringMember = stringUnmarshaller.GetInstance().Unmarshall(context);
    ///             continue;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </summary>
    public class JsonUnmarshallerContext : UnmarshallerContext
    {
        #region Private members

        private StreamReader streamReader = null;
        private bool disposed = false;

        #endregion

        #region Constructors

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

            var contentLength = responseData.Content.Headers.ContentLength;

            // Temporary work around checking Content-Encoding for an issue with NetStandard on Linux returning Content-Length for a gzipped response.
            // Causing the SDK to attempt a CRC check over the gzipped response data with a CRC value for the uncompressed value. 
            // The Content-Encoding check can be removed with the following github issue is shipped.
            // https://github.com/dotnet/corefx/issues/6796

            if (contentLength.HasValue && responseData.Content.Headers.ContentLength.Equals(contentLength) &&
                string.IsNullOrEmpty(responseData.Content.Headers.ContentEncoding.FirstOrDefault()))
            {
                SetupCRCStream(responseData, responseStream, contentLength.Value);
            }

            if (CrcStream != null)
                streamReader = new StreamReader(CrcStream);
            else
                streamReader = new StreamReader(responseStream);
        }

        #endregion

        #region Internal methods/properties

        /// <summary>
        /// Get the base stream of the jsonStream.
        /// </summary>
        public Stream Stream => streamReader.BaseStream;

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (streamReader != null)
                    {
                        streamReader.Dispose();
                        streamReader = null;
                    }
                }
                disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
