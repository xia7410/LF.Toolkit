using LF.Toolkit.MongoDB.Config;
using LF.Toolkit.Singleton;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                var credential = MongoCredential.CreateScramSha1Credential(dbcfg.DatabaseName, dbcfg.Username, dbcfg.Password);
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
        /// 获取MongoDatabase实例
        /// </summary>
        /// <returns></returns>
        protected virtual MongoDatabase GetDatabase()
        {
            var client = new MongoClient(Settings);
            var server = client.GetServer();

            //当前不是admin库的用户无权限使用listDatabase命令
            //if (!server.DatabaseExists(databaseName)) throw new Exception("名称为 " + DatabaseName + " 的数据库不存在");

            return server.GetDatabase(DatabaseName);
        }

        /// <summary>
        /// Returns a cursor that can be used to find one document in this collection
        /// by its _id value as a BsonDocument.
        /// </summary>
        /// <param name="collectionName">集合名词</param>
        /// <param name="objectId">文档对象Id</param>
        /// <param name="db">数据库对象</param>
        /// <returns></returns>
        protected virtual BsonDocument FindOneById(string collectionName, ObjectId objectId, MongoDatabase db = null)
        {
            if (string.IsNullOrEmpty(collectionName)) throw new ArgumentNullException("collectionName");
            if (objectId == null) throw new ArgumentNullException("objectId");

            if (db == null)
            {
                db = GetDatabase();
            }

            BsonDocument bson = null;

            using (db.RequestStart())
            {
                var collection = db.GetCollection(collectionName);
                bson = collection.FindOneById(objectId);
            }

            return bson;
        }

        /// <summary>
        /// Returns a cursor that can be used to find one document in this collection
        /// by its _id value as a BsonDocument.
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public BsonDocument FindOneById(string objectId)
        {
            ObjectId _id;
            if (string.IsNullOrEmpty(objectId)) throw new ArgumentNullException("objectId");
            if (!ObjectId.TryParse(objectId, out _id)) throw new ArgumentException("objectId");

            return FindOneById(CollectionName, _id);
        }

        /// <summary>
        /// Returns a cursor that can be used to find one document in this collection
        /// by its _id value as a TObject.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public T FindOneById<T>(string objectId) where T : new()
        {
            ObjectId _id;
            if (string.IsNullOrEmpty(objectId)) throw new ArgumentNullException("objectId");
            if (!ObjectId.TryParse(objectId, out _id)) throw new ArgumentException("objectId");

            T obj = default(T);
            var bson = FindOneById(CollectionName, _id);
            if (bson != null)
            {
                obj = BsonConvert.DeserializeObject<T>(bson);
            }

            return obj;
        }
    }
}
