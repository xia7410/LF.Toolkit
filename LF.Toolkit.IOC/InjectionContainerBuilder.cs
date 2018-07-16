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
        internal static IContainer InternalBuild(Action<ContainerBuilder> action, Action<ContainerBuilder> beforBuild = null)
        {
            var containerBuilder = new ContainerBuilder();
            action(containerBuilder);
            beforBuild?.Invoke(containerBuilder);

            return containerBuilder.Build();
        }

        /// <summary>
        /// Create a new container with the component registrations (which components is marked InjectableAttribute) that have been made. 
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="parameters"></param>
        /// <param name="beforBuild"></param>
        /// <returns></returns>
        public static IContainer Build(Assembly assembly, IDictionary<Type, object> parameters = null, Action<ContainerBuilder> beforBuild = null)
            => InternalBuild(containerBuilder =>
            {
                assembly.GetTypes().Where(i => i.IsDefined(typeof(InjectableAttribute), true)).ToList().ForEach(t =>
                {
                    var attr = t.GetCustomAttribute<InjectableAttribute>(true);
                    Register(containerBuilder, t, parameters, attr.AsSelf, attr.AsImplementedInterfaces, attr.SingleInstance);
                });
            }, beforBuild);

        /// <summary>
        /// Create a new container with the component registrations (which components Inherited from T) that have been made.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyString"></param>
        /// <param name="parameters"></param>
        /// <param name="asSelf"></param>
        /// <param name="asImplementedInterfaces"></param>
        /// <param name="singleInstance"></param>
        /// <param name="beforBuild"></param>
        /// <returns></returns>
        public static IContainer Build<T>(string assemblyString, IDictionary<Type, object> parameters = null, bool asSelf = true, bool asImplementedInterfaces = true, bool singleInstance = true
            , Action<ContainerBuilder> beforBuild = null)
            => InternalBuild(containerBuilder =>
            {
                Assembly.Load(assemblyString).GetTypes().Where(i => i.IsClass && !i.IsAbstract && typeof(T).IsAssignableFrom(i)).ToList().ForEach(t =>
                {
                    Register(containerBuilder, t, parameters, asSelf, asImplementedInterfaces, singleInstance);
                });
            }, beforBuild);

        /// <summary>
        /// Create a new container with the component registrations (which components Inherited from T) that have been made.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="parameters"></param>
        /// <param name="asSelf"></param>
        /// <param name="asImplementedInterfaces"></param>
        /// <param name="singleInstance"></param>
        /// <param name="beforBuild"></param>
        /// <returns></returns>
        public static IContainer Build<T>(Assembly assembly, IDictionary<Type, object> parameters = null, bool asSelf = true, bool asImplementedInterfaces = true, bool singleInstance = true
            , Action<ContainerBuilder> beforBuild = null)
            => InternalBuild(containerBuilder =>
            {
                assembly.GetTypes().Where(i => i.IsClass && !i.IsAbstract && typeof(T).IsAssignableFrom(i)).ToList().ForEach(t =>
                {
                    Register(containerBuilder, t, parameters, asSelf, asImplementedInterfaces, singleInstance);
                });
            }, beforBuild);

        /// <summary>
        /// Register a component to be created through reflection.
        /// </summary>
        /// <param name="containerBuilder"></param>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        /// <param name="asSelf"></param>
        /// <param name="asImplementedInterfaces"></param>
        /// <param name="singleInstance"></param>
        internal static void Register(ContainerBuilder containerBuilder, Type type, IDictionary<Type, object> parameters = null, bool asSelf = true, bool asImplementedInterfaces = true, bool singleInstance = true)
        {
            var builder = containerBuilder.RegisterType(type).IfNotRegistered(type);
            var list = parameters?.Select(p => new TypedParameter(p.Key, p.Value));
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
