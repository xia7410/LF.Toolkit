using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LF.Toolkit.Data
{
    /// <summary>
    ///表示Sql映射接口
    /// </summary>
    public interface ISqlMapping
    {
        /// <summary>
        /// 获取Sql映射对应的类型完全类型名称
        /// </summary>
        string Type { get; }

        /// <summary>
        /// 获取指定的数据库连接关键字
        /// </summary>
        string ConnectionKey { get; }

        /// <summary>
        /// 获取指定的Sql命令
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ISqlCommand this[string key] { get; }
    }
}
