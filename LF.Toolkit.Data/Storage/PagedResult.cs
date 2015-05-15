using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LF.Toolkit.Data.Storage
{
    /// <summary>
    /// 表示分页查询结果类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResult<T> 
    {
        /// <summary>
        /// 获取或设置分页结果集合
        /// </summary>
        IEnumerable<T> Results { get; set; }

        /// <summary>
        /// 获取或设置分页查询记录总数
        /// </summary>
        public long Total { get; set; }
    }
}
