using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LF.Toolkit.Common
{
    /// <summary>
    /// 表示哈希加密算法提供类
    /// 算法名称：MD5,SHA1,SHA256, SHA384,SHA512
    /// </summary>
    public class HashAlgorithmProvider
    {
        static string ConvertToHex(byte[] buffer, bool lowerCase)
        {
            string hex = BitConverter.ToString(buffer).Replace("-", "");
            if (lowerCase) hex = hex.ToLower();

            return hex;
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字符串（UTF-8字节编码）的Hash值
        /// </summary>
        /// <param name="alg">算法名称</param>
        /// <param name="input">输入字符串</param>
        /// <returns></returns>
        public static byte[] ComputeHash(string alg, string input)
        {
            return ComputeHash(alg, input, Encoding.UTF8);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字符串（指定编码）的Hash值
        /// </summary>
        /// <param name="alg">算法名称</param>
        /// <param name="input">输入字符串</param>
        /// <param name="encoding">字符编码</param>
        /// <returns></returns>
        public static byte[] ComputeHash(string alg, string input, Encoding encoding)
        {
            var buffer = encoding.GetBytes(input);
            return ComputeHash(alg, buffer);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节数组的Hash值
        /// </summary>
        /// <param name="alg">算法名称</param>
        /// <param name="buffer">字节数组</param>
        /// <returns></returns>
        public static byte[] ComputeHash(string alg, byte[] buffer)
        {
            return HashAlgorithm.Create(alg).ComputeHash(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节数组的Hash值
        /// </summary>
        /// <param name="alg">算法名称</param>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">开始位置</param>
        /// <param name="count">长度</param>
        /// <returns></returns>
        public static byte[] ComputeHash(string alg, byte[] buffer, int offset, int count)
        {
            return HashAlgorithm.Create(alg).ComputeHash(buffer, offset, count);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节流的Hash值
        /// </summary>
        /// <param name="alg">算法名称</param>
        /// <param name="stream">字节流</param>
        /// <returns></returns>
        public static byte[] ComputeHash(string alg, Stream stream)
        {
            return HashAlgorithm.Create(alg).ComputeHash(stream);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字符串（UTF-8字节编码）的Hash值
        /// </summary>
        /// <param name="alg">算法名称</param>
        /// <param name="input">输入字符串</param>
        /// <param name="lowerCase">是否小写</param>
        /// <returns></returns>
        public static string ComputeHash(string alg, string input, bool lowerCase)
        {
            var data = ComputeHash(alg, input);

            return ConvertToHex(data, lowerCase);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字符串（指定编码）的Hash值
        /// </summary>
        /// <param name="alg">算法名称</param>
        /// <param name="input">输入字符串</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="lowerCase">是否小写</param>
        /// <returns></returns>
        public static string ComputeHash(string alg, string input, Encoding encoding, bool lowerCase)
        {
            var buffer = encoding.GetBytes(input);
            var data = ComputeHash(alg, buffer, 0, buffer.Length);

            return ConvertToHex(data, lowerCase);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节数组的Hash值
        /// </summary>
        /// <param name="alg">算法名称</param>
        /// <param name="buffer">字节数组</param>
        /// <param name="lowerCase">是否小写</param>
        /// <returns></returns>
        public static string ComputeHash(string alg, byte[] buffer, bool lowerCase)
        {
            var data = ComputeHash(alg, buffer);

            return ConvertToHex(data, lowerCase);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节数组的Hash值
        /// </summary>
        /// <param name="alg">算法名称</param>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">开始位置</param>
        /// <param name="count">长度</param>
        /// <param name="lowerCase">是否小写</param>
        /// <returns></returns>
        public static string ComputeHash(string alg, byte[] buffer, int offset, int count, bool lowerCase)
        {
            var data = ComputeHash(alg, buffer, offset, count);
            return ConvertToHex(data, lowerCase);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节流的Hash值
        /// </summary>
        /// <param name="alg">算法名称</param>
        /// <param name="stream">字节流</param>
        /// <param name="lowerCase">是否小写</param>
        /// <returns></returns>
        public static string ComputeHash(string alg, Stream stream, bool lowerCase)
        {
            var data = ComputeHash(alg, stream);

            return ConvertToHex(data, lowerCase);
        }
    }
}
