using LF.Toolkit.DataEngine;
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
    public class MongoStroageBootstrap : StorageBootstrap<IMongoStorageConfig>
    {
        IMongoStorageConfig Config { get; set; }

        public MongoStroageBootstrap(IMongoStorageConfig config)
        {
            if (config == null) throw new ArgumentNullException("config");

            this.Config = config;
        }

        /// <summary>
        /// 创建指定存储类型的实例
        /// </summary>
        /// <typeparam name="T">派生自IStorageBase的类型</typeparam>
        /// <returns></returns>
        protected override object CreateInstance<T>()
        {
            var type = typeof(T);
            //查找当前类型是否含有MongoStorageConfig参数构造器
            var constructor = type.GetConstructors().Where(i => i.GetParameters().Length == 1
                && typeof(IMongoStorageConfig).IsAssignableFrom(i.GetParameters()[0].ParameterType)).FirstOrDefault();

            if (constructor != null)
            {
                return type.Assembly.CreateInstance(type.FullName, false, BindingFlags.Default | BindingFlags.CreateInstance
                        | BindingFlags.Instance | BindingFlags.Public, null, new object[] { Config }, null, null);
            }

            return null;
        }
    }

}
