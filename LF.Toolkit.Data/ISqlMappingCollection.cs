using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LF.Toolkit.Data
{
    /// <summary>
    /// 表示Sql映射集合接口
    /// </summary>
    public interface ISqlMappingCollection
    {
        /// <summary>
        /// 获取指定关键字的Sql映射
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ISqlMapping this[string key] { get; }
    }
}
