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
        /// 转换制定数组的制定区域
        /// </summary>
        /// <param name="ciperType">加解密类型</param>
        /// <param name="buffer">字节数组</param>
        /// <param name="key">密钥字节数组(128,192,256 bit)</param>
        /// <param name="mode">加密块密码模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static byte[] TransformFinalBlock(CipherType ciperType, byte[] buffer, byte[] key, CipherMode mode, PaddingMode padding)
        {
            if (key.Length != 16 && key.Length != 24 && key.Length != 32) throw new ArgumentException("key");

            using (var algorithm = SymmetricAlgorithm.Create("Aes"))
            {
                algorithm.Key = key;
                var iv = new byte[16];
                Array.Copy(key, 0, iv, 0, 16);
                algorithm.IV = iv;
                algorithm.Mode = mode;
                algorithm.Padding = padding;
                ICryptoTransform transform = null;
                switch (ciperType)
                {
                    case CipherType.Encrypt:
                        transform = algorithm.CreateEncryptor();
                        break;
                    case CipherType.Decrypt:
                        transform = algorithm.CreateDecryptor();
                        break;
                    default:
                        break;
                }

                return transform.TransformFinalBlock(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// 加密并用base64编码加密结果
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="mode">加密块密码模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static string EncryptToBase64(string input, string key, CipherMode mode, PaddingMode padding)
        {
            return EncryptToBase64(Encoding.UTF8, input, key, mode, padding);
        }

        /// <summary>
        /// 加密并用base64编码加密结果
        /// </summary>
        /// <param name="encoding">字符编码</param>
        /// <param name="input">输入字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="mode">加密块密码模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static string EncryptToBase64(Encoding encoding, string input, string key, CipherMode mode, PaddingMode padding)
        {
            return EncryptToBase64(encoding.GetBytes(input), encoding.GetBytes(key), mode, padding);
        }

        /// <summary>
        /// 加密并用base64编码加密结果
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="key">密钥字节数组</param>
        /// <param name="mode">加密块密码模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static string EncryptToBase64(byte[] buffer, byte[] key, CipherMode mode, PaddingMode padding)
        {
            var data = TransformFinalBlock(CipherType.Encrypt, buffer, key, mode, padding);

            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// 解密用base64编码的字符串
        /// </summary>
        /// <param name="cipherBase64">加密字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="mode">加密块密码模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static string DecryptFromBase64(string cipherBase64, string key, CipherMode mode, PaddingMode padding)
        {
            return DecryptFromBase64(Encoding.UTF8, cipherBase64, key, mode, padding);
        }

        /// <summary>
        /// 解密用base64编码的字符串
        /// </summary>
        /// <param name="encoding">字符编码</param>
        /// <param name="cipherBase64">加密字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="mode">加密块密码模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static string DecryptFromBase64(Encoding encoding, string cipherBase64, string key, CipherMode mode, PaddingMode padding)
        {
            return DecryptFromBase64(Encoding.UTF8, Convert.FromBase64String(cipherBase64), encoding.GetBytes(key), mode, padding);
        }

        /// <summary>
        /// 解密加密的字节数组
        /// </summary>
        /// <param name="encoding">字符编码</param>
        /// <param name="buffer">字节数组</param>
        /// <param name="key">密钥字节数组</param>
        /// <param name="mode">加密块密码模式</param>
        /// <param name="padding">填充模式</param>
        /// <returns></returns>
        public static string DecryptFromBase64(Encoding encoding, byte[] buffer, byte[] key, CipherMode mode, PaddingMode padding)
        {
            var data = TransformFinalBlock(CipherType.Decrypt, buffer, key, mode, padding);

            return encoding.GetString(data);
        }

    }
}
