using System.IO;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client
{
    /// <summary>
    /// This class is used to wrap the function response stream.
    /// It allows the wrapped stream to be reused.
    /// </summary>
    internal class NonDisposingStreamWrapper : Stream
    {
        Stream _wrappedStream;

        public NonDisposingStreamWrapper(Stream wrappedStream)
        {
            _wrappedStream = wrappedStream;
        }

        public override bool CanRead
        {
            get
            {
                return _wrappedStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return _wrappedStream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return _wrappedStream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return _wrappedStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return _wrappedStream.Position;
            }

            set
            {
                _wrappedStream.Position = value;
            }
        }

        public override void Flush()
        {
            _wrappedStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _wrappedStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _wrappedStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _wrappedStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _wrappedStream.Write(buffer, offset, count);
        }
    }
}
