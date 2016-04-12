using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.MongoDB
{
    /// <summary>
    /// 表示带配置名称的mongo存储配置类
    /// </summary>
    public class NamedMongoStorageConfig
    {
        /// <summary>
        /// 获取或设置配置名称
        /// </summary>
        [JsonProperty("configName")]
        public string ConfigName { get; set; }

        /// <summary>
        /// 获取或设置存储配置
        /// </summary>
        [JsonProperty("mongoStorageConfig")]
        public MongoStorageConfig MongoStorageConfig { get; set; }
    }
}
