using LF.Toolkit.MongoDB.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LF.Toolkit.MongoDB
{
    /// <summary>
    /// 表示支持从存储启动创建接口实例的标识接口
    /// </summary>
    public interface IMongoStroageBootstrap
    {

    }

    public sealed class MongoStroageBootstrap
    {
        /// <summary>
        /// 实例总表
        /// </summary>
        static ConcurrentDictionary<Type, object> dalInstanceTable;

        /// <summary>
        /// 查找实例缓存表
        /// </summary>
        static ConcurrentDictionary<Type, object> queryCacheTable;

        /// <summary>
        /// 初始化相关配置
        /// </summary>
        static void Initialize()
        {
            dalInstanceTable = new ConcurrentDictionary<Type, object>();
            queryCacheTable = new ConcurrentDictionary<Type, object>();
        }

        /// <summary>
        /// 创建MongoDB存储的Bootstrap
        /// </summary>
        /// <param name="configPath">数据库存储配置文件</param>
        /// <param name="dalAssembly">数据库存储实现层程序集</param>
        public static void CreateBootstrap(string configPath, Assembly dalAssembly)
        {
            if (string.IsNullOrEmpty(configPath)) throw new ArgumentNullException("cfgPath");
            if (dalAssembly == null) throw new ArgumentNullException("dalAssembly");

            //初始化相关配置
            Initialize();
            var config = new MongoStorageConfig();
            config.Configure(configPath);
            var baseType = typeof(MongoStorageBase);
            var types = dalAssembly.GetTypes().Where(i => i.IsSubclassOf(baseType));
            if (types.Count() > 0)
            {
                foreach (var t in types)
                {
                    var instance = dalAssembly.CreateInstance(t.FullName, false, BindingFlags.Default | BindingFlags.CreateInstance
                        | BindingFlags.Instance | BindingFlags.Public, null, new object[] { config }, null, null);
                    dalInstanceTable.AddOrUpdate(t, instance, (k, v) => instance);
                }
            }
            else
            {
                throw new Exception("Could not find the 'MongoStorageBase' implementation classes in dalAssembly");
            }
        }

        /// <summary>
        /// 创建指定类型存储接口的实例
        /// </summary>
        /// <typeparam name="TInterface">继承ISqlStorageBootstrap的存储接口</typeparam>
        /// <returns></returns>
        public static TInterface CreateInstance<TInterface>()
            where TInterface : class, IMongoStroageBootstrap
        {
            if (dalInstanceTable == null || queryCacheTable == null) throw new Exception("bootstrap not created");

            var interType = typeof(TInterface);
            if (!interType.IsInterface) throw new Exception(interType.FullName + " is not interface");

            object value = null;
            //先从查询缓存中查找实例，若不存在则从实例表中查找
            if (!queryCacheTable.TryGetValue(interType, out value))
            {
                var kv = dalInstanceTable.FirstOrDefault(i => interType.IsAssignableFrom(i.Key));
                if (kv.Key != null)
                {
                    value = kv.Value;
                    queryCacheTable.AddOrUpdate(interType, value, (k, v) => value);
                }
            }

            if (value == null) throw new Exception("Could not find the '" + interType.FullName + "' implementation class instance");

            return value as TInterface;
        }
    }
}
