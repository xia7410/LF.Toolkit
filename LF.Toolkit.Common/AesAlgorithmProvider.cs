using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LF.Toolkit.Common
{
    /// <summary>
    /// 表示Aes加密算法提供类
    /// </summary>
    public class AesAlgorithmProvider
    {
        /// <summary>
        /// 转换指定数组的制定区域
        /// </summary>
        /// <param name="encrypt">是否加密</param>
        /// <param name="buffer">字节数组</param>
        /// <param name="keys">密钥字节数组(128,192,256 bit)</param>
        /// <param name="mode">加密块密码模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static byte[] TransformFinalBlock(bool encrypt, byte[] keys, byte[] buffer, CipherMode mode, PaddingMode padding)
        {
            using (var algorithm = SymmetricAlgorithm.Create("Aes"))
            {
                algorithm.Key = keys;
                var iv = new byte[16];
                Array.Copy(keys, 0, iv, 0, 16);
                algorithm.IV = iv;
                algorithm.Mode = mode;
                algorithm.Padding = padding;
                ICryptoTransform transform = null;
                if (encrypt)
                {
                    transform = algorithm.CreateEncryptor();
                }
                else
                {
                    transform = algorithm.CreateDecryptor();
                }

                return transform.TransformFinalBlock(buffer, 0, buffer.Length);
            }
        }

        public static byte[] Encrypt(byte[] keys, byte[] buffer, CipherMode mode, PaddingMode padding)
        {
            return TransformFinalBlock(true, keys, buffer, mode, padding);
        }

        public static byte[] Encrypt(Encoding encoding, string key, string text, CipherMode mode, PaddingMode padding)
        {
            return Encrypt(encoding.GetBytes(key), encoding.GetBytes(text), mode, padding);
        }

        public static byte[] Encrypt(string key, string text, CipherMode mode, PaddingMode padding)
        {
            return Encrypt(Encoding.UTF8, key, text, mode, padding);
        }

        public static byte[] Decrypt(byte[] keys, byte[] buffer, CipherMode mode, PaddingMode padding)
        {
            return TransformFinalBlock(false, keys, buffer, mode, padding);
        }

        public static string Decrypt(Encoding encoding, byte[] keys, byte[] buffer, CipherMode mode, PaddingMode padding)
        {
            return encoding.GetString(Decrypt(keys, buffer, mode, padding));
        }
    }
}
