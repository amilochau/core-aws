namespace Milochau.Core.Aws.Core.Util
{
    public interface ICryptoUtil
    {
        byte[] ComputeSHA256Hash(byte[] data);

        byte[] HMACSignBinary(byte[] data, byte[] key);
    }
}
