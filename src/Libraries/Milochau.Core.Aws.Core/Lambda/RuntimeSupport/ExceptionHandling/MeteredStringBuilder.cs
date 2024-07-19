using System;
using System.Text;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.ExceptionHandling
{
    internal class MeteredStringBuilder
    {
        private readonly int _maxSize;
        private readonly Encoding _encoding;
        private readonly StringBuilder _stringBuilder;

        public int SizeInBytes { get; private set; }

        public MeteredStringBuilder(Encoding encoding, int maxSize)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxSize);

            _stringBuilder = new StringBuilder();
            SizeInBytes = 0;
            _encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            _maxSize = maxSize;
        }

        public void Append(string str)
        {
            int strSizeInBytes = _encoding.GetByteCount(str);
            _stringBuilder.Append(str);
            SizeInBytes += strSizeInBytes;
        }

        public void AppendLine(string str)
        {
            string strWithLine = str + Environment.NewLine;
            int strSizeInBytes = _encoding.GetByteCount(strWithLine);
            _stringBuilder.Append(strWithLine);
            SizeInBytes += strSizeInBytes;
        }

        public void AppendLine()
        {
            AppendLine("");
        }

        public bool HasRoomForString(string str)
        {
            return SizeInBytes + _encoding.GetByteCount(str) < _maxSize;
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }
    }
}
