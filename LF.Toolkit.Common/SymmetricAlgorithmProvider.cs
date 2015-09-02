using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LF.Toolkit.Common
{
    public class SymmetricAlgorithmProvider
    {
        static SymmetricAlgorithm Create(SymmetricAlgorithmType algType)
        {
            string algName = Enum.GetName(typeof(SymmetricAlgorithmType), algType);
            return SymmetricAlgorithm.Create(algName);
        }

        public static byte[] TransformFinalBlock(SymmetricAlgorithmType algType, byte[] buffer, bool decrypt, byte[] key, CipherMode mode, PaddingMode padding)
        {
            using (var algorithm = Create(algType))
            {
                algorithm.Key = key;
                var iv = new byte[16];
                Array.Copy(key, 0, iv, 0, 16);
                algorithm.IV = iv;
                algorithm.Mode = mode;
                algorithm.Padding = padding;
                ICryptoTransform transform = null;
                if (decrypt)
                {
                    transform = algorithm.CreateDecryptor();
                }
                else
                {
                    transform = algorithm.CreateEncryptor();
                }

                return transform.TransformFinalBlock(buffer, 0, buffer.Length);
            }
        }

        public static string EncryptToBase64(SymmetricAlgorithmType algType, byte[] buffer, byte[] key, CipherMode mode, PaddingMode padding)
        {
            var data = TransformFinalBlock(algType, buffer, false, key, mode, padding);

            return Convert.ToBase64String(data);
        }

        public static string DecryptFromBase64(SymmetricAlgorithmType algType, byte[] buffer, Encoding encoding, byte[] key, CipherMode mode, PaddingMode padding)
        {
            var data = TransformFinalBlock(algType, buffer, true, key, mode, padding);

            return encoding.GetString(data);
        }
    }
}
