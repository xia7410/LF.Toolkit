using LF.Toolkit.Singleton;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace LF.Toolkit.Data.Map
{
    public interface IDapperProvider : ISingleton
    {
        IDictionary<string, IDapperMapping> DapperMappings { get; }

        /// <summary>
        /// 映射配置,必须在使用映射前执行
        /// </summary>
        /// <param name="path"></param>
        void Configure(string path);
    }
}
