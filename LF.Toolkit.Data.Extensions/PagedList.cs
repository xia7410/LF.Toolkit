using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.Extensions
{
    /// <summary>
    /// 分页查询列表对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T>
    {
        /// <summary>
        /// 分页列表集合
        /// </summary>
        public IEnumerable<T> RowSet { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int Count { get; set; }
    }
}
