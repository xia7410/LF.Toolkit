using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LF.Toolkit.DataEngine
{
    /// <summary>
    /// 表示存储启动接口
    /// </summary>
    public interface IStorageBootstrap
    {
        /// <summary>
        /// 从指定程序集载入指定存储基类的所有子类实例
        /// </summary>
        /// <typeparam name="TStorageBase">派生自IStorageBase的类型</typeparam>
        /// <param name="assembly">实现存储接口的程序集</param>
        void CreateInstanceFrom<TStorageBase>(Assembly assembly) where TStorageBase : class, IStorageBase;

        /// <summary>
        /// 创建指定接口类型的实例引用
        /// </summary>
        /// <typeparam name="TInterface">所有派生自IBootstrap的接口类型</typeparam>
        /// <returns></returns>
        TInterface CreateInstanceRef<TInterface>() where TInterface : class, IBootstrap;
    }
}
