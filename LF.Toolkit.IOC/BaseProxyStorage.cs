using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.IOC
{
    /// <summary>
    /// 代理存储接口标识
    /// </summary>
    public interface IProxyStorage
    {
        /// <summary>
        /// 存储实现类容器实例
        /// </summary>
        IContainer Container { get; }
    }

    /// <summary>
    /// 代理存储基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseProxyStorage<T> : IProxyStorage
    {
        /// <summary>
        /// 获取当前代理的存储类实例
        /// </summary>
        public T ProxyStorage { get; private set; }

        /// <summary>
        /// 获取存储类容器
        /// </summary>
        public IContainer Container { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container">包含实际存储类的容器</param>
        public BaseProxyStorage(IContainer container)
        {
            Container = container;
            ProxyStorage = container.Resolve<T>();
        }
    }

    /// <summary>
    /// 带有缓存的代理存储基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseMemoryCacheProxyStorage<T> : BaseProxyStorage<T>
    {
        readonly MemoryCache m_Cached;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container">包含实际存储类的容器</param>
        public BaseMemoryCacheProxyStorage(IContainer container)
            : base(container)
        {
            m_Cached = new MemoryCache(this.GetType().FullName + "." + Guid.NewGuid().ToString("N"));
        }

        /// <summary>
        /// 使用键和值将某个缓存项插入缓存中，并指定基于时间的过期详细信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiration">若为空则设置为最大值</param>
        protected void SetCache(string key, object value, TimeSpan? expiration = null)
        {
            if (!string.IsNullOrEmpty(key) && value != null)
            {
                var offset = DateTimeOffset.MinValue;
                if (expiration.HasValue && expiration.Value > TimeSpan.Zero)
                {
                    offset = DateTimeOffset.Now.Add(expiration.Value);
                }
                else
                {
                    offset = DateTimeOffset.MaxValue;
                }

                m_Cached.Set(key, value, offset);
            }
        }

        /// <summary>
        /// 获取指定名称的缓存对象
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool TryGetCache<K>(string key, out K value)
        {
            var item = m_Cached.Get(key);
            if (item != null && typeof(K).IsAssignableFrom(item.GetType()))
            {
                value = (K)item;
                return true;
            }
            else
            {
                value = default(K);
                return false;
            }
        }

        /// <summary>
        /// 获取缓存中是否包含指定键名的缓存项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool Contians(string key)
        {
            return m_Cached.Contains(key);
        }

        /// <summary>
        /// 删除指定名称的缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool RemoveCache(string key)
        {
            var obj = m_Cached.Remove(key);

            return obj != null;
        }

        /// <summary>
        /// 获取当前类的只读的缓存对象实例
        /// </summary>
        /// <returns></returns>
        protected MemoryCache GetReadOnlyCacheInstance()
        {
            return this.m_Cached;
        }
    }
}
