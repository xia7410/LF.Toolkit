using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LF.Toolkit.Data.Map
{
    public sealed class DapperProvider<T> where T : IDapperProvider
    {
        private static readonly Lazy<T> lazyInstance = new Lazy<T>(() =>
        {
            Assembly asm = typeof(T).Assembly;
            return (T)asm.CreateInstance(typeof(T).FullName, false, BindingFlags.CreateInstance | BindingFlags.Instance
                | BindingFlags.Public, null, null, null, null);
        }, true);

        public static T Factory { get { return lazyInstance.Value; } }
    }
}
