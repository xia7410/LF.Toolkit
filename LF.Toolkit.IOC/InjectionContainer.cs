using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LF.Toolkit.IOC
{
    /// <summary>
    /// 注入容器类
    /// </summary>
    public static class InjectionContainer
    {
        static readonly ContainerBuilder m_ContainerBuilder = new ContainerBuilder();
        static IContainer m_Container = null;

        /// <summary>
        /// Register components to be created through reflection.
        /// </summary>
        /// <typeparam name="T">base type</typeparam>
        /// <param name="assembly"></param>
        /// <param name="parameters"></param>
        public static void Register<T>(Assembly assembly, IDictionary<Type, object> parameters = null)
        {
            //return if continer is builded
            if (m_Container != null) return;

            var types = assembly.GetTypes().Where(i => i.IsClass && !i.IsAbstract && typeof(T).IsAssignableFrom(i));
            foreach (var t in types)
            {
                var attr = t.GetCustomAttribute<InjectableAttribute>(true);
                if (attr == null)
                {
                    continue;
                }
                Register(t, parameters: parameters, asSelf: attr.AsSelf, asImplementedInterfaces: attr.AsImplementedInterfaces, singleInstance: attr.SingleInstance);
            }
        }

        /// <summary>
        /// Register a component to be created through reflection.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        /// <param name="asSelf"></param>
        /// <param name="asImplementedInterfaces"></param>
        /// <param name="singleInstance"></param>
        public static void Register(Type type, IDictionary<Type, object> parameters = null, bool asSelf = true, bool asImplementedInterfaces = true, bool singleInstance = true)
        {
            //return if continer is builded
            if (m_Container != null) return;

            IEnumerable<Parameter> list = null;
            if (parameters != null)
            {
                list = parameters.Select(p => new TypedParameter(p.Key, p.Value));
            }
            var builder = m_ContainerBuilder.RegisterType(type);
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

        /// <summary>
        /// Create a new container with the component registrations that have been made.
        /// </summary>
        /// <param name="beforBuild"></param>
        /// <param name="afterBuild"></param>
        public static void Build(Action<ContainerBuilder> beforBuild = null, Action<IContainer> afterBuild = null)
        {
            //return if continer is builded
            if (m_Container != null) return;

            if (beforBuild != null)
            {
                beforBuild(m_ContainerBuilder);
            }
            m_Container = m_ContainerBuilder.Build();
            if (afterBuild != null)
            {
                afterBuild(m_Container);
            }
        }

        /// <summary>
        /// Retrieve a service from the context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
            where T : class
        {
            if (m_Container != null)
            {
                return m_Container.Resolve<T>();
            }

            return default(T);
        }
    }
}
