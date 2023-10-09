using System;
using System.Security.Cryptography;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Util
{
    public partial class HashingWrapper : IHashingWrapper
    {
        private HashAlgorithm _algorithm = null;


        #region IHashingWrapper Members

        public void Clear()
        {
            _algorithm.Initialize();
        }

        public void AppendBlock(byte[] buffer, int offset, int count)
        {
            // We're not transforming the data, so don't use the outputBuffer arguments
            _algorithm.TransformBlock(buffer, offset, count, null, 0);
        }

        public byte[] AppendLastBlock(byte[] buffer, int offset, int count)
        {
            _algorithm.TransformFinalBlock(buffer, offset, count);
            return _algorithm.Hash;
        }

        #endregion

        #region Dispose Pattern Implementation

        /// <summary>
        /// Implements the Dispose pattern
        /// </summary>
        /// <param name="disposing">Whether this object is being disposed via a call to Dispose
        /// or garbage collected.</param>
        protected virtual void Dispose(bool disposing)
        {
            var disposable = _algorithm as IDisposable;
            if (disposing && disposable != null)
            {
                disposable.Dispose();
                _algorithm = null;
            }
        }

        #endregion
    }

    public partial class HashingWrapper : IHashingWrapper
    {
        public HashingWrapper(HashAlgorithm algorithm)
        {
            _algorithm = algorithm;
        }

         #region IHashingWrapper Members

        public byte[] AppendLastBlock(byte[] buffer)
        {
            return AppendLastBlock(buffer, 0, buffer.Length);
        }

        #endregion


        #region Dispose Pattern Implementation

        /// <summary>
        /// Disposes of all managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
