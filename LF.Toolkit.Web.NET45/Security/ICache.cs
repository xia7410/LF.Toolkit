using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LF.Toolkit.Web.Security
{
    public interface ICache<T> where T : class , new()
    {
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        T Cache { get; }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="cache"></param>
        void SetCache(T cache);

        /// <summary>
        /// 存储缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        Task StoreCache(T cache);

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <returns></returns>
        Task ClearCache();
    }
}