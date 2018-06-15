using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Reflection;

namespace LF.Toolkit.IOC
{
    /// <summary>
    /// 代理存储接口标识
    /// </summary>
    public interface IProxyStorage
    {
        /// <summary>
        /// Autofac存储实现类容器实例
        /// </summary>
        IContainer Container { get; }
    }

    /// <summary>
    /// 代理存储基类
    /// </summary>
    /// <typeparam name="T">缓存类类型</typeparam>
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
    /// <typeparam name="T">缓存类类型</typeparam>
    public abstract class BaseMemoryCacheProxyStorage<T> : BaseProxyStorage<T>
    {
        readonly MemoryCache m_Cached;
        static readonly ManualResetEventSlim m_Locker = new ManualResetEventSlim(true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container">包含实际存储类的容器</param>
        public BaseMemoryCacheProxyStorage(IContainer container)
            : base(container)
        {
            m_Cached = new MemoryCache(this.GetType().FullName + "-" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// 使用键和值将某个缓存项插入缓存中，并指定基于时间的过期详细信息
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="value">缓存值</param>
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
        /// <typeparam name="TSource">对象类型</typeparam>
        /// <param name="key">缓存键名</param>
        /// <param name="value">缓存值</param>
        /// <returns></returns>
        protected bool TryGetCache<TSource>(string key, out TSource value)
        {
            var item = m_Cached.Get(key);
            if (item != null && typeof(TSource).IsAssignableFrom(item.GetType()))
            {
                value = (TSource)item;
                return true;
            }
            else
            {
                value = default(TSource);
                return false;
            }
        }

        /// <summary>
        /// 获取缓存中是否包含指定键名的缓存项
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <returns></returns>
        protected bool Contians(string key)
        {
            return m_Cached.Contains(key);
        }

        /// <summary>
        /// 删除指定名称的缓存对象
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <returns></returns>
        protected bool RemoveCache(string key)
        {
            var obj = m_Cached.Remove(key);

            return obj != null;
        }

        /// <summary>
        /// 获取当前类的缓存对象实例
        /// </summary>
        /// <returns></returns>
        protected MemoryCache GetCacheInstance()
        {
            return this.m_Cached;
        }

        #region 列表缓存操作相关

        /// <summary>
        /// 将指定对象添加到对象列表缓存中，若添加不成功则返回false
        /// 【此操作是线程安全的】
        /// </summary>
        /// <typeparam name="TSource">对象类型</typeparam>
        /// <param name="key">列表缓存的键名</param>
        /// <param name="value">缓存类型对象</param>
        /// <param name="predicate">删除缓存中与当前对象匹配的条件</param>
        /// <param name="expires">缓存过期时间</param>
        /// <param name="waitSeconds">线程锁等待时间</param>
        protected bool AddToListCache<TSource>(string key, TSource value, Predicate<TSource> predicate, TimeSpan expires, int waitSeconds = 30)
        {
            bool success = false;
            if (waitSeconds < 0)
            {
                waitSeconds = 30;
            }
            if (m_Locker.Wait(TimeSpan.FromSeconds(waitSeconds)))
            {
                try
                {
                    List<TSource> list = null;
                    if (!this.TryGetCache(key, out list))
                    {
                        list = new List<TSource>();
                    }
                    else
                    {
                        //移除所有存在的对象
                        list.RemoveAll(predicate);
                    }
                    list.Add(value);
                    //设置缓存
                    this.SetCache(key, list, expires);
                    success = true;
                }
                finally
                {
                    m_Locker.Set();
                }
            }

            return success;
        }

        /// <summary>
        /// 移除与指定的谓词所定义的条件相匹配的所有元素。
        /// 【此操作是线程安全的】
        /// </summary>
        /// <typeparam name="TSource">对象类型</typeparam>
        /// <param name="key">列表缓存的键名</param>
        /// <param name="predicate">删除查找条件</param>
        /// <param name="expires">缓存过期时间</param>
        /// <param name="waitSeconds">线程锁等待时间</param>
        protected bool RemoveAllFromListCache<TSource>(string key, Predicate<TSource> predicate, TimeSpan expires, int waitSeconds = 30)
        {
            bool success = false;
            if (waitSeconds < 0)
            {
                waitSeconds = 30;
            }
            if (m_Locker.Wait(TimeSpan.FromSeconds(waitSeconds)))
            {
                try
                {
                    List<TSource> list = null;
                    if (this.TryGetCache(key, out list))
                    {
                        //移除所有存在的对象
                        int count = list.RemoveAll(predicate);
                        if (count > 0)
                        {
                            this.SetCache(key, list, expires);
                            success = true;
                        }
                    }
                }
                finally
                {
                    m_Locker.Set();
                }
            }

            return success;
        }

        /// <summary>
        /// 从对象列表缓存中获取符合指定条件的第一个对象
        /// 【此操作是线程安全的】
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="key">列表缓存的键名</param>
        /// <param name="predicate">查找条件</param>
        /// <param name="waitSeconds">线程锁等待时间</param>
        /// <returns></returns>
        protected TSource FindOneFromListCache<TSource>(string key, Func<TSource, bool> predicate, int waitSeconds = 30)
        {
            TSource value = default(TSource);
            if (waitSeconds < 0)
            {
                waitSeconds = 30;
            }
            if (m_Locker.Wait(TimeSpan.FromSeconds(waitSeconds)))
            {
                try
                {
                    List<TSource> list = null;
                    if (this.TryGetCache(key, out list))
                    {
                        value = list.FirstOrDefault(predicate);
                    }
                }
                finally
                {
                    m_Locker.Set();
                }
            }

            return value;
        }

        #endregion
    }
}
