using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LF.Toolkit.Misc.Cache
{
    public class MemoryCached
    {
        static readonly ConcurrentDictionary<string, CacheValue> majorCached;
        static readonly ConcurrentDictionary<string, DateTime> timeCached;
        static volatile bool jobDone = true;

        static MemoryCached()
        {
            majorCached = new ConcurrentDictionary<string, CacheValue>();
            timeCached = new ConcurrentDictionary<string, DateTime>();

            Timer timer = new Timer(1000);
            timer.Elapsed += (sender, e) =>
            {
                if (!timeCached.IsEmpty)
                {
                    if (!jobDone) return;

                    var expires = timeCached.Where(i => i.Value <= DateTime.Now);
                    if (expires.Count() > 0)
                    {
                        jobDone = false;
                        expires.AsParallel().ForAll(kv =>
                        {
                            CacheValue cv;
                            majorCached.TryRemove(kv.Key, out cv);
                            DateTime dt;
                            timeCached.TryRemove(kv.Key, out dt);
                        });
                        jobDone = true;
                    }
                }
            };

            timer.Start();
        }

        /// <summary>
        /// 设置键的过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public static bool KeyExpire(string key, DateTime expiry)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            bool success = false;
            if (DateTime.Now < expiry && timeCached.ContainsKey(key))
            {
                timeCached[key] = expiry;
                success = true;
            }

            return success;
        }

        /// <summary>
        /// 设置键的过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public static bool KeyExpire(string key, TimeSpan expiry)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            bool success = false;
            if (expiry.TotalSeconds > 0 && timeCached.ContainsKey(key))
            {
                timeCached[key] = DateTime.Now.Add(expiry);
                success = true;
            }

            return success;
        }

        /// <summary>
        /// 设置指定键名值的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Set(string key, object value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");

            var cv = new CacheValue { Value = value };
            majorCached.AddOrUpdate(key, cv, (k, v) => cv);

            return true;
        }

        /// <summary>
        /// 设置指定键名值的缓存并附带过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public static bool Set(string key, object value, DateTime expiry)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");

            var cv = new CacheValue { Value = value, Expiry = expiry };
            timeCached.AddOrUpdate(key, expiry, (k, v) => expiry);
            majorCached.AddOrUpdate(key, cv, (k, v) => cv);

            return true;
        }

        /// <summary>
        /// 设置指定键名值的缓存并附带过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public static bool Set(string key, object value, TimeSpan expiry)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");

            if (expiry <= TimeSpan.Zero)
            {
                return false;
            }

            return Set(key, value, DateTime.Now.Add(expiry));
        }

        /// <summary>
        /// 获取指定键名的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static CacheValue Get(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            CacheValue cv = null;

            if (majorCached.ContainsKey(key))
            {
                if (majorCached.TryGetValue(key, out cv))
                {
                    if (cv.Expiry.HasValue)
                    {
                        if (cv.Expiry.Value - DateTime.Now <= TimeSpan.Zero)
                        {
                            if (majorCached.TryRemove(key, out cv))
                            {
                                cv = null;
                            }
                        }
                    }
                }
            }

            return cv;
        }

        /// <summary>
        /// 获取指定键名的缓存并转化为指定类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetAs<T>(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            var result = default(T);
            var cv = Get(key);
            if (cv != null)
            {
                result = (T)cv.Value;
            }

            return result;
        }

        /// <summary>
        /// 删除指定键名的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool KeyDelete(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            bool success = false;
            if (majorCached.ContainsKey(key))
            {
                CacheValue cv;
                success = majorCached.TryRemove(key, out cv);
            }

            if (timeCached.ContainsKey(key))
            {
                DateTime dt;
                timeCached.TryRemove(key, out dt);
            }

            return success;
        }

    }
}
