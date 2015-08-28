using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.MongoDB
{
    public class MongoDatabaseConfig
    {
        /// <summary>
        /// 获取或设置数据库名称
        /// </summary>
        [JsonProperty("databaseName")]
        public string DatabaseName { get; set; }

        /// <summary>
        /// 获取或设置用户名称
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// 获取或设置密码
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
