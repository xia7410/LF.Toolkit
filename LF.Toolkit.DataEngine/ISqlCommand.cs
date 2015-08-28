using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LF.Toolkit.DataEngine
{
    /// <summary>
    /// 表示Sql命令映射接口
    /// </summary>
    public interface ISqlCommand
    {
        /// <summary>
        /// 获取Sql命令的关键字
        /// </summary>
        string CommandKey { get; }

        /// <summary>
        /// 获取Sql命令的类型
        /// </summary>
        CommandType CommandType { get; }

        /// <summary>
        /// 获取Sql命令的文本
        /// </summary>
        string CommandText { get; }
    }
}
