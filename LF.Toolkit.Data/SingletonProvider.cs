using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LF.Toolkit.Data
{
    public sealed class SingletonProvider<T> where T : class,ISingleton, new()
    {
        private static readonly Lazy<T> lazyInstance = new Lazy<T>(() =>
        {
            return (T)typeof(T).Assembly.CreateInstance(typeof(T).FullName, false,
                BindingFlags.CreateInstance | BindingFlags.Instance
                | BindingFlags.Public, null, null, null, null);
        }, true);

        /// <summary>
        /// 获取指定数据库存储类的实例
        /// </summary>
        /// <returns></returns>
        public static T SessionFactory { get { return lazyInstance.Value; } }
    }
}
