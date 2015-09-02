using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LF.Toolkit.Common
{
    /// <summary>
    /// 表示随机字符串生成类
    /// </summary>
    public static class RandomStringGenerator
    {
        const string LETTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string NUMERIC = "0123456789";

        enum RandomType
        {
            ALPHANUMERIC,
            LETTERS,
            NUMERIC
        }

        static string CreateRandom(int length, RandomType type)
        {
            if (length < 0) return "";
            char[] arrs = new char[length];
            string reference = "";
            //随机最大值不包含maxValue
            int maxValue = 0;
            Random random = new Random(Guid.NewGuid().GetHashCode());

            switch (type)
            {
                case RandomType.ALPHANUMERIC:
                    reference = NUMERIC + LETTERS;
                    maxValue = 62;
                    break;
                case RandomType.LETTERS:
                    reference = LETTERS;
                    maxValue = 52;
                    break;
                case RandomType.NUMERIC:
                    reference = NUMERIC;
                    maxValue = 10;
                    break;
                default:
                    break;
            }

            if (reference != "")
            {
                for (int i = 0; i < length; i++)
                {
                    arrs[i] = reference[random.Next(0, maxValue)];
                }
                return new string(arrs);
            }

            return "";
        }

        /// <summary>
        /// 创建随机字母与数字字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CreateRandomAlphanumeric(int length)
        {
            return CreateRandom(length, RandomType.ALPHANUMERIC);
        }

        /// <summary>
        /// 创建随机字母字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CreateRandomLetters(int length)
        {
            return CreateRandom(length, RandomType.LETTERS);
        }

        /// <summary>
        /// 创建随机数字字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CreateRandomNumeric(int length)
        {
            return CreateRandom(length, RandomType.NUMERIC);
        }
    }
}
