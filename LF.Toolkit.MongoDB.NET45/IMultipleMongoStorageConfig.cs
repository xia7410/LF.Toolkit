using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.MongoDB
{
    public interface IMultipleMongoStorageConfig
    {
        /// <summary>
        /// 获取存储配置集合
        /// </summary>
        IReadOnlyDictionary<string , IMongoStorageConfig> MongoStorageConfigs { get; }

        /// <summary>
        /// 获取指定名称存储配置
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        IMongoStorageConfig GetMongoStorageConfig(string configName);
    }
}
