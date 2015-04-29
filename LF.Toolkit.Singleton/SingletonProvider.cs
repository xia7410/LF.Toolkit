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

        /// <summary>
        /// 获取当前会话实例并转化为其基类或接口对象
        /// </summary>
        /// <typeparam name="TInter">T的基类或接口</typeparam>
        /// <returns></returns>
        public static TInter GetCurrentSessionAs<TInter>()
            where TInter : class
        {
            if (lazyInstance.Value is TInter)
            {
                return lazyInstance.Value as TInter;
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}
