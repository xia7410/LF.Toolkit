using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LF.Toolkit.Singleton
{
    /// <summary>
    /// 单例工厂实例提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SingletonProvider<T> where T : class,ISingleton, new()
    {
        private static readonly Lazy<T> lazyInstance = new Lazy<T>(() =>
        {
            return (T)typeof(T).Assembly.CreateInstance(typeof(T).FullName, false,
                BindingFlags.CreateInstance | BindingFlags.Instance
                | BindingFlags.Public, null, null, null, null);
        }, true);

        /// <summary>
        /// 获取单例类的实例会话
        /// </summary>
        /// <returns></returns>
        public static T CurrentSession { get { return lazyInstance.Value; } }
    }
}
