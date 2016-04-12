using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.MongoDB
{
    public interface IMongoStorageConfig
    {
        /// <summary>
        /// 获取单个BsonDocument容量最大字节值（16M）
        /// </summary>
        long MaxBsonDocumentSize { get; }

        /// <summary>
        /// 获取MongoDB服务器地址集合
        /// </summary>
        IReadOnlyList<MongoServerAddress> ServerAddress { get; }

        /// <summary>
        /// 获取连接池连接最大个数
        /// </summary>
        int MaxConnectionPoolSize { get; }

        /// <summary>
        /// 获取MongoDB数据库配置集合
        /// </summary>
        IReadOnlyDictionary<string, MongoDatabaseConfig> Databases { get; }
    }
}
