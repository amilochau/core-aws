namespace Milochau.Core.Aws.Core.Runtime.Internal.Util
{
    public interface IEncryptionWrapper
    {
        void Reset();
        int AppendBlock(byte[] buffer, int offset, int count, byte[] target, int targetOffset);
        byte[] AppendLastBlock(byte[] buffer, int offset, int count);
        void SetEncryptionData(byte[] key, byte[] IV);
        void CreateEncryptor();
    }
}
