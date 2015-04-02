using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.MongoDB.Config
{
    public class MongoStorageConfig
    {
        /// <summary>
        /// 获取或设置服务器地址列表
        /// </summary>
        [JsonProperty("serverAddress")]
        public List<string> AddressList { get; set; }

        /// <summary>
        /// 获取或设置数据库配置列表
        /// </summary>
        [JsonProperty("databases")]
        public List<MongoDatabaseConfig> DatabaseList { get; set; }

        /// <summary>
        /// 当个BsonDocument容量最大字节值（16M）
        /// </summary>
        [JsonIgnore]
        public static long MaxBsonDocumentSize { get { return 16777216; } }

        /// <summary>
        /// MongoDB服务器地址集合
        /// </summary>
        [JsonIgnore]
        public static IList<MongoServerAddress> ServerAddress { get; private set; }

        /// <summary>
        /// MongoDB数据库配置集合
        /// </summary>
        [JsonIgnore]
        public static IDictionary<string, MongoDatabaseConfig> Databases { get; private set; }

        /// <summary>
        /// 从json文件加载配置
        /// </summary>
        /// <param name="filename"></param>
        public static void FromJson(string filename)
        {
            if (File.Exists(filename))
            {
                using (var sw = File.OpenText(filename))
                {
                    var cfg = JsonConvert.DeserializeObject<MongoStorageConfig>(sw.ReadToEnd());
                    if (cfg != null)
                    {
                        if (cfg.AddressList == null || cfg.AddressList.Count <= 0) throw new Exception("serverAddress not configure");
                        if (cfg.DatabaseList == null || cfg.DatabaseList.Count <= 0) throw new Exception("databases not configure");

                        ServerAddress = cfg.AddressList.Select(i => MongoServerAddress.Parse(i)).ToList();
                        Databases = cfg.DatabaseList.ToDictionary(i => i.DatabaseName, i => i);
                    }
                    else
                    {
                        throw new Exception("不正确的MongoDB数据库配置文件");
                    }
                }
            }
            else
            {
                throw new Exception("未找到MongoDB数据库存储配置");
            }
        }
    }
}
