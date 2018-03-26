using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.Dapper
{
    /// <summary>
    /// 简单分页列表类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T>
    {
        /// <summary>
        /// 获取或设置分页列表集合
        /// </summary>
        public IEnumerable<T> RowSet { get; set; }

        /// <summary>
        /// 获取或设置分页总条数
        /// </summary>
        public int Count { get; set; }
    }
}
