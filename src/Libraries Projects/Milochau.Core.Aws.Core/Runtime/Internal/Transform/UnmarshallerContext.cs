/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 * 
 *  http://aws.amazon.com/apache2.0
 * 
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

using Amazon.Util.Internal;
using Amazon.Runtime.Internal.Util;
using System;
using System.IO;
using ThirdParty.Ionic.Zlib;

namespace Amazon.Runtime.Internal.Transform
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
        protected IWebResponseData WebResponseData { get; set; }

        protected CachingWrapperStream WrappingStream { get; set; }

        public string ResponseBody
        {
            get
            {
                var bytes = GetResponseBodyBytes();
                return System.Text.UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }
        }

        public byte[] GetResponseBodyBytes()
        {
            if (IsException)
            {
                return this.WrappingStream.AllReadBytes.ToArray();
            }

            if (MaintainResponseBody)
            {
                return this.WrappingStream.LoggableReadBytes.ToArray();
            }
            else
            {
                return ArrayEx.Empty<byte>();
            }
        }

        public IWebResponseData ResponseData
        {
            get { return WebResponseData; }
        }

        internal void ValidateCRC32IfAvailable()
        {
            if (this.CrcStream != null)
            {
                if (this.CrcStream.Crc32 != this.Crc32Result)
                {
                    throw new IOException("CRC value returned with response does not match the computed CRC value for the returned response body.");
                }
            }
        }

        protected void SetupCRCStream(IWebResponseData responseData, Stream responseStream, long contentLength)
        {
            this.CrcStream = null;

            UInt32 parsed;
            if (responseData != null && UInt32.TryParse(responseData.GetHeaderValue("x-amz-crc32"), out parsed))
            {
                this.Crc32Result = unchecked((int) parsed);
                this.CrcStream = new CrcCalculatorStream(responseStream, contentLength);
            }
        }

        #region Abstract members

        /// <summary>
        /// The current path that is being unmarshalled.
        /// </summary>
        public abstract string CurrentPath { get; }

        #endregion

        #region Dispose Pattern Implementation

        /// <summary>
        /// Implements the Dispose pattern
        /// </summary>
        /// <param name="disposing">Whether this object is being disposed via a call to Dispose
        /// or garbage collected.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.CrcStream != null)
                    {
                        CrcStream.Dispose();
                        CrcStream = null;
                    }
                    if (this.WrappingStream != null)
                    {
                        WrappingStream.Dispose();
                        WrappingStream = null;
                    }
                }
                this.disposed = true;
            }
        }

        /// <summary>
        /// Disposes of all managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
