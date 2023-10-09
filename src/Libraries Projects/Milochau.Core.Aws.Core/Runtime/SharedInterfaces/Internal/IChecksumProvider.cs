namespace Milochau.Core.Aws.Core.Runtime.SharedInterfaces.Internal
{
    /// <summary>
    /// Interface for an implementation of checksum algorithms
    /// </summary>
    public interface IChecksumProvider
    {
        /// <summary>
        /// Computes a CRC32 hash
        /// </summary>
        /// <param name="source">Data to hash</param>
        /// <returns>CRC32 hash as a base64-encoded string</returns>
        string Crc32(byte[] source);

        /// <summary>
        /// Computes a CRC32 hash
        /// </summary>
        /// <param name="source">Data to hash</param>
        /// <param name="previous">Previous value of a rolling checksum</param>
        /// <returns>Updated CRC32 hash as 32-bit integer</returns>
        uint Crc32(byte[] source, uint previous);

        /// <summary>
        /// Computes a CRC32C hash
        /// </summary>
        /// <param name="source">Data to hash</param>
        /// <returns>CRC32C hash as a base64-encoded string</returns>
        string Crc32C(byte[] source);

        /// <summary>
        /// Computes a CRC32C hash
        /// </summary>
        /// <param name="source">Data to hash</param>
        /// <param name="previous">Previous value of a rolling checksum</param>
        /// <returns>Updated CRC32C hash as 32-bit integer</returns>
        uint Crc32C(byte[] source, uint previous);
    }
}
