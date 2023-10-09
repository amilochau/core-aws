﻿using System.Security.Cryptography;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Util
{
    public abstract class EncryptionWrapper : IEncryptionWrapper
    {
        private SymmetricAlgorithm algorithm;
        private ICryptoTransform encryptor;
        private const int encryptionKeySize = 256;

        protected EncryptionWrapper()
        {
            algorithm = CreateAlgorithm();
        }

        protected abstract SymmetricAlgorithm CreateAlgorithm();

        #region IEncryptionWrapper Members

        public int AppendBlock(byte[] buffer, int offset, int count, byte[] target, int targetOffset)
        {
            int bytesRead = encryptor.TransformBlock(buffer, offset, count, target, targetOffset);
            return bytesRead;
        }

        public byte[] AppendLastBlock(byte[] buffer, int offset, int count)
        {
            byte[] finalTransform = encryptor.TransformFinalBlock(buffer, offset, count);
            return finalTransform;
        }

        public void CreateEncryptor()
        {
            encryptor = algorithm.CreateEncryptor();
        }

        public void SetEncryptionData(byte[] key, byte[] IV)
        {
            algorithm.KeySize = encryptionKeySize;
            algorithm.Padding = PaddingMode.PKCS7;
            algorithm.Mode = CipherMode.CBC;
            algorithm.Key = key;
            algorithm.IV = IV;
        }

        public void Reset()
        {
            CreateEncryptor();
        }

        #endregion
    }


    public class EncryptionWrapperAES : EncryptionWrapper
    {
        public EncryptionWrapperAES()
            : base() { }

        protected override SymmetricAlgorithm CreateAlgorithm()
        {
            return Aes.Create();
        }
    }
}
