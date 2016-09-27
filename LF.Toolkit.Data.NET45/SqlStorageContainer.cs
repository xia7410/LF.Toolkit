using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LF.Toolkit.Data
{
    public static class SqlStorageContainer
    {
        /// <summary>
        /// 注册Sql存储实现类
        /// </summary>
        /// <param name="assembly">含有Sql存储的实现类程序集</param>
        /// <returns></returns>
        public static IContainer Register(Assembly assembly)
        {
            var baseType = typeof(SqlStorageBase);
            var types = assembly.GetTypes().Where(i => i.IsClass && baseType.IsAssignableFrom(i));
            var build = new ContainerBuilder();
            foreach (var t in types)
            {
                //注册本类及其继承的接口
                build.RegisterType(t).AsSelf().AsImplementedInterfaces().SingleInstance();
            }

            return build.Build();
        }

        /// <summary>
        /// 注册Sql存储实现类
        /// </summary>
        /// <param name="maps">Sql存储映射目录</param>
        /// <param name="assembly">含有Sql存储的实现类程序集</param>
        /// <returns></returns>
        public static IContainer Register(string maps, Assembly assembly)
        {
            ISqlMappingCollection collection = SqlMappingCollection.LoadFrom(maps);
            return Register(collection, assembly);
        }

        /// <summary>
        /// 注册Sql存储实现类
        /// </summary>
        /// <param name="collection">Sql存储映射集合</param>
        /// <param name="assembly">含有Sql存储的实现类程序集</param>
        /// <returns></returns>
        public static IContainer Register(ISqlMappingCollection collection, Assembly assembly)
        {
            var baseType = typeof(SqlStorageBase);
            var types = assembly.GetTypes().Where(i => i.IsClass && baseType.IsAssignableFrom(i));
            var build = new ContainerBuilder();
            foreach (var t in types)
            {
                //获取指定实例化构造器
                var c = t.GetConstructor(new Type[] { typeof(ISqlMapping) });
                if (c == null)
                {
                    c = t.GetConstructor(new Type[] { typeof(SqlMapping) });
                }
                if (c == null) throw new Exception("未找到指定类型 " + t.FullName + " 的带有ISqlMapping参数的构造函数");
                //获取构造器参数信息
                var p = c.GetParameters()[0];
                var mapping = collection[t.FullName];
                //注册本类及其继承的接口
                build.RegisterType(t).WithParameter(p.Name, mapping).AsSelf().AsImplementedInterfaces().SingleInstance();
            }

            return build.Build();
        }
    }
}
