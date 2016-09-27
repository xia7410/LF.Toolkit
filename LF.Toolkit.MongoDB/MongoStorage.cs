using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace LF.Toolkit.MongoDB
{
    /// <summary>
    /// 表示简单的Mongo存储类
    /// </summary>
    public class MongoStorage : MongoStorageBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mongoUrl">mongo连接字符串</param>
        /// <param name="databaseName">mongo数据库名称</param>
        public MongoStorage(string mongoUrl, string databaseName)
            : base(mongoUrl, databaseName)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config">存储配置</param>
        /// <param name="databaseKey">当前存储类使用的数据库标识名称</param>
        public MongoStorage(IMongoStorageConfig config, string databaseKey)
            : base(config, databaseKey)
        {
        }
    }
}
