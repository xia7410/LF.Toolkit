using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LF.Toolkit.Util.Crypto
{
    /// <summary>
    /// 表示Md5 帮助类
    /// </summary>
    public class MD5Provider
    {
        /// <summary>
        /// 计算字符串的md5值（默认utf-8）
        /// </summary>
        /// <param name="str">输入的字符串</param>
        /// <returns>32位小写的16进制的hash值</returns>
        public static string ComputeHash(string str)
        {
            String hash = String.Empty;

            if (!string.IsNullOrEmpty(str))
            {
                byte[] encoder = Encoding.UTF8.GetBytes(str);
                hash = ComputeHash(encoder);
            }

            return hash;
        }

        /// <summary>
        /// 计算字节数组的md5值
        /// </summary>
        /// <param name="buffer">输入的byte数组</param>
        /// <returns>32位小写的16进制的hash值</returns>
        public static string ComputeHash(byte[] buffer)
        {
            String hash = String.Empty;

            if (buffer != null && buffer.Length > 0)
            {
                StringBuilder sb = new StringBuilder();

                using (MD5 md5 = MD5.Create())
                {
                    byte[] data = md5.ComputeHash(buffer);
                    foreach (byte b in data)
                    {
                        //转换为16进制的hash码
                        sb.Append(b.ToString("x2"));
                    }
                }

                hash = sb.ToString();
            }

            return hash;
        }

        /// <summary>
        /// 计算流的md5值
        /// </summary>
        /// <param name="stream">IO流</param>
        /// <returns>32位小写的16进制的hash值</returns>
        public static string ComputeHash(Stream stream)
        {
            String hash = String.Empty;

            if (stream != null)
            {
                using (MD5 md5 = MD5.Create())
                {
                    StringBuilder sb = new StringBuilder();

                    byte[] data = md5.ComputeHash(stream);

                    foreach (byte b in data)
                    {
                        //转换为16进制的hash码
                        sb.Append(b.ToString("x2"));
                    }

                    hash = sb.ToString();
                }
            }

            return hash;
        }

    }
}
