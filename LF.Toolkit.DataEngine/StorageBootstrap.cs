using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LF.Toolkit.DataEngine
{
    /// <summary>
    /// 表示无参构造存储启动实现类
    /// </summary>
    public class StorageBootstrap : StorageBootstrapBase
    {
        /// <summary>
        /// 创建指定存储类型的实例
        /// </summary>
        /// <typeparam name="T">派生自IStorageBase的类型</typeparam>
        /// <returns></returns>
        protected override object CreateInstance<T>()
        {
            var type = typeof(T);
            //查找当前类型是否含有无参构造
            var constructor = type.GetConstructors().Where(i => i.GetParameters().Length == 0).FirstOrDefault();
            //初始化无参构造对象的实例
            if (constructor != null)
            {
                return type.Assembly.CreateInstance(type.FullName, false, BindingFlags.Default | BindingFlags.CreateInstance
                        | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);
            }

            return null;
        }
    }

    /// <summary>
    /// 表示含有一个参数构造存储启动实现基类
    /// </summary>
    /// <typeparam name="TBootstrapArgument">存储启动构造参数类型</typeparam>
    public abstract class StorageBootstrap<TBootstrapArgument> : StorageBootstrapBase { }
}
