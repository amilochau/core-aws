﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Util
{
    /// <summary>
    /// This class is used to wrap a stream for a particular segment of a stream.  It 
    /// makes that segment look like you are reading from beginning to end of the stream.
    /// </summary>
    public class PartialWrapperStream : WrapperStream
    {
        private readonly long initialPosition;
        private readonly long partSize;

        public PartialWrapperStream(Stream stream, long partSize)
            : base(stream)
        {
            if (!stream.CanSeek)
                throw new InvalidOperationException("Base stream of PartialWrapperStream must be seekable");

            initialPosition = stream.Position;
            long remainingData = stream.Length - stream.Position;
            if (partSize == 0 || remainingData < partSize)
            {
                this.partSize = remainingData;
            }
            else
            {
                this.partSize = partSize;
                if (BaseStream is AESEncryptionUploadPartStream && (partSize % 16) != 0)
                {
                    this.partSize = partSize - (partSize % EncryptUploadPartStream.InternalEncryptionBlockSize);
                }
            }
        }

        private long RemainingPartSize
        {
            get
            {
                long remaining = partSize - Position;
                return remaining;
            }
        }

        #region Stream overrides

        public override long Length
        {
            get
            {
                long length = base.Length - initialPosition;
                if (length > partSize)
                {
                    length = partSize;
                }
                return length;
            }
        }

        public override long Position
        {
            get
            {
                return base.Position - initialPosition;
            }
            set
            {
                base.Position = initialPosition + value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesToRead = count < RemainingPartSize ? count : (int)RemainingPartSize;
            if (bytesToRead <= 0)
                return 0;
            return base.Read(buffer, offset, bytesToRead);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long position = 0;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    position = initialPosition + offset;
                    break;
                case SeekOrigin.Current:
                    position = base.Position + offset;
                    break;
                case SeekOrigin.End:
                    position = base.Position + partSize + offset;
                    break;
            }

            if (position < initialPosition)
            {
                position = initialPosition;
            }
            else if (position > initialPosition + partSize)
            {
                position = initialPosition + partSize;
            }

            base.Position = position;

            return Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Asynchronously reads a sequence of bytes from the current stream, advances
        /// the position within the stream by the number of bytes read, and monitors
        /// cancellation requests.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains the specified
        /// byte array with the values between offset and (offset + count - 1) replaced
        /// by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin storing the data read
        /// from the current stream.
        /// </param>
        /// <param name="count">
        /// The maximum number of bytes to be read from the current stream.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is
        /// System.Threading.CancellationToken.None.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous read operation. The value of the TResult
        /// parameter contains the total number of bytes read into the buffer. This can be
        /// less than the number of bytes requested if that many bytes are not currently
        /// available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int bytesToRead = count < RemainingPartSize ? count : (int)RemainingPartSize;
            if (bytesToRead <= 0)
                return 0;
            return await base.ReadAsync(buffer, offset, bytesToRead, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes a sequence of bytes to the current stream and advances the
        /// current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. This method copies count bytes from buffer to the current stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin copying bytes to the
        /// current stream.
        /// </param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests. The default value is
        /// System.Threading.CancellationToken.None.
        /// </param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
