using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LF.Toolkit.Data
{
    /// <summary>
    /// 表示支持从存储启动创建接口实例的标识接口
    /// </summary>
    public interface ISqlStorageBootstrap
    {

    }

    public sealed class SqlStorageBootstrap
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
        /// 创建不带Sql存储映射的Bootstrap
        /// </summary>
        /// <param name="dalAssembly">数据库存储实现层程序集</param>
        public static void CreateBootstrap(Assembly dalAssembly)
        {
            if (dalAssembly == null) throw new ArgumentNullException("dalAssembly");

            //初始化相关配置
            Initialize();
            var baseType = typeof(SqlStorageBase);
            var types = dalAssembly.GetTypes().Where(i => i.IsSubclassOf(baseType) && !i.IsAbstract);
            if (types.Count() > 0)
            {
                foreach (var t in types)
                {
                    //查找当前类型是否含有无参构造
                    var constructor = t.GetConstructors().Where(i => i.GetParameters().Length == 0).FirstOrDefault();
                    //初始化无参构造对象的实例
                    if (constructor != null)
                    {
                        var instance = dalAssembly.CreateInstance(t.FullName, false, BindingFlags.Default | BindingFlags.CreateInstance
                                | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);
                        dalInstanceTable.AddOrUpdate(t, instance, (k, v) => instance);
                    }
                }
            }
            else
            {
                throw new Exception("Could not find the 'SqlStorageBase' implementation classes in dalAssembly");
            }
        }

        /// <summary>
        /// 创建带Sql存储映射的Bootstrap
        /// </summary>
        /// <param name="mapPath">sql映射文件目录</param>
        /// <param name="mappedDALAssembly">带映射的数据库存储实现层程序集</param>
        public static void CreateBootstrap(string mapPath, Assembly mappedDALAssembly)
        {
            if (string.IsNullOrEmpty(mapPath)) throw new ArgumentNullException("mapPath");
            if (mappedDALAssembly == null) throw new ArgumentNullException("mappedDALAssembly");

            //初始化相关配置
            Initialize();
            //初始化sql映射文件集合
            var provider = new SqlMappingProvider(mapPath);
            var baseType = typeof(SqlMappingStorageBase);
            var types = mappedDALAssembly.GetTypes().Where(i => i.IsSubclassOf(baseType));
            if (types.Count() > 0)
            {
                foreach (var t in types)
                {
                    //查找当前类型是否含有ISqlMapping参数构造
                    var constructor = t.GetConstructors().Where(i =>
                    {
                        var parameters = i.GetParameters();
                        return parameters.Length == 1 && parameters[0].ParameterType == typeof(ISqlMapping);
                    }).FirstOrDefault(); ;
                    //初始化含有ISqlMapping参数构造对象的实例
                    if (constructor != null)
                    {
                        //获取映射对象
                        var mapping = provider.GetSqlMapping(t.FullName);
                        var instance = mappedDALAssembly.CreateInstance(t.FullName, false, BindingFlags.Default | BindingFlags.CreateInstance
                            | BindingFlags.Instance | BindingFlags.Public, null, new object[] { mapping }, null, null);
                        dalInstanceTable.AddOrUpdate(t, instance, (k, v) => instance);
                    }
                }
            }
            else
            {
                throw new Exception("Could not find the 'SqlMappingStorageBase' implementation classes in mappedDALAssembly");
            }
        }

        /// <summary>
        /// 创建指定类型存储接口的实例
        /// </summary>
        /// <typeparam name="TInterface">继承ISqlStorageBootstrap的存储接口</typeparam>
        /// <returns></returns>
        public static TInterface CreateInstance<TInterface>()
            where TInterface : class, ISqlStorageBootstrap
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
