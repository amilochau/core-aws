using System;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Util
{
    public interface IHashingWrapper : IDisposable
    {
        void Clear();
        void AppendBlock(byte[] buffer, int offset, int count);
        byte[] AppendLastBlock(byte[] buffer);
        byte[] AppendLastBlock(byte[] buffer, int offset, int count);
    }
}
