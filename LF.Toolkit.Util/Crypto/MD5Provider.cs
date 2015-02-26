﻿using System;
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
        /// 使用指定的格式将当前 System.Byte 对象的值转换为它的等效字符串表示形式。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="format">一个数值格式字符串</param>
        /// <returns></returns>
        static string Format(byte[] buffer, string format)
        {
            StringBuilder sb = new StringBuilder();

            if (buffer != null && buffer.Length > 0)
            {
                foreach (byte b in buffer)
                {
                    sb.Append(b.ToString(format));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 计算字符串的md5值（默认utf-8）
        /// </summary>
        /// <param name="str">要计算其哈希代码的输入</param>
        /// <param name="format">一个数值格式字符串</param>
        /// <returns></returns>
        public static string ComputeHash(string str, string format = "x2")
        {
            String hash = String.Empty;

            if (!string.IsNullOrEmpty(str))
            {
                byte[] encoder = Encoding.UTF8.GetBytes(str);
                hash = ComputeHash(encoder, format);
            }

            return hash;
        }

        /// <summary>
        /// 计算byte数组的md5值
        /// </summary>
        /// <param name="buffer">要计算其哈希代码的输入</param>
        /// <param name="format">一个数值格式字符串</param>
        /// <returns></returns>
        public static string ComputeHash(byte[] buffer, string format = "x2")
        {
            String hash = String.Empty;

            if (buffer != null && buffer.Length > 0)
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] data = md5.ComputeHash(buffer);
                    hash = Format(data, format);
                }
            }

            return hash;
        }

        /// <summary>
        /// 计算byte数组的md5值
        /// </summary>
        /// <param name="buffer">要计算其哈希代码的输入</param>
        /// <param name="offset">字节数组中的偏移量，从该位置开始使用数据</param>
        /// <param name="count">数组中用作数据的字节数</param>
        /// <param name="format">一个数值格式字符串</param>
        /// <returns></returns>
        public static string ComputeHash(byte[] buffer, int offset, int count, string format = "x2")
        {
            String hash = String.Empty;

            if (buffer != null && buffer.Length > 0)
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] data = md5.ComputeHash(buffer, offset, count);
                    hash = Format(data, format);
                }
            }

            return hash;
        }

        /// <summary>
        /// 计算字流的md5值
        /// </summary>
        /// <param name="str">要计算其哈希代码的输入</param>
        /// <param name="format">一个数值格式字符串</param>
        /// <returns></returns>
        public static string ComputeHash(Stream stream, string format = "x2")
        {
            String hash = String.Empty;

            if (stream != null)
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] data = md5.ComputeHash(stream);
                    hash = Format(data, format);
                }
            }

            return hash;
        }

    }
}
