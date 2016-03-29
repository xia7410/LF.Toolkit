using LF.Toolkit.DataEngine;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.MongoDB
{
    public abstract class MongoStorageBase : IStorageBase
    {
        /// <summary>
        /// MongoDB存储基类
        /// </summary>
        /// <param name="config">存储配置</param>
        /// <param name="databaseName">当前存储类使用的数据库名称</param>
        /// <param name="collectionName">当前存储类使用的集合名称</param>
        public MongoStorageBase(IMongoStorageConfig config, string databaseName, string collectionName)
        {
            if(config == null) throw new ArgumentNullException("config");
            if (string.IsNullOrEmpty(databaseName)) throw new ArgumentNullException("databaseName");
            if (string.IsNullOrEmpty(collectionName)) throw new ArgumentNullException("collectionName");

            if (config.Databases.ContainsKey(databaseName))
            {
                var dbconfig = config.Databases[databaseName];
                //设置客户端配置
                Settings = new MongoClientSettings();
                if (config.ServerAddress.Count <= 1)
                {
                    Settings.Server = config.ServerAddress.First();
                }
                else
                {
                    Settings.Servers = config.ServerAddress;
                }
                //设置连接池最大连接个数
                if (config.MaxConnectionPoolSize > 0)
                {
                    Settings.MaxConnectionPoolSize = config.MaxConnectionPoolSize;
                }
                //设置认证
                if(!string.IsNullOrEmpty(dbconfig.Username) && !string.IsNullOrEmpty(dbconfig.Password))
                {
                    var credential = MongoCredential.CreateCredential(dbconfig.DatabaseName, dbconfig.Username, dbconfig.Password);
                    Settings.Credentials = new MongoCredential[] { credential };
                }

                //设置数据库名称
                this.DatabaseName = databaseName;
            }
            else
            {
                throw new Exception("Could not find the '" + databaseName + "' database config");
            }

            this.CollectionName = collectionName;
        }

        /// <summary>
        /// 获取客户端连接配置
        /// </summary>
        public MongoClientSettings Settings { get; private set; }

        /// <summary>
        /// 获取当前数据库名称
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// 获取当前集合名称
        /// </summary>
        public string CollectionName { get; private set; }

        /// <summary>
        /// 获取当前数据库实例
        /// </summary>
        /// <returns></returns>
        protected virtual IMongoDatabase GetDatabase()
        {
            var client = new MongoClient(Settings);
            return client.GetDatabase(DatabaseName);
        }

        /// <summary>
        /// 根据ObjectId获取一条文档记录
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="objectId">文档对象Id</param>
        /// <param name="projection">投影对象linq表达式</param>
        /// <returns></returns>
        public Task<TDocument> FindOneByIdAsync<TDocument>(ObjectId objectId, Expression<Func<TDocument, TDocument>> projection = null)
            where TDocument : MongoBsonId
        {
            Task<TDocument> doc = null;

            var collection = GetDatabase().GetCollection<TDocument>(CollectionName);
            var fileter = Builders<TDocument>.Filter.Eq<ObjectId>(i => i.Id, objectId);
            if (projection == null)
            {
                doc = collection.Find<TDocument>(i => i.Id.Equals(objectId)).FirstOrDefaultAsync();
            }
            else
            {
                doc = collection.Find<TDocument>(fileter).Project(projection).FirstOrDefaultAsync();
            }

            return doc;
        }

        /// <summary>
        /// 根据指定过滤条件获取一条文档记录
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">过滤条件linq表达式</param>
        /// <param name="projection">投影对象定义linq表达式</param>
        /// <returns></returns>
        public Task<TDocument> FindOneAsync<TDocument>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TDocument>> projection = null)
            where TDocument : new()
        {
            Task<TDocument> doc = null;
            var collection = GetDatabase().GetCollection<TDocument>(CollectionName);
            if (projection == null)
            {
                doc = collection.Find<TDocument>(filter).FirstOrDefaultAsync();
            }
            else
            {
                doc = collection.Find<TDocument>(filter).Project(projection).FirstOrDefaultAsync();
            }

            return doc;
        }

        /// <summary>
        /// 计算当前集合指定查询条件的文档个数
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">过滤条件</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<long> CountAsync<TDocument>(Expression<Func<TDocument, bool>> filter, CountOptions options = null)
        {
            var collection = GetDatabase().GetCollection<TDocument>(CollectionName);

            return collection.CountAsync<TDocument>(filter, options);
        }
    }
}
