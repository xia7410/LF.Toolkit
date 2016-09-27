using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LF.Toolkit.Common
{
    public class StringProvider
    {
        /// <summary>
        /// 判断字符串是否数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumberic(string value)
        {
            const string pattern = @"^\d+$";
            return Regex.IsMatch(value, pattern);
        }

        static MethodInfo GetTryParseMethodInfo(Type numericType)
        {
            MethodInfo method = null;
            if (numericType == typeof(int) || numericType == typeof(byte) || numericType == typeof(float)
                || numericType == typeof(double) || numericType == typeof(decimal))
            {
                var methodInfos = numericType.GetMember("TryParse", MemberTypes.Method, BindingFlags.Public | BindingFlags.Static);
                if (methodInfos.Length > 0)
                {
                    foreach (MethodInfo mi in methodInfos)
                    {
                        if (mi.GetParameters().Length == 2)
                        {
                            method = mi;
                            break;
                        }
                    }
                }
            }

            return method;
        }

        /// <summary>
        /// 分割字符串并转成相应的数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="split"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] SpiltToNumbers<T>(string split, string value)
        {
            T[] arrays = new T[0];

            if (!string.IsNullOrEmpty(split) && !string.IsNullOrEmpty(value))
            {
                var tryParse = GetTryParseMethodInfo(typeof(T));
                var vs = value.Split(new string[] { split }, StringSplitOptions.RemoveEmptyEntries);
                if (vs.Length > 0)
                {
                    arrays = new T[vs.Length];
                    for (int i = 0; i < vs.Length; i++)
                    {
                        var parms = new object[] { vs[i], null };
                        if ((bool)tryParse.Invoke(null, parms))
                        {
                            arrays[i] = (T)parms[1];
                        }
                        else
                        {
                            throw new Exception("指定转换的值无效");
                        }
                    }
                }
            }

            return arrays;
        }
    }
}
