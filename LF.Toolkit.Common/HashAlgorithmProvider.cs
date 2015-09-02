using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LF.Toolkit.Common
{
    public class HashAlgorithmProvider
    {
        static HashAlgorithm Create(HashAlgorithmType algType)
        {
            string algName = Enum.GetName(typeof(HashAlgorithmType), algType);
            return HashAlgorithm.Create(algName);
        }

        static string ConvertToHex(byte[] buffer, bool lowercase)
        {
            string hex = BitConverter.ToString(buffer).Replace("-", "");
            if (lowercase) hex = hex.ToLower();

            return hex;
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字符串（UTF-8字节编码）的Hash值
        /// </summary>
        /// <param name="algType"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ComputeHash(HashAlgorithmType algType, string input)
        {
            return ComputeHash(algType, input, Encoding.UTF8);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字符串（指定编码）的Hash值
        /// </summary>
        /// <param name="algType"></param>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ComputeHash(HashAlgorithmType algType, string input, Encoding encoding)
        {
            var buffer = encoding.GetBytes(input);
            return ComputeHash(algType, buffer);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节数组的Hash值
        /// </summary>
        /// <param name="algType"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] ComputeHash(HashAlgorithmType algType, byte[] buffer)
        {
            return Create(algType).ComputeHash(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节数组的Hash值
        /// </summary>
        /// <param name="algType"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] ComputeHash(HashAlgorithmType algType, byte[] buffer, int offset, int count)
        {
            return Create(algType).ComputeHash(buffer, offset, count);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节流的Hash值
        /// </summary>
        /// <param name="algType"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ComputeHash(HashAlgorithmType algType, Stream stream)
        {
            return Create(algType).ComputeHash(stream);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字符串（UTF-8字节编码）的Hash值
        /// </summary>
        /// <param name="algType"></param>
        /// <param name="input"></param>
        /// <param name="lowercase"></param>
        /// <returns></returns>
        public static string ComputeHash(HashAlgorithmType algType, string input, bool lowercase)
        {
            var data = ComputeHash(algType, input);

            return ConvertToHex(data, lowercase);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字符串（指定编码）的Hash值
        /// </summary>
        /// <param name="algType"></param>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        /// <param name="lowercase"></param>
        /// <returns></returns>
        public static string ComputeHash(HashAlgorithmType algType, string input, Encoding encoding, bool lowercase)
        {
            var buffer = encoding.GetBytes(input);
            var data = ComputeHash(algType, buffer, 0, buffer.Length);

            return ConvertToHex(data, lowercase);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节数组的Hash值
        /// </summary>
        /// <param name="algType"></param>
        /// <param name="buffer"></param>
        /// <param name="lowercase"></param>
        /// <returns></returns>
        public static string ComputeHash(HashAlgorithmType algType, byte[] buffer, bool lowercase)
        {
            var data = ComputeHash(algType, buffer);

            return ConvertToHex(data, lowercase);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节数组的Hash值
        /// </summary>
        /// <param name="algType"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="lowercase"></param>
        /// <returns></returns>
        public static string ComputeHash(HashAlgorithmType algType, byte[] buffer, int offset, int count, bool lowercase)
        {
            var data = ComputeHash(algType, buffer, offset, count);
            return ConvertToHex(data, lowercase);
        }

        /// <summary>
        /// 使用指定类型的Hash算法计算字节流的Hash值
        /// </summary>
        /// <param name="algType"></param>
        /// <param name="stream"></param>
        /// <param name="lowercase"></param>
        /// <returns></returns>
        public static string ComputeHash(HashAlgorithmType algType, Stream stream, bool lowercase)
        {
            var data = ComputeHash(algType, stream);

            return ConvertToHex(data, lowercase);
        }
    }
}
