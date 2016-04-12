using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.MongoDB
{
    /// <summary>
    /// 表示多个存储配置集合类
    /// </summary>
    public class MultipleMongoStorageConfig : IMultipleMongoStorageConfig
    {
        /// <summary>
        /// 获取存储配置字典集合
        /// </summary>
        public IReadOnlyDictionary<string, IMongoStorageConfig> MongoStorageConfigs { get; private set; }

        public MultipleMongoStorageConfig(string path)
        {
            LoadFrom(path);
        }

        void LoadFrom(string path)
        {
            if (File.Exists(path))
            {
                using (var sw = File.OpenText(path))
                {
                    var listConfig = JsonConvert.DeserializeObject<List<NamedMongoStorageConfig>>(sw.ReadToEnd());
                    if (listConfig != null)
                    {
                        foreach (var namedConfig in listConfig)
                        {
                            if (string.IsNullOrEmpty(namedConfig.ConfigName)) throw new Exception("configName not configure");

                            var config = namedConfig.MongoStorageConfig;
                            if (config.AddressList == null || config.AddressList.Count <= 0) throw new Exception("serverAddress not configure");
                            if (config.DatabaseList == null || config.DatabaseList.Count <= 0) throw new Exception("databases not configure");

                            config.ServerAddress = config.AddressList.Select(i => MongoServerAddress.Parse(i)).ToList();
                            config.Databases = config.DatabaseList.ToDictionary(i => i.DatabaseKey, i => i);
                            config.MaxConnectionPoolSize = config.MaxConnectionPoolSize;
                        }

                        MongoStorageConfigs = listConfig.ToDictionary(i => i.ConfigName, i => i.MongoStorageConfig as IMongoStorageConfig);
                    }
                    else
                    {
                        throw new Exception("Invalid mongodb config file");
                    }
                }
            }
            else
            {
                throw new Exception("Could not find mongodb config file");
            }
        }

        /// <summary>
        /// 获取指定名称的存储配置
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public IMongoStorageConfig GetMongoStorageConfig(string configName)
        {
            if (MongoStorageConfigs != null)
            {
                if (MongoStorageConfigs.ContainsKey(configName))
                {
                    return MongoStorageConfigs[configName];
                }
            }

            return null;
        }
    }
}
