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
        /// <param name="config">存储配置</param>
        /// <param name="databaseKey">当前存储类使用的数据库标识名称</param>
        public MongoStorage(IMongoStorageConfig config, string databaseKey)
            : base(config, databaseKey)
        {

        }

        /// <summary>
        /// 获取指定名称的集合
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="name">集合名称</param>
        /// <param name="settings">集合设置</param>
        /// <returns></returns>
        public IMongoCollection<TDocument> GetCollection<TDocument>(string name, MongoCollectionSettings settings = null)
        {
            return base.GetDatabase().GetCollection<TDocument>(name, settings);
        }
    }
}
