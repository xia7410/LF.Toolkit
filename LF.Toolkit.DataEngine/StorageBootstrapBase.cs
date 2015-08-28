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
        ConcurrentDictionary<Type, object> instanceTable;
        ConcurrentDictionary<Type, object> queryTable;

        /// <returns></returns>
        bool EnsureInitialize()
        {
            return instanceTable != null && queryTable != null;
        }

        protected void Initialize()
        {
            instanceTable = new ConcurrentDictionary<Type, object>();
            queryTable = new ConcurrentDictionary<Type, object>();
        }

        /// <summary>
        /// 获取当前存储启动项的类型
        /// </summary>
        public Type BootstrapType
        {
            get
            {
                return this.GetType();
            }
        }

        /// <summary>
        /// 创建指定存储类型的实例
        /// </summary>
        /// <typeparam name="T">派生自IStorageBase的类型</typeparam>
        /// <returns></returns>
        public abstract object CreateInstance<T>() where T : class, IStorageBase;

        /// <summary>
        /// 从指定程序集载入指定存储基类的所有子类实例
        /// </summary>
        /// <typeparam name="TStorageBase">派生自IStorageBase的类型</typeparam>
        /// <param name="assembly">实现存储接口的程序集</param>
        public void CreateInstanceFrom<TStorageBase>(Assembly assembly)
            where TStorageBase : class, IStorageBase
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            //初始化
            Initialize();

            var baseType = typeof(TStorageBase);
            if (!baseType.IsAbstract) throw new Exception(baseType.FullName + " is not abstract StorageBase class");
            var implTypes = assembly.GetTypes().Where(i => i.IsSubclassOf(baseType) && !i.IsAbstract);
            if (implTypes.Count() > 0)
            {
                var createInstanceMethod = this.GetType().GetMethod("CreateInstance");
                foreach (var type in implTypes)
                {
                    var instance = createInstanceMethod.MakeGenericMethod(type).Invoke(this, null);
                    if (instance != null)
                    {
                        instanceTable.AddOrUpdate(type, instance, (k, v) => instance);
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
        public TInterface CreateInstanceRef<TInterface>() where TInterface : class, IBootstrap
        {
            if (!EnsureInitialize()) throw new Exception(BootstrapType.FullName + " not initialize");

            var interType = typeof(TInterface);
            if (!interType.IsInterface) throw new Exception(interType.FullName + " is not interface type");

            object value = null;
            //先从查询缓存中查找实例，若不存在则从实例表中查找
            if (!queryTable.TryGetValue(interType, out value))
            {
                var kv = instanceTable.FirstOrDefault(i => interType.IsAssignableFrom(i.Key));
                if (kv.Key != null)
                {
                    value = kv.Value;
                    queryTable.AddOrUpdate(interType, value, (k, v) => value);
                }
            }

            if (value == null) throw new Exception("Could not find the '" + interType.FullName + "' implementation class instance");

            return value as TInterface;
        }
    }
}
