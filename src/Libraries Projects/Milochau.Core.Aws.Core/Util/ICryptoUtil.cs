using System.IO;

namespace Milochau.Core.Aws.Core.Util
{
    public interface ICryptoUtil
    {
        byte[] ComputeSHA256Hash(byte[] data);
        byte[] ComputeSHA256Hash(Stream steam);

        byte[] HMACSignBinary(byte[] data, byte[] key);
    }
}
