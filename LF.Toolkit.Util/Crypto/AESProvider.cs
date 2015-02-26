using System;
using System.Text;
using System.Security.Cryptography;

namespace LF.Toolkit.Util.Crypto
{
    public class AESProvider
    {
        public static byte[] Encrypt(byte[] keyBuffer, byte[] inputBuffer)
        {
            byte[] outBlock;

            Aes aes = Aes.Create();
            aes.Key = keyBuffer;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform ict = aes.CreateEncryptor();
            outBlock = ict.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            ict.Dispose();
            aes.Clear();

            return outBlock;
        }

        public static byte[] Decrypt(byte[] keyBuffer, byte[] inputBuffer)
        {
            byte[] outBlock;

            Aes aes = Aes.Create();
            aes.Key = keyBuffer;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform ict = aes.CreateDecryptor();
            outBlock = ict.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            ict.Dispose();
            aes.Clear();

            return outBlock;
        }

        /// <summary>
        /// 将数据加密并用Base64编码
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="plainText">明文</param>
        /// <returns></returns>
        public static String EncryptToBase64(string key, string plainText)
        {
            byte[] buffer = Encrypt(Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(plainText));

            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// 从Base64编码的字符串中解密
        /// </summary>
        /// <param name="cryptoString">加密的字符串</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static String DecryptFromBase64(string key, string cryptoString)
        {
            byte[] buffer = Convert.FromBase64String(cryptoString);
            byte[] data = Decrypt(Encoding.UTF8.GetBytes(key) , buffer);

            return Encoding.UTF8.GetString(data);
        }

    }
}
