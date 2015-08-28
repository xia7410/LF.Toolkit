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
        /// 当个BsonDocument容量最大字节值（16M）
        /// </summary>
        long MaxBsonDocumentSize { get; }

        /// <summary>
        /// MongoDB服务器地址集合
        /// </summary>
        IList<MongoServerAddress> ServerAddress { get;  }

        /// <summary>
        /// MongoDB数据库配置集合
        /// </summary>
        IDictionary<string, MongoDatabaseConfig> Databases { get;}

        /// <summary>
        /// 从配置文件载入配置
        /// </summary>
        /// <param name="path"></param>
        void LoadFrom(string path);
    }
}
