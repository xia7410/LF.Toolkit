using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.MongoDB
{
    public abstract class MongoStorageBase
    {
        /// <summary>
        /// MongoDB存储基类
        /// </summary>
        /// <param name="mongoUrl">mongo连接字符串</param>
        /// <param name="databaseName">mongo数据库名称</param>
        public MongoStorageBase(string mongoUrl, string databaseName)
        {
            this.Settings = MongoClientSettings.FromUrl(MongoUrl.Create(mongoUrl));
            this.DatabaseName = databaseName;
        }

        /// <summary>
        /// MongoDB存储基类
        /// </summary>
        /// <param name="config">存储配置</param>
        /// <param name="databaseKey">当前存储类使用的数据库标识名称</param>
        public MongoStorageBase(IMongoStorageConfig config, string databaseKey)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (string.IsNullOrEmpty(databaseKey)) throw new ArgumentNullException("databaseKey");

            if (config.DatabaseConfigs.ContainsKey(databaseKey))
            {
                var dbConfig = config.DatabaseConfigs[databaseKey];
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
                if (!string.IsNullOrEmpty(dbConfig.Username) && !string.IsNullOrEmpty(dbConfig.Password))
                {
                    var credential = MongoCredential.CreateCredential(dbConfig.DatabaseName, dbConfig.Username, dbConfig.Password);
                    Settings.Credentials = new MongoCredential[] { credential };
                }
                //设置副本集名称
                if (!string.IsNullOrEmpty(config.ReplicaSetName))
                {
                    Settings.ReplicaSetName = config.ReplicaSetName;
                }
                //设置数据库名称
                this.DatabaseName = dbConfig.DatabaseName;
            }
            else
            {
                throw new Exception("Could not find the '" + databaseKey + "' database config");
            }
        }

        /// <summary>
        /// MongoDB存储基类
        /// </summary>
        /// <param name="config">存储配置</param>
        /// <param name="databaseKey">当前存储类使用的数据库名称</param>
        /// <param name="collectionName">当前存储类使用的集合名称</param>
        public MongoStorageBase(IMongoStorageConfig config, string databaseKey, string collectionName)
            : this(config, databaseKey)
        {
            if (string.IsNullOrEmpty(collectionName)) throw new ArgumentNullException("collectionName");

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
        /// 缓存客户端类
        /// </summary>
        ConcurrentDictionary<int, MongoClient> m_Clients = new ConcurrentDictionary<int, MongoClient>();

        /// <summary>
        /// 获取当前数据库实例
        /// </summary>
        /// <returns></returns>
        protected virtual IMongoDatabase GetDatabase()
        {
            var client = m_Clients.GetOrAdd(this.Settings.GetHashCode(), new MongoClient(this.Settings));
            return client.GetDatabase(this.DatabaseName);
        }

        /// <summary>
        /// 返回指定过滤条件的第一条文档记录
        /// </summary>
        /// <typeparam name="TDocument">文档类型</typeparam>
        /// <param name="filter">过滤条件Linq表达式</param>
        /// <returns></returns>
        public Task<TDocument> FindOneAsync<TDocument>(string collectionName, Expression<Func<TDocument, bool>> filter)
        {
            var collection = GetDatabase().GetCollection<TDocument>(collectionName);
            return collection.Find<TDocument>(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 返回指定过滤条件的第一条BsonDocument文档记录并转换为指定类型对象
        /// </summary>
        /// <typeparam name="TDocument">文档类型</typeparam>
        /// <param name="filter">过滤条件Linq表达式</param>
        /// <returns></returns>
        public Task<TDocument> FindOneAsync<TDocument>(string collectionName, FilterDefinition<BsonDocument> filter)
        {
            var collection = GetDatabase().GetCollection<BsonDocument>(collectionName);
            return collection.Find<BsonDocument>(filter).As<TDocument>().FirstOrDefaultAsync();
        }

        /// <summary>
        /// 返回指定过滤条件的第一条文档记录并转换为新的文档类型对象
        /// </summary>
        /// <typeparam name="TDocument">文档类型</typeparam>
        /// <typeparam name="TNewDocument">返回的新文档类型</typeparam>
        /// <param name="filter">过滤条件Linq表达式</param>
        /// <param name="projection">投影对象定义</param>
        /// <returns></returns>
        public Task<TNewDocument> FindOneAsync<TDocument, TNewDocument>(string collectionName, Expression<Func<TDocument, bool>> filter, ProjectionDefinition<TDocument, TNewDocument> projection)
        {
            var collection = GetDatabase().GetCollection<TDocument>(collectionName);
            return collection.Find<TDocument>(filter).Project<TNewDocument>(projection).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 根据指定过滤条件获取一条文档记录并转换为新的文档类型对象
        /// </summary>
        /// <typeparam name="TDocument">文档类型</typeparam>
        /// <typeparam name="TNewDocument">返回的新文档类型</typeparam>
        /// <param name="collectionName"></param>
        /// <param name="filter">过滤条件定义</param>
        /// <param name="projection">投影对象定义</param>
        /// <returns></returns>
        public Task<TNewDocument> FindOneAsync<TDocument, TNewDocument>(string collectionName, FilterDefinition<TDocument> filter, ProjectionDefinition<TDocument, TNewDocument> projection)
        {
            var collection = GetDatabase().GetCollection<TDocument>(collectionName);
            return collection.Find<TDocument>(filter).Project<TNewDocument>(projection).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 获取指定过滤条件的列表
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TDocument>> FindAsync<TDocument>(string collectionName, Expression<Func<TDocument, bool>> filter, int? limit = null)
        {
            var list = new List<TDocument>();
            var collection = GetDatabase().GetCollection<TDocument>(collectionName);
            FindOptions<TDocument> options = null;
            if (limit.HasValue)
            {
                options = new FindOptions<TDocument>();
                options.Limit = limit.Value;
            }
            var cursor = await collection.FindAsync(filter, options);
            while (await cursor.MoveNextAsync())
            {
                list.AddRange(cursor.Current);
            }

            return list;
        }

        /// <summary>
        /// 获取指定过滤条件的列表
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TDocument>> FindAsync<TDocument>(string collectionName, FilterDefinition<TDocument> filter, int? limit = null)
        {
            var list = new List<TDocument>();
            var collection = GetDatabase().GetCollection<TDocument>(collectionName);
            FindOptions<TDocument> options = null;
            if (limit.HasValue)
            {
                options = new FindOptions<TDocument>();
                options.Limit = limit.Value;
            }
            var cursor = await collection.FindAsync(filter, options);
            while (await cursor.MoveNextAsync())
            {
                list.AddRange(cursor.Current);
            }

            return list;
        }

        /// <summary>
        /// 获取指定过滤条件的列表
        /// </summary>
        /// <typeparam name="TDocument">文档类型</typeparam>
        /// <typeparam name="TNewDocument">返回的新文档类型</typeparam>
        /// <param name="collectionName"></param>
        /// <param name="filter">过滤条件定义</param>
        /// <param name="projection">投影对象定义</param>
        /// <returns></returns>
        public async Task<IEnumerable<TNewDocument>> FindAsync<TDocument, TNewDocument>(string collectionName, Expression<Func<TDocument, bool>> filter, ProjectionDefinition<TDocument, TNewDocument> projection, int? limit = null)
        {
            var list = new List<TNewDocument>();
            var collection = GetDatabase().GetCollection<TDocument>(collectionName);
            var options = new FindOptions<TDocument, TNewDocument>();
            options.Projection = projection;
            if (limit.HasValue)
            {
                options.Limit = limit.Value;
            }
            var cursor = await collection.FindAsync(filter, options);
            while (await cursor.MoveNextAsync())
            {
                list.AddRange(cursor.Current);
            }

            return list;
        }

        /// <summary>
        /// 获取指定过滤条件的列表
        /// </summary>
        /// <typeparam name="TDocument">文档类型</typeparam>
        /// <typeparam name="TNewDocument">返回的新文档类型</typeparam>
        /// <param name="collectionName"></param>
        /// <param name="filter">过滤条件定义</param>
        /// <param name="projection">投影对象定义</param>
        /// <returns></returns>
        public async Task<IEnumerable<TNewDocument>> FindAsync<TDocument, TNewDocument>(string collectionName, FilterDefinition<TDocument> filter, ProjectionDefinition<TDocument, TNewDocument> projection, int? limit = null)
        {
            var list = new List<TNewDocument>();
            var collection = GetDatabase().GetCollection<TDocument>(collectionName);
            var options = new FindOptions<TDocument, TNewDocument>();
            options.Projection = projection;
            if (limit.HasValue)
            {
                options.Limit = limit.Value;
            }
            var cursor = await collection.FindAsync(filter, options);
            while (await cursor.MoveNextAsync())
            {
                list.AddRange(cursor.Current);
            }

            return list;
        }

        /// <summary>
        /// 计算当前集合指定查询条件的文档个数
        /// </summary>
        /// <typeparam name="TDocument">文档类型</typeparam>
        /// <param name="collectionName"></param>
        /// <param name="filter">过滤条件Linq表达式</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<long> CountAsync<TDocument>(string collectionName, Expression<Func<TDocument, bool>> filter, CountOptions options = null)
        {
            var collection = GetDatabase().GetCollection<TDocument>(collectionName);
            return collection.CountAsync<TDocument>(filter, options);
        }

        /// <summary>
        /// 计算当前集合指定查询条件的文档个数
        /// </summary>
        /// <typeparam name="TDocument">文档类型</typeparam>
        /// <param name="collectionName"></param>
        /// <param name="filter">过滤条件定义</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<long> CountAsync<TDocument>(string collectionName, FilterDefinition<TDocument> filter, CountOptions options = null)
        {
            var collection = GetDatabase().GetCollection<TDocument>(collectionName);
            return collection.CountAsync(filter, options);
        }
    }
}
