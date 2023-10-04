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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.Runtime.Internal.Util
{
    internal class EventStream : WrapperStream
    {
        internal delegate void ReadProgress(int bytesRead);
        internal event ReadProgress OnRead;
        bool disableClose;

        //Stream wrappedStream;

        internal EventStream(Stream stream, bool disableClose)
            : base(stream)
        {
            //this.wrappedStream = stream;
            this.disableClose = disableClose;
        }

        protected override void Dispose(bool disposing)
        {
        }

        public override bool CanRead
        {
            get { return BaseStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return BaseStream.CanSeek; }
        }

        public override bool CanTimeout
        {
            get { return BaseStream.CanTimeout; }
        }

        public override bool CanWrite
        {
            get { return BaseStream.CanWrite; }
        }

        public override long Length
        {
            get { return BaseStream.Length; }
        }

        public override long Position
        {
            get { return BaseStream.Position; }
            set { BaseStream.Position = value; }
        }

        public override int ReadTimeout
        {
            get { return BaseStream.ReadTimeout; }
            set { BaseStream.ReadTimeout = value; }
        }

        public override int WriteTimeout
        {
            get { return BaseStream.WriteTimeout; }
            set { BaseStream.WriteTimeout = value; }
        }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = BaseStream.Read(buffer, offset, count);
            if (this.OnRead != null)
            {
                this.OnRead(bytesRead);
            }

            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void WriteByte(byte value)
        {
            throw new NotImplementedException();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return BaseStream.FlushAsync(cancellationToken);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int bytesRead = await BaseStream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
            if (this.OnRead != null)
            {
                this.OnRead(bytesRead);
            }

            return bytesRead;
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
