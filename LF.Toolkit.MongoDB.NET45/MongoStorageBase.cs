using LF.Toolkit.MongoDB.Config;
using LF.Toolkit.Singleton;
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
    public abstract class MongoStorageBase : ISingleton
    {
        /// <summary>
        /// MongoDB存储基类
        /// </summary>
        /// <param name="databaseName">当前存储类使用的数据库名称</param>
        /// <param name="collectionName">当前存储类使用的集合名称</param>
        public MongoStorageBase(string databaseName, string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName)) throw new ArgumentNullException("collectionName");
            if (MongoStorageConfig.ServerAddress == null || MongoStorageConfig.Databases == null)
            {
                throw new Exception("未执行MongoDB数据库存储配置");
            }

            if (MongoStorageConfig.Databases.ContainsKey(databaseName))
            {
                var dbcfg = MongoStorageConfig.Databases[databaseName];
                //设置客户端配置
                Settings = new MongoClientSettings();
                if (MongoStorageConfig.ServerAddress.Count <= 1)
                {
                    Settings.Server = MongoStorageConfig.ServerAddress.First();
                }
                else
                {
                    Settings.Servers = MongoStorageConfig.ServerAddress;
                }
                var credential = MongoCredential.CreateCredential(dbcfg.DatabaseName, dbcfg.Username, dbcfg.Password);
                Settings.Credentials = new MongoCredential[] { credential };
                //设置数据库名称
                this.DatabaseName = databaseName;
            }
            else
            {
                throw new Exception("未找到数据库名为 " + databaseName + " 的数据库配置");
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
        /// <param name="collection">当前集合对象</param>
        /// <param name="projection">投影定义对象</param>
        /// <returns></returns>
        protected internal Task<TDocument> FindOneByIdAsync<TDocument>(ObjectId objectId, IMongoCollection<TDocument> collection = null, ProjectionDefinition<TDocument> projection = null) where TDocument : BsonObjectId
        {
            Task<TDocument> doc = null;

            if (collection == null)
            {
                collection = GetDatabase().GetCollection<TDocument>(CollectionName);
            }

            var fileter = Builders<TDocument>.Filter.Eq<ObjectId>(i => i.Id, objectId);
            if (projection == null)
            {
                doc = collection.Find<TDocument>(i => i.Id.Equals(objectId)).FirstOrDefaultAsync();
            }
            else
            {
                doc = collection.Find<TDocument>(fileter).Project<TDocument>(projection).FirstOrDefaultAsync();
            }

            return doc;
        }

        /// <summary>
        /// 根据ObjectId获取一条文档记录
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="objectId">文档对象Id</param>
        /// <param name="projection">投影对象linq表达式</param>
        /// <returns></returns>
        public Task<TDocument> FindOneByIdAsync<TDocument>(ObjectId objectId, Expression<Func<TDocument, TDocument>> projection = null) where TDocument : BsonObjectId
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
        /// <param name="filter">过滤条件</param>
        /// <param name="collection">当前集合对象</param>
        /// <param name="projection">投影对象定义</param>
        /// <returns></returns>
        protected internal Task<TDocument> FindOneAsync<TDocument>(FilterDefinition<TDocument> filter, IMongoCollection<TDocument> collection = null, ProjectionDefinition<TDocument> projection = null) where TDocument : new()
        {
            Task<TDocument> doc = null;
            if (collection == null)
            {
                collection = GetDatabase().GetCollection<TDocument>(CollectionName);
            }
            if (projection == null)
            {
                doc = collection.Find<TDocument>(filter).FirstOrDefaultAsync();
            }
            else
            {
                doc = collection.Find<TDocument>(filter).Project<TDocument>(projection).FirstOrDefaultAsync();
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
        public Task<TDocument> FindOneAsync<TDocument>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TDocument>> projection = null) where TDocument : new()
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
        /// 计算当前集合指定查询条件的文档格式
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">过滤条件</param>
        /// <param name="collection">当前集合对象</param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected internal Task<long> CountAsync<TDocument>(FilterDefinition<TDocument> filter, IMongoCollection<TDocument> collection = null, CountOptions options = null)
        {
            if (collection == null)
            {
                collection = GetDatabase().GetCollection<TDocument>(CollectionName);
            }

            return collection.CountAsync(filter, options);
        }

        /// <summary>
        /// 计算当前集合指定查询条件的文档格式
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
