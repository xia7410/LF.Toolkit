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
        ConcurrentDictionary<Type, object> InstanceTable { get; set; }

        ConcurrentDictionary<Type, object> QuueryTable { get; set; }

        public StorageBootstrapBase()
        {
            InstanceTable = new ConcurrentDictionary<Type, object>();
            QuueryTable = new ConcurrentDictionary<Type, object>();
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
            if (!baseType.IsAbstract) throw new Exception(baseType.FullName + " is not abstract StorageBase class");
            var implTypes = assembly.GetTypes().Where(i => i.IsSubclassOf(baseType) && !i.IsAbstract);
            if (implTypes.Count() > 0)
            {
                var createInstance = this.GetType().GetMethod("CreateInstance", BindingFlags.NonPublic 
                    | BindingFlags.Instance);
                foreach (var type in implTypes)
                {
                    var instance = createInstance.MakeGenericMethod(type).Invoke(this, null);
                    if (instance != null)
                    {
                        InstanceTable.AddOrUpdate(type, instance, (k, v) => instance);
                    }
                }
            }
            else
            {
                throw new Exception("Could not find the '" + baseType.FullName + "' implementation classes in assembly");
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
            if (!interType.IsInterface) throw new Exception(interType.FullName + " is not interface type");

            object value = null;
            //先从查询表中查找实例，若不存在则从实例表中查找
            if (!QuueryTable.TryGetValue(interType, out value))
            {
                var kv = InstanceTable.FirstOrDefault(i => interType.IsAssignableFrom(i.Key));
                if (kv.Key != null)
                {
                    value = kv.Value;
                    QuueryTable.AddOrUpdate(interType, value, (k, v) => value);
                }
            }

            if (value == null) throw new Exception("Could not find the '" + interType.FullName + "' implementation class instance");

            return value as TInterface;
        }
    }
}
