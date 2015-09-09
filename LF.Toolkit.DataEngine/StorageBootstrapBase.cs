using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LF.Toolkit.DataEngine
{
    /// <summary>
    /// 表示存储启动实现基类
    /// </summary>
    public abstract class StorageBootstrapBase : IStorageBootstrap
    {
        ConcurrentDictionary<Type, object> storageInstancePool;

        ConcurrentDictionary<Type, object> quueryCachePool;

        public StorageBootstrapBase()
        {
            storageInstancePool = new ConcurrentDictionary<Type, object>();
            quueryCachePool = new ConcurrentDictionary<Type, object>();
        }

        /// <summary>
        /// 创建指定存储类型的实例
        /// </summary>
        /// <typeparam name="T">派生自IStorageBase的类型</typeparam>
        /// <returns></returns>
        protected abstract object CreateInstance<T>() where T : class, IStorageBase;

        /// <summary>
        /// 从指定程序集载入指定存储基类的所有子类实例
        /// </summary>
        /// <typeparam name="TStorageBase">派生自IStorageBase的类型</typeparam>
        /// <param name="assembly">实现存储接口的程序集</param>
        void IStorageBootstrap.CreateInstanceFrom<TStorageBase>(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            var baseType = typeof(TStorageBase);
            var implTypes = assembly.GetTypes().Where(i => i.IsSubclassOf(baseType) && !i.IsAbstract);
            if (implTypes.Count() > 0)
            {
                var createInstance = this.GetType().GetMethod("CreateInstance", BindingFlags.NonPublic 
                    | BindingFlags.Instance);
                //并发创建实例
                implTypes.AsParallel().ForAll(t =>
                {
                    var value = createInstance.MakeGenericMethod(t).Invoke(this, null);
                    if (value != null)
                    {
                        storageInstancePool.AddOrUpdate(t, value, (k, v) => value);
                    }
                });
            }
            else
            {
                throw new Exception("Could not find the '" + baseType.FullName + "' Subclasses in assembly");
            }
        }

        /// <summary>
        /// 创建指定接口类型的实例引用
        /// </summary>
        /// <typeparam name="TInterface">所有派生自IBootstrap的接口类型</typeparam>
        /// <returns></returns>
        TInterface IStorageBootstrap.CreateInstanceRef<TInterface>()
        {
            var interType = typeof(TInterface);
            if (!interType.IsInterface) throw new Exception(interType.FullName + " is not interface");

            object value = null;
            //先从查询缓存池中查找实例，若不存在则从实例池中查找
            if (!quueryCachePool.TryGetValue(interType, out value))
            {
                var kv = storageInstancePool.FirstOrDefault(i => interType.IsAssignableFrom(i.Key));
                if (kv.Key != null)
                {
                    value = kv.Value;
                    quueryCachePool.AddOrUpdate(interType, value, (k, v) => value);
                }
            }

            if (value == null) throw new Exception("Could not find the '" + interType.FullName + "' implementation class instance");

            return value as TInterface;
        }
    }
}
