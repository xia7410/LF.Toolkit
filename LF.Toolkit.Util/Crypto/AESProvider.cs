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
        /// 将数据加密并转成Base64字符串
        /// </summary>
        /// <param name="inputBuffer">要解密的数据</param>
        /// <param name="keyBuffer">密钥</param>
        /// <returns></returns>
        public static String EncryptToBase64(byte[] keyBuffer, byte[] inputBuffer)
        {
            byte[] buffer = Encrypt(keyBuffer , inputBuffer);

            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// 将数据加密并转成Base64字符串
        /// </summary>
        /// <param name="inputBuffer">要解密的数据</param>
        /// <param name="keyBuffer">密钥</param>
        /// <returns></returns>
        public static String EncryptToBase64(string key , string palinText)
        {
            byte[] buffer = Encrypt(Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(palinText));

            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// 从Base64字符串中解密
        /// </summary>
        /// <param name="crtyptoString">密文的Base64字符串</param>
        /// <param name="keyBuffer">密钥</param>
        /// <returns></returns>
        public static String DecryptFromBase64(byte[] keyBuffer, string crtyptoString)
        {
            byte[] buffer = Convert.FromBase64String(crtyptoString);
            byte[] data = Decrypt(keyBuffer, buffer);

            return UTF8Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// 从Base64字符串中解密
        /// </summary>
        /// <param name="crtyptoString">密文的Base64字符串</param>
        /// <param name="keyBuffer">密钥</param>
        /// <returns></returns>
        public static String DecryptFromBase64(string key, string crtyptoString)
        {
            byte[] buffer = Convert.FromBase64String(crtyptoString);
            byte[] data = Decrypt(Encoding.UTF8.GetBytes(key) , buffer);

            return UTF8Encoding.UTF8.GetString(data);
        }

    }
}
