using System;
using System.IO;
using ThirdParty.Ionic.Zlib;
using Milochau.Core.Aws.Core.Runtime.Internal.Util;
using System.Linq;
using System.Net.Http;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Transform
{
    /// <summary>
    /// Base class for the UnmarshallerContext objects that are used
    /// to unmarshall a web-service response.
    /// </summary>
    public abstract class UnmarshallerContext : IDisposable
    {
        private bool disposed = false;

        protected bool MaintainResponseBody { get; set; }
        protected bool IsException { get; set; }
        protected CrcCalculatorStream CrcStream { get; set; }
        protected int Crc32Result { get; set; }

        protected CachingWrapperStream WrappingStream { get; set; }

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
                return WrappingStream.AllReadBytes.ToArray();
            }
            else if (MaintainResponseBody)
            {
                return WrappingStream.LoggableReadBytes.ToArray();
            }
            else
            {
                return Array.Empty<byte>();
            }
        }

        public HttpResponseMessage ResponseData { get; protected set; }

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
