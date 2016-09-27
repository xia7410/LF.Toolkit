using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.MongoDB
{
    public static class MongoStorageContainer
    {
        /// <summary>
        /// 注册Mongo存储实现
        /// </summary>
        /// <param name="configs">存储配置文件路径</param>
        /// <param name="configName">存储配置名称</param>
        /// <param name="assembly">含有Mongo存储的实现类程序集</param>
        /// <returns></returns>
        public static IContainer Register(string configs, string configName, Assembly assembly)
        {
            return Register(MongoStorageConfigs.LoadFrom(configs), configName, assembly);
        }

        /// <summary>
        /// 注册Mongo存储实现
        /// </summary>
        /// <param name="configs">存储配置实例</param>
        /// <param name="configName">存储配置名称</param>
        /// <param name="assembly">含有Mongo存储的实现类程序集</param>
        /// <returns></returns>
        public static IContainer Register(MongoStorageConfigs configs, string configName, Assembly assembly)
        {
            IMongoStorageConfig config = null;
            if (!configs.StorageConfigs.TryGetValue(configName, out config)) throw new Exception("未找到指定名称 " + configName + " 的存储配置");

            var baseType = typeof(MongoStorageBase);
            var types = assembly.GetTypes().Where(i => i.IsClass && baseType.IsAssignableFrom(i));
            var build = new ContainerBuilder();
            foreach (var t in types)
            {
                //获取指定实例化构造器
                var c = t.GetConstructor(new Type[] { typeof(IMongoStorageConfig) });
                if (c == null)
                {
                    c = t.GetConstructor(new Type[] { typeof(MongoStorageConfig) });
                }
                if (c == null) throw new Exception("未找到指定类型 " + t.FullName + " 的带有IMongoStorageConfig参数的构造函数");
                //获取构造器参数信息
                var p = c.GetParameters()[0];
                //注册本类及其继承的接口
                build.RegisterType(t).WithParameter(p.Name, config).AsSelf().AsImplementedInterfaces().SingleInstance();
            }

            return build.Build();
        }
    }
}
