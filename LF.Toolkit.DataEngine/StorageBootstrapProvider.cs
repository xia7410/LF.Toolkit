using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LF.Toolkit.DataEngine
{
    public class StorageBootstrapProvider
    {
        public static IStorageBootstrap CreateBootstrap<TStorageBase, TStorageBootstrap>(Assembly assembly)
            where TStorageBase : class, IStorageBase
            where TStorageBootstrap : class, IStorageBootstrap, new()
        {
            var bootstrap = new TStorageBootstrap();
            bootstrap.CreateInstanceFrom<TStorageBase>(assembly);

            return bootstrap;
        }

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
