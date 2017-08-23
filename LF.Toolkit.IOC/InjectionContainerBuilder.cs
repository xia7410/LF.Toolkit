using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.IOC
{
    /// <summary>
    /// 注入容器生成类
    /// </summary>
    public static class InjectionContainerBuilder
    {
        /// <summary>
        /// Register components to be created through reflection. which components is marked InjectableAttribute
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="func"></param>
        /// <param name="beforBuild"></param>
        /// <param name="afterBuild"></param>
        /// <returns></returns>
        public static IContainer Build(Assembly assembly, Func<Type, IDictionary<Type, object>> func = null, Action<ContainerBuilder> beforBuild = null)
        {
            var containerBuilder = new ContainerBuilder();
            var types = assembly.GetTypes().Where(i => i.IsClass && !i.IsAbstract);
            foreach (var t in types)
            {
                var attr = t.GetCustomAttribute<InjectableAttribute>(true);
                if (attr == null)
                {
                    continue;
                }
                Register(containerBuilder, t, parameters: func != null ? func.Invoke(t) : null, asSelf: attr.AsSelf, asImplementedInterfaces: attr.AsImplementedInterfaces, singleInstance: attr.SingleInstance);
            }

            if (beforBuild != null)
            {
                beforBuild.Invoke(containerBuilder);
            }

            return containerBuilder.Build();
        }

        /// <summary>
        /// Register components to be created through reflection. which components Inherited from T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="func"></param>
        /// <param name="asSelf"></param>
        /// <param name="asImplementedInterfaces"></param>
        /// <param name="singleInstance"></param>
        /// <param name="beforBuild"></param>
        /// <param name="afterBuild"></param>
        /// <returns></returns>
        public static IContainer Build<T>(Assembly assembly, Func<Type, IDictionary<Type, object>> func = null, bool asSelf = true, bool asImplementedInterfaces = true, bool singleInstance = true
            , Action<ContainerBuilder> beforBuild = null)
        {
            var containerBuilder = new ContainerBuilder();
            var types = assembly.GetTypes().Where(i => i.IsClass && !i.IsAbstract && typeof(T).IsAssignableFrom(i));
            foreach (var t in types)
            {
                Register(containerBuilder, t, parameters: func != null ? func.Invoke(t) : null);
            }

            if (beforBuild != null)
            {
                beforBuild.Invoke(containerBuilder);
            }

            return containerBuilder.Build();
        }

        /// <summary>
        /// Register a component to be created through reflection.
        /// </summary>
        /// <param name="containerBuilder"></param>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        /// <param name="asSelf"></param>
        /// <param name="asImplementedInterfaces"></param>
        /// <param name="singleInstance"></param>
        static void Register(ContainerBuilder containerBuilder, Type type, IDictionary<Type, object> parameters = null, bool asSelf = true, bool asImplementedInterfaces = true, bool singleInstance = true)
        {
            IEnumerable<Parameter> list = null;
            if (parameters != null)
            {
                list = parameters.Select(p => new TypedParameter(p.Key, p.Value));
            }
            var builder = containerBuilder.RegisterType(type);
            if (list != null)
            {
                builder = builder.WithParameters(list);
            }
            if (asSelf)
            {
                builder = builder.AsSelf();
            }
            if (asImplementedInterfaces)
            {
                builder = builder.AsImplementedInterfaces();
            }
            if (singleInstance)
            {
                builder = builder.SingleInstance();
            }
        }
    }
}
