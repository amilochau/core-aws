using System.IO;
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
            IWebResponseData responseData,
            bool isException = false)
        {
            if (isException || maintainResponseBody)
            {
                WrappingStream = new CachingWrapperStream(responseStream);
                responseStream = WrappingStream;
            }

            WebResponseData = responseData;
            MaintainResponseBody = maintainResponseBody;
            IsException = isException;

            //if the json unmarshaller context is being called internally without there being a http response then the response data would be null
            if(responseData != null)
            {

                bool parsedContentLengthHeader = long.TryParse(responseData.GetHeaderValue("Content-Length"), out long contentLength);

                // Temporary work around checking Content-Encoding for an issue with NetStandard on Linux returning Content-Length for a gzipped response.
                // Causing the SDK to attempt a CRC check over the gzipped response data with a CRC value for the uncompressed value. 
                // The Content-Encoding check can be removed with the following github issue is shipped.
                // https://github.com/dotnet/corefx/issues/6796

                if (parsedContentLengthHeader && responseData.ContentLength.Equals(contentLength) &&
                    string.IsNullOrEmpty(responseData.GetHeaderValue("Content-Encoding")))
                {
                    SetupCRCStream(responseData, responseStream, contentLength);
                }
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
