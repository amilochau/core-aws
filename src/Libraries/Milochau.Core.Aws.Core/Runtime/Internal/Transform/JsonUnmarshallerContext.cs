using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Milochau.Core.Aws.Core.Runtime.Internal.Util;
using ThirdParty.Ionic.Zlib;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Transform
{
    /// <summary>
    /// Wraps a json string for unmarshalling.
    /// </summary>
    public class JsonUnmarshallerContext : IDisposable
    {
        private bool disposed = false;

        protected bool MaintainResponseBody { get; set; }
        protected bool IsException { get; set; }
        protected CrcCalculatorStream? CrcStream { get; set; }
        protected int Crc32Result { get; set; }

        protected CachingWrapperStream? WrappingStream { get; set; }

        public string ResponseBody
        {
            get
            {
                var bytes = GetResponseBodyBytes();
                return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }
        }

        public byte[] GetResponseBodyBytes()
        {
            if (IsException)
            {
                return WrappingStream!.AllReadBytes.ToArray();
            }
            else if (MaintainResponseBody)
            {
                return WrappingStream!.LoggableReadBytes.ToArray();
            }
            else
            {
                return [];
            }
        }

        public HttpResponseMessage ResponseData { get; protected set; }

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
        public JsonUnmarshallerContext(Stream responseStream, bool maintainResponseBody, HttpResponseMessage responseData, bool isException)
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
        internal void ValidateCRC32IfAvailable()
        {
            if (CrcStream != null)
            {
                if (CrcStream.Crc32 != Crc32Result)
                {
                    throw new IOException("CRC value returned with response does not match the computed CRC value for the returned response body.");
                }
            }
        }

        protected void SetupCRCStream(HttpResponseMessage responseData, Stream responseStream, long contentLength)
        {
            CrcStream = null;

            if (responseData != null
                && responseData.Headers.TryGetValues("x-amz-crc32", out var values)
                && uint.TryParse(values.FirstOrDefault(), out uint parsed))
            {
                Crc32Result = unchecked((int)parsed);
                CrcStream = new CrcCalculatorStream(responseStream, contentLength);
            }
        }

        #region Dispose Pattern Implementation

        /// <summary>
        /// Implements the Dispose pattern
        /// </summary>
        /// <param name="disposing">Whether this object is being disposed via a call to Dispose
        /// or garbage collected.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (CrcStream != null)
                    {
                        CrcStream.Dispose();
                        CrcStream = null;
                    }
                    if (WrappingStream != null)
                    {
                        WrappingStream.Dispose();
                        WrappingStream = null;
                    }
                }
                disposed = true;
            }
        }

        /// <summary>
        /// Disposes of all managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
