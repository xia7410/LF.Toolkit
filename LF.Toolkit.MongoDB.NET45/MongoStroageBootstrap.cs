using LF.Toolkit.DataEngine;
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
    /// 表示MongoDB存储启动类
    /// </summary>
    public class MongoStroageBootstrap : StorageBootstrap<MongoStorageConfig>
    {
        MongoStorageConfig Config { get; set; }

        public MongoStroageBootstrap(MongoStorageConfig config)
        {
            if (config == null) throw new ArgumentNullException("config");

            this.Config = config;
        }

        /// <summary>
        /// 创建指定存储类型的实例
        /// </summary>
        /// <typeparam name="T">派生自IStorageBase的类型</typeparam>
        /// <returns></returns>
        public override object CreateInstance<T>()
        {
            var type = typeof(T);
            if (type.IsSubclassOf(typeof(MongoStorageBase)))
            {
                //查找当前类型是否含有ISqlMapping参数构造
                var constructor = type.GetConstructors().Where(i =>
                {
                    var parameters = i.GetParameters();
                    return parameters.Length == 1 && parameters[0].ParameterType == typeof(MongoStorageConfig);
                }).FirstOrDefault();
                //初始化含有ISqlMapping参数构造对象的实例
                if (constructor != null)
                {
                    return type.Assembly.CreateInstance(type.FullName, false, BindingFlags.Default | BindingFlags.CreateInstance
                            | BindingFlags.Instance | BindingFlags.Public, null, new object[] { Config }, null, null);
                }
            }

            return null;
        }
    }

}
