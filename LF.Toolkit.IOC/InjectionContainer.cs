using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LF.Toolkit.IOC
{
    /// <summary>
    /// 全局注入容器类
    /// </summary>
    public static class InjectionContainer
    {
        static readonly ContainerBuilder m_ContainerBuilder = new ContainerBuilder();
        static IContainer m_Container = null;

        /// <summary>
        /// 确认当前容器未初始
        /// </summary>
        /// <param name="action"></param>
        static void EnsureNotInitialize(Action action)
        {
            if (m_Container != null) throw new Exception("容器已经初始化");

            action();
        }

        /// <summary>
        /// 确认当前容器已经初始化
        /// </summary>
        /// <param name="func"></param>
        static T EnsureInitialize<T>(Func<T> func)
        {
            if (m_Container == null) throw new Exception("请先调用Build方法初始化容器");

            return func();
        }

        /// <summary>
        /// Register components to be created through reflection. which components is marked InjectableAttribute
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="parameters">实例化参数</param>
        public static void Register(Assembly assembly, IDictionary<Type, object> parameters = null)
            => EnsureNotInitialize(() =>
            {
                assembly.GetTypes().Where(i => i.IsDefined(typeof(InjectableAttribute), true))
                    .ToList().ForEach(t =>
                    {
                        var attr = t.GetCustomAttribute<InjectableAttribute>(true);
                        Register(t, parameters, attr.AsSelf, attr.AsImplementedInterfaces, attr.SingleInstance);
                    });
            });

        /// <summary>
        /// Register components to be created through reflection. which components is marked attributeType
        /// </summary>
        /// <param name="assemblyString"></param>
        /// <param name="attributeType"></param>
        /// <param name="parameters"></param>
        /// <param name="asSelf"></param>
        /// <param name="asImplementedInterfaces"></param>
        /// <param name="singleInstance"></param>
        public static void Register(string assemblyString, Type attributeType, IDictionary<Type, object> parameters = null, bool asSelf = true, bool asImplementedInterfaces = true, bool singleInstance = true)
            => EnsureNotInitialize(() => Register(Assembly.Load(assemblyString), attributeType, parameters, asSelf, asImplementedInterfaces, singleInstance));

        /// <summary>
        /// Register components to be created through reflection. which components is marked attributeType
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="attributeType"></param>
        /// <param name="parameters"></param>
        /// <param name="asSelf"></param>
        /// <param name="asImplementedInterfaces"></param>
        /// <param name="singleInstance"></param>
        public static void Register(Assembly assembly, Type attributeType, IDictionary<Type, object> parameters = null, bool asSelf = true, bool asImplementedInterfaces = true, bool singleInstance = true)
            => EnsureNotInitialize(() =>
            {
                assembly.GetTypes().Where(i => i.IsDefined(attributeType, true)).ToList().ForEach(t =>
                {
                    var attr = t.GetCustomAttribute(attributeType, true);
                    Register(t, parameters, asSelf, asImplementedInterfaces, singleInstance);
                });
            });

        /// <summary>
        /// Register components to be created through reflection. which components Inherited from T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyString">The long form of the assembly name.</param>
        /// <param name="parameters"></param>
        /// <param name="asSelf"></param>
        /// <param name="asImplementedInterfaces"></param>
        /// <param name="singleInstance"></param>
        public static void Register<T>(string assemblyString, IDictionary<Type, object> parameters = null, bool asSelf = true, bool asImplementedInterfaces = true, bool singleInstance = true)
            => EnsureNotInitialize(() => Register<T>(Assembly.Load(assemblyString), parameters, asSelf, asImplementedInterfaces, singleInstance));

        /// <summary>
        /// Register components to be created through reflection. which components Inherited from T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="parameters"></param>
        /// <param name="asSelf"></param>
        /// <param name="asImplementedInterfaces"></param>
        /// <param name="singleInstance"></param>
        public static void Register<T>(Assembly assembly, IDictionary<Type, object> parameters = null, bool asSelf = true, bool asImplementedInterfaces = true, bool singleInstance = true)
            => EnsureNotInitialize(() =>
            {
                assembly.GetTypes().Where(i => i.IsClass && !i.IsAbstract && typeof(T).IsAssignableFrom(i)).ToList().ForEach(t =>
                {
                    Register(t, parameters, asSelf, asImplementedInterfaces, singleInstance);
                });
            });

        /// <summary>
        /// Register a component to be created through reflection.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        /// <param name="asSelf"></param>
        /// <param name="asImplementedInterfaces"></param>
        /// <param name="singleInstance"></param>
        public static void Register(Type type, IDictionary<Type, object> parameters = null, bool asSelf = true, bool asImplementedInterfaces = true, bool singleInstance = true)
            => EnsureNotInitialize(() => InjectionContainerBuilder.Register(m_ContainerBuilder, type, parameters, asSelf, asImplementedInterfaces, singleInstance));

        /// <summary>
        /// Create a new container with the component registrations that have been made.
        /// </summary>
        /// <param name="beforBuild"></param>
        /// <param name="afterBuild"></param>
        public static void Build(Action<ContainerBuilder> beforBuild = null, Action<IContainer> afterBuild = null)
            => EnsureNotInitialize(() =>
            {
                beforBuild?.Invoke(m_ContainerBuilder);
                m_Container = m_ContainerBuilder.Build();
                afterBuild?.Invoke(m_Container);
            });

        /// <summary>
        /// Retrieve a service from the context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>() where T : class
            => EnsureInitialize<T>(() => m_Container.Resolve<T>());

        /// <summary>
        ///  Retrieve a service from the context.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Resolve(Type type)
             => EnsureInitialize<object>(() => m_Container.Resolve(type));

        /// <summary>
        /// 判断指定类型是否注册过
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsRegistered<T>()
            => EnsureInitialize<bool>(() => m_Container.IsRegistered<T>());

        /// <summary>
        /// 判断指定类型是否注册过
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsRegistered(Type type)
            => EnsureInitialize<bool>(() => m_Container.IsRegistered(type));
    }
}
