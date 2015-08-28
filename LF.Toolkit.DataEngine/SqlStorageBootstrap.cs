﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LF.Toolkit.DataEngine
{
    /// <summary>
    /// 表示带有Sql存储映射的启动实现类
    /// </summary>
    public class SqlStorageBootstrap : StorageBootstrap<ISqlMappingCollection>
    {
        ISqlMappingCollection Collection { get; set; }

        public SqlStorageBootstrap(ISqlMappingCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            Collection = collection;
        }

        /// <summary>
        /// 创建指定存储类型的实例
        /// </summary>
        /// <typeparam name="T">派生自IStorageBase的类型</typeparam>
        /// <returns></returns>
        public override object CreateInstance<T>()
        {
            var type = typeof(T);
            //查找当前类型是否含有ISqlMapping参数构造
            var constructor = type.GetConstructors().Where(i =>
            {
                var parameters = i.GetParameters();
                return parameters.Length == 1 && typeof(ISqlMapping).IsAssignableFrom(parameters[0].ParameterType);
            }).FirstOrDefault(); ;
            //初始化含有ISqlMapping参数构造对象的实例
            if (constructor != null)
            {
                return type.Assembly.CreateInstance(type.FullName, false, BindingFlags.Default | BindingFlags.CreateInstance
                        | BindingFlags.Instance | BindingFlags.Public, null, new object[] { Collection[type.FullName] }, null, null);
            }

            return null;
        }
    }
}
