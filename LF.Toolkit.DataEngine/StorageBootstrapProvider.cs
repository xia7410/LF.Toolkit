using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LF.Toolkit.DataEngine
{
    public class StorageBootstrapProvider
    {
        /// <summary>
        /// 创建默认构造存储启动器
        /// </summary>
        /// <typeparam name="TStorageBase"></typeparam>
        /// <typeparam name="TStorageBootstrap"></typeparam>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IStorageBootstrap CreateBootstrap<TStorageBase, TStorageBootstrap>(Assembly assembly)
            where TStorageBase : class, IStorageBase
            where TStorageBootstrap : class, IStorageBootstrap, new()
        {
            var bootstrap = new TStorageBootstrap();
            bootstrap.CreateInstanceFrom<TStorageBase>(assembly);

            return bootstrap;
        }

        /// <summary>
        /// 创建泛型构造存储启动器
        /// </summary>
        /// <typeparam name="TStorageBase"></typeparam>
        /// <typeparam name="TStorageBootstrap"></typeparam>
        /// <typeparam name="TBootstrapParam"></typeparam>
        /// <param name="param"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IStorageBootstrap CreateBootstrap<TStorageBase, TStorageBootstrap ,TBootstrapParam>(TBootstrapParam param, Assembly assembly)
            where TStorageBase : class, IStorageBase
            where TBootstrapParam : class
            where TStorageBootstrap : StorageBootstrap<TBootstrapParam>
        {
            var bootType = typeof(TStorageBootstrap);
            var value = bootType.Assembly.CreateInstance(bootType.FullName, false, BindingFlags.Default | BindingFlags.CreateInstance
                        | BindingFlags.Instance | BindingFlags.Public, null, new object[] { param }, null, null);
            var bootstrap = value as IStorageBootstrap;
            bootstrap.CreateInstanceFrom<TStorageBase>(assembly);

            return bootstrap;
        }
    }
}
