using System;

namespace ThirdParty.Ionic.Zlib
{
    /// <summary>
    /// Calculates a 32bit Cyclic Redundancy Checksum (CRC) using the
    /// same polynomial used by Zip. This type is used internally by DotNetZip; it is generally not used directly
    /// by applications wishing to create, read, or manipulate zip archive files.
    /// </summary>
    internal class CRC32
    {
        /// <summary>
        /// indicates the total number of bytes read on the CRC stream.
        /// This is used when writing the ZipDirEntry when compressing files.
        /// </summary>
        public long TotalBytesRead { get; private set; }

        /// <summary>
        /// Indicates the current CRC for all blocks slurped in.
        /// </summary>
        public int Crc32Result
        {
            get
            {
                // return one's complement of the running result
                return unchecked((int)~_RunningCrc32Result);
            }
        }

        /// <summary>
        /// Update the value for the running CRC32 using the given block of bytes.
        /// This is useful when using the CRC32() class in a Stream.
        /// </summary>
        /// <param name="block">block of bytes to slurp</param>
        /// <param name="offset">starting point in the block</param>
        /// <param name="count">how many bytes within the block to slurp</param>
        public void SlurpBlock(byte[] block, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int x = offset + i;
                _RunningCrc32Result = ((_RunningCrc32Result) >> 8) ^ crc32Table[(block[x]) ^ ((_RunningCrc32Result) & 0x000000FF)];
            }
            TotalBytesRead += count;
        }


        // pre-initialize the crc table for speed of lookup.
        static CRC32()
        {
            unchecked
            {
                // This is the official polynomial used by CRC32 in PKZip.
                // Often the polynomial is shown reversed as 0x04C11DB7.
                uint dwPolynomial = 0xEDB88320;
                uint i, j;

                crc32Table = new uint[256];

                uint dwCrc;
                for (i = 0; i < 256; i++)
                {
                    dwCrc = i;
                    for (j = 8; j > 0; j--)
                    {
                        if ((dwCrc & 1) == 1)
                        {
                            dwCrc = (dwCrc >> 1) ^ dwPolynomial;
                        }
                        else
                        {
                            dwCrc >>= 1;
                        }
                    }
                    crc32Table[i] = dwCrc;
                }
            }
        }

        private static readonly uint[] crc32Table;
        private uint _RunningCrc32Result = 0xFFFFFFFF;

    }

    /// <summary>
    /// A Stream that calculates a CRC32 (a checksum) on all bytes read, 
    /// or on all bytes written.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    /// This class can be used to verify the CRC of a ZipEntry when reading from a stream, 
    /// or to calculate a CRC when writing to a stream.  The stream should be used to either 
    /// read, or write, but not both.  If you intermix reads and writes, the results are
    /// not defined. 
    /// </para>
    /// <para>This class is intended primarily for use internally by the DotNetZip library.</para>
    /// </remarks>
    /// <remarks>
    /// The constructor.
    /// </remarks>
    /// <param name="stream">The underlying stream</param>
    /// <param name="length">The length of the stream to slurp</param>
    public class CrcCalculatorStream(System.IO.Stream stream, long length) : System.IO.Stream()
    {
        private readonly CRC32 _Crc32 = new();

        /// <summary>
        /// Provides the current CRC for all blocks slurped in.
        /// </summary>
        public int Crc32 => _Crc32.Crc32Result;

        /// <summary>
        /// Read from the stream
        /// </summary>
        /// <param name="buffer">the buffer to read</param>
        /// <param name="offset">the offset at which to start</param>
        /// <param name="count">the number of bytes to read</param>
        /// <returns>the number of bytes actually read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesToRead = count;

            // Need to limit the # of bytes returned, if the stream is intended to have a definite length.
            // This is especially useful when returning a stream for the uncompressed data directly to the 
            // application.  The app won't necessarily read only the UncompressedSize number of bytes.  
            // For example wrapping the stream returned from OpenReader() into a StreadReader() and
            // calling ReadToEnd() on it, We can "over-read" the zip data and get a corrupt string.  
            // The length limits that, prevents that problem. 

            if (length != 0)
            {
                if (_Crc32.TotalBytesRead >= length) return 0; // EOF
                long bytesRemaining = length - _Crc32.TotalBytesRead;
                if (bytesRemaining < count) bytesToRead = (int)bytesRemaining;
            }
            int n = stream.Read(buffer, offset, bytesToRead);
            if (n > 0) _Crc32.SlurpBlock(buffer, offset, n);
            return n;
        }

        /// <summary>
        /// Write to the stream. 
        /// </summary>
        /// <param name="buffer">the buffer from which to write</param>
        /// <param name="offset">the offset at which to start writing</param>
        /// <param name="count">the number of bytes to write</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count > 0) _Crc32.SlurpBlock(buffer, offset, count);
            stream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Indicates whether the stream supports reading. 
        /// </summary>
        public override bool CanRead => stream.CanRead;

        /// <summary>
        /// Indicates whether the stream supports seeking. 
        /// </summary>
        public override bool CanSeek => stream.CanSeek;

        /// <summary>
        /// Indicates whether the stream supports writing. 
        /// </summary>
        public override bool CanWrite => stream.CanWrite;

        /// <summary>
        /// Flush the stream.
        /// </summary>
        public override void Flush()
        {
            stream.Flush();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public override long Length
        {
            get
            {
                return length == 0 ? throw new NotImplementedException() : length;
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public override long Position
        {
            get { return _Crc32.TotalBytesRead; }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="offset">N/A</param>
        /// <param name="origin">N/A</param>
        /// <returns>N/A</returns>
        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">N/A</param>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
    }
}