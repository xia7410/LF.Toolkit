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
        /// 创建无参构造存储启动器
        /// </summary>
        /// <typeparam name="TStorageBase">存储基类</typeparam>
        /// <typeparam name="TStorageBootstrap">存储启动实现类</typeparam>
        /// <param name="assembly">存储实现程序集</param>
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
        /// 创建带有一个参数构造存储启动器
        /// </summary>
        /// <typeparam name="TStorageBase">存储基类</typeparam>
        /// <typeparam name="TStorageBootstrap">存储启动实现类</typeparam>
        /// <typeparam name="TBootstrapArgument">存储启动类构造参数类型</typeparam>
        /// <param name="arg">存储启动类构造参数值</param>
        /// <param name="assembly">存储实现程序集</param>
        /// <returns></returns>
        public static IStorageBootstrap CreateBootstrap<TStorageBase, TStorageBootstrap ,TBootstrapArgument>(TBootstrapArgument arg, Assembly assembly)
            where TStorageBase : class, IStorageBase
            where TStorageBootstrap : StorageBootstrap<TBootstrapArgument>
        {
            var bootType = typeof(TStorageBootstrap);
            var value = bootType.Assembly.CreateInstance(bootType.FullName, false, BindingFlags.Default | BindingFlags.CreateInstance
                        | BindingFlags.Instance | BindingFlags.Public, null, new object[] { arg }, null, null);
            var bootstrap = value as IStorageBootstrap;
            bootstrap.CreateInstanceFrom<TStorageBase>(assembly);

            return bootstrap;
        }
    }
}
