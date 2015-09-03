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
    /// </summary>
    public class HashAlgorithmProvider
    {
        static HashAlgorithm CreateAlgorithm(HashAlgorithmType algType)
        {
            string algName = Enum.GetName(typeof(HashAlgorithmType), algType);
            return HashAlgorithm.Create(algName);
        }

        static string ConvertToHex(byte[] buffer, bool lowerCase)
        {
            string hex = BitConverter.ToString(buffer).Replace("-", "");
            if (lowerCase) hex = hex.ToLower();

            return hex;
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字符串（UTF-8字节编码）的Hash值
        /// </summary>
        /// <param name="algType">算法类型</param>
        /// <param name="input">输入字符串</param>
        /// <returns></returns>
        public static byte[] ComputeHash(HashAlgorithmType algType, string input)
        {
            return ComputeHash(algType, input, Encoding.UTF8);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字符串（指定编码）的Hash值
        /// </summary>
        /// <param name="algType">算法类型</param>
        /// <param name="input">输入字符串</param>
        /// <param name="encoding">字符编码</param>
        /// <returns></returns>
        public static byte[] ComputeHash(HashAlgorithmType algType, string input, Encoding encoding)
        {
            var buffer = encoding.GetBytes(input);
            return ComputeHash(algType, buffer);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节数组的Hash值
        /// </summary>
        /// <param name="algType">算法类型</param>
        /// <param name="buffer">字节数组</param>
        /// <returns></returns>
        public static byte[] ComputeHash(HashAlgorithmType algType, byte[] buffer)
        {
            return CreateAlgorithm(algType).ComputeHash(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节数组的Hash值
        /// </summary>
        /// <param name="algType">算法类型</param>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">开始位置</param>
        /// <param name="count">长度</param>
        /// <returns></returns>
        public static byte[] ComputeHash(HashAlgorithmType algType, byte[] buffer, int offset, int count)
        {
            return CreateAlgorithm(algType).ComputeHash(buffer, offset, count);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节流的Hash值
        /// </summary>
        /// <param name="algType">算法类型</param>
        /// <param name="stream">字节流</param>
        /// <returns></returns>
        public static byte[] ComputeHash(HashAlgorithmType algType, Stream stream)
        {
            return CreateAlgorithm(algType).ComputeHash(stream);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字符串（UTF-8字节编码）的Hash值
        /// </summary>
        /// <param name="algType">算法类型</param>
        /// <param name="input">输入字符串</param>
        /// <param name="lowerCase">是否小写</param>
        /// <returns></returns>
        public static string ComputeHash(HashAlgorithmType algType, string input, bool lowerCase)
        {
            var data = ComputeHash(algType, input);

            return ConvertToHex(data, lowerCase);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字符串（指定编码）的Hash值
        /// </summary>
        /// <param name="algType">算法类型</param>
        /// <param name="input">输入字符串</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="lowerCase">是否小写</param>
        /// <returns></returns>
        public static string ComputeHash(HashAlgorithmType algType, string input, Encoding encoding, bool lowerCase)
        {
            var buffer = encoding.GetBytes(input);
            var data = ComputeHash(algType, buffer, 0, buffer.Length);

            return ConvertToHex(data, lowerCase);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节数组的Hash值
        /// </summary>
        /// <param name="algType">算法类型</param>
        /// <param name="buffer">字节数组</param>
        /// <param name="lowerCase">是否小写</param>
        /// <returns></returns>
        public static string ComputeHash(HashAlgorithmType algType, byte[] buffer, bool lowerCase)
        {
            var data = ComputeHash(algType, buffer);

            return ConvertToHex(data, lowerCase);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节数组的Hash值
        /// </summary>
        /// <param name="algType">算法类型</param>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">开始位置</param>
        /// <param name="count">长度</param>
        /// <param name="lowerCase">是否小写</param>
        /// <returns></returns>
        public static string ComputeHash(HashAlgorithmType algType, byte[] buffer, int offset, int count, bool lowerCase)
        {
            var data = ComputeHash(algType, buffer, offset, count);
            return ConvertToHex(data, lowerCase);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节流的Hash值
        /// </summary>
        /// <param name="algType">算法类型</param>
        /// <param name="stream">字节流</param>
        /// <param name="lowerCase">是否小写</param>
        /// <returns></returns>
        public static string ComputeHash(HashAlgorithmType algType, Stream stream, bool lowerCase)
        {
            var data = ComputeHash(algType, stream);

            return ConvertToHex(data, lowerCase);
        }
    }
}
