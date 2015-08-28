using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LF.Toolkit.DataEngine
{
    /// <summary>
    /// 表示数据库存储类接口标识
    /// </summary>
    public interface IStorageBase
    {
        /// <summary>
        /// 获取当前存储类型
        /// </summary>
        Type StorageType { get; }
    }
}
