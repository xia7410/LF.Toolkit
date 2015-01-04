using LF.Toolkit.Data.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LF.Toolkit.Data
{
    public sealed class StorageProvider<T> where T : IStorage
    {
        private static readonly Lazy<T> lazyInstance = new Lazy<T>(() =>
        {
            Assembly asm = typeof(T).Assembly;
            return (T)asm.CreateInstance(typeof(T).FullName, false, BindingFlags.CreateInstance | BindingFlags.Instance
                | BindingFlags.Public, null, null, null, null);
        }, true);

        /// <summary>
        /// 获取指定数据库存储类的实例
        /// </summary>
        /// <returns></returns>
        public static T Factory { get { return lazyInstance.Value; } }
    }
}
