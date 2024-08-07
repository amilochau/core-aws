﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Util
{
    /// <summary>
    /// A stream which caches the contents of the underlying stream as it reads it.
    /// </summary>
    /// <remarks>
    /// Initializes the CachingWrapperStream with a base stream.
    /// </remarks>
    /// <param name="baseStream">The stream to be wrapped.</param>
    public class CachingWrapperStream(Stream baseStream) : WrapperStream(baseStream)
    {
        // Default limit for response logging is 1 KB.
        public static readonly int DefaultLogResponsesSizeLimit = 1024;

        private int _cachedBytes = 0;

        /// <summary>
        /// All the bytes read by the stream.
        /// </summary>
        public List<byte> AllReadBytes { get; private set; } = [];

        /// <summary>
        /// All the bytes read by the stream constrained with _cacheLimit
        /// </summary>
        public List<byte> LoggableReadBytes => AllReadBytes.Take(DefaultLogResponsesSizeLimit).ToList();

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position
        /// within the stream by the number of bytes read.
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
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the
        /// number of bytes requested if that many bytes are not currently available,
        /// or zero (0) if the end of the stream has been reached.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var numberOfBytesRead = base.Read(buffer, offset, count);
            UpdateCacheAfterReading(buffer, offset, numberOfBytesRead);
            return numberOfBytesRead;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var numberOfBytesRead = await base.ReadAsync(buffer.AsMemory(offset, count), cancellationToken).ConfigureAwait(false);
            UpdateCacheAfterReading(buffer, offset, numberOfBytesRead);
            return numberOfBytesRead;
        }

        private void UpdateCacheAfterReading(byte[] buffer, int offset, int numberOfBytesRead)
        {
            if (_cachedBytes < DefaultLogResponsesSizeLimit)
            {
                int remainingBytes = DefaultLogResponsesSizeLimit - _cachedBytes;
                int bytesToCache = Math.Min(numberOfBytesRead, remainingBytes);

                var readBytes = new byte[bytesToCache];
                Array.Copy(buffer, offset, readBytes, 0, bytesToCache);
                AllReadBytes.AddRange(readBytes);
                _cachedBytes += bytesToCache;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// CachingWrapperStream does not support seeking, this will always be false.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                // Restrict random access.
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// CachingWrapperStream does not support seeking, attempting to set Position
        /// will throw NotSupportedException.
        /// </summary>
        public override long Position
        {
            get
            {
                throw new NotSupportedException("CachingWrapperStream does not support seeking");
            }
            set
            {
                // Restrict random access, as this will break hashing.
                throw new NotSupportedException("CachingWrapperStream does not support seeking");
            }
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// CachingWrapperStream does not support seeking, attempting to call Seek
        /// will throw NotSupportedException.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">
        /// A value of type System.IO.SeekOrigin indicating the reference point used
        /// to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            // Restrict random access.
            throw new NotSupportedException("CachingWrapperStream does not support seeking");
        }
    }
}
