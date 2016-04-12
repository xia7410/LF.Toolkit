using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LF.Toolkit.MongoDB;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using LF.Toolkit.DataEngine;
using System.IO;
using MongoDB.Driver.GridFS;
using System.Text;
using System.Linq;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace LF.Toolkit.UnitTests.NET45
{
    #region Storage

    public class ChatFilePropertyEntity
    {
        /// <summary>
        /// 获取或设置分辨率(格式如：100*100)
        /// </summary>
        [BsonElement("resolution")]
        public string Resolution { get; set; }

        /// <summary>
        /// 获取或设置影音时间长度(秒)
        /// </summary>
        [BsonElement("timeLength")]
        public int TimeLength { get; set; }

        /// <summary>
        /// 获取或设置图片缩略图二进制数据
        /// </summary>
        [BsonElement("thumbData")]
        public byte[] ThumbData { get; set; }
    }

    public class ChatFileEntity : MongoBsonId<ObjectId>
    {
        /// <summary>
        /// 获取或设置文件名称
        /// </summary>
        [BsonElement("fileName")]
        public string FileName { get; set; }

        /// <summary>
        /// 获取或设置文件大小
        /// </summary>
        [BsonElement("fileSize")]
        public long FileSize { get; set; }

        /// <summary>
        /// 获取或设置文件类型
        /// </summary>
        [BsonElement("contentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// 获取或设置文件二进制数据
        /// </summary>
        [BsonElement("binaryData")]
        public byte[] BinaryData { get; set; }

        /// <summary>
        /// 获取或设置发送者
        /// </summary>
        [BsonElement("sender")]
        public string Sender { get; set; }

        /// <summary>
        /// 获取或设置收着
        /// </summary>
        [BsonElement("receiver")]
        public string Receiver { get; set; }

        /// <summary>
        /// 获取或设置文件其他属性信息
        /// </summary>
        [BsonElement("property")]
        public ChatFilePropertyEntity Property { get; set; }

        /// <summary>
        /// 获取或设置创建日期
        /// </summary>
        [BsonElement("creationDate")]
        public DateTime CreationDate { get; set; }
    }

    public interface IChatFileStorage : IBootstrap
    {
        Task<ChatFileEntity> FindOneAsync(string objectId);
    }

    public interface IRoleStorage : IBootstrap
    {

    }

    public class ChatFileStorage : MongoStorageBase, IChatFileStorage
    {
        public ChatFileStorage(IMongoStorageConfig config)
            : base(config, "localim", "chatfile")
        {

        }

        async Task<ChatFileEntity> IChatFileStorage.FindOneAsync(string objectId)
        {
            //var projection = Builders<BsonDocument>.Projection.Include("fileName").Include("fileSize").Exclude("_id");
            var projection = Builders<BsonDocument>.Projection.As<ChatFileEntity>();
            //FilterDefinition
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(objectId));
            var findOne = await base.FindOneAsync<BsonDocument, ChatFileEntity>(base.CollectionName, filter, projection);
            Assert.IsNotNull(findOne);

            //expression
            var pj = Builders<ChatFileEntity>.Projection.Include(i => i.FileSize);
            findOne = await base.FindOneAsync<ChatFileEntity, ChatFileEntity>(base.CollectionName, b => b.Id == new ObjectId(objectId), pj);

            Assert.IsNotNull(findOne);

            //find as
            findOne = await base.FindOneAsync<ChatFileEntity>(base.CollectionName, filter);

            return findOne;
        }
    }

    public interface IBigDataGridFS : IBootstrap
    {
        Task<ObjectId> Upload(FileStream fs);

        Task<byte[]> Open(ObjectId id);

        Task<GridFSFileInfo> FindOne(string md5);
    }

    public class BigDataGridFS : MongoGridFSBase, IBigDataGridFS
    {
        public BigDataGridFS(IMongoStorageConfig config)
            : base(config, "im", "bigdata")
        {

        }

        Task<ObjectId> IBigDataGridFS.Upload(FileStream fs)
        {
            return base.UploadFromStreamAsync(Path.GetFileName(fs.Name), fs);
        }

        Task<byte[]> IBigDataGridFS.Open(ObjectId id)
        {
            return base.DownloadAsBytesAsync(id);
        }

        async Task<GridFSFileInfo> IBigDataGridFS.FindOne(string filename)
        {
            var builder = Builders<GridFSFileInfo>.Filter.Eq<string>(i => i.Filename, filename);
            var cursor = await base.FindAsync(builder, new GridFSFindOptions { Limit = 1 });
            var list = await cursor.ToListAsync();

            return list.FirstOrDefault();
        }
    }

    #endregion

    [TestClass]
    public class MongoStorageBaseTests
    {
        [TestMethod]
        public void TestMongoStorageConfig()
        {
            var config = new MongoStorageConfig("mongocfg.json");
            Assert.IsTrue(config.Databases.ContainsKey("localim"));
            Assert.IsTrue(config.Databases["localim"].DatabaseName == "im");
            var multiple_config = new MultipleMongoStorageConfig("mongocfg_multiple.json");
            var _config  = multiple_config.GetMongoStorageConfig("localConfig");
            Assert.IsNotNull(_config);
            Assert.IsTrue(_config.Databases.ContainsKey("localfile"));
            Assert.IsTrue(_config.Databases["localfile"].DatabaseName == "file");
        }

        [TestMethod]
        public async Task TestCreateBootstrap()
        {
            var config = new MongoStorageConfig();
            config.LoadFrom("mongocfg.json");
            var bootstrap = StorageBootstrapProvider.CreateBootstrap<MongoStorageBase, MongoStroageBootstrap, IMongoStorageConfig>(config, GetType().Assembly);
            var storage = bootstrap.CreateInstanceRef<IChatFileStorage>();

            Assert.IsInstanceOfType(storage, typeof(IChatFileStorage));
            var entity = await storage.FindOneAsync("5700ee72016ff92ef825c14b");

            Assert.IsNotNull(entity);
        }

        [TestMethod]
        public async Task TestTransferGridFS()
        {
            var config = new MongoStorageConfig();
            config.LoadFrom("mongocfg.json");
            var bootstrap = StorageBootstrapProvider.CreateBootstrap<MongoStorageBase, MongoStroageBootstrap, IMongoStorageConfig>(config, GetType().Assembly);
            var gridfs = bootstrap.CreateInstanceRef<IBigDataGridFS>();

            Assert.IsInstanceOfType(gridfs, typeof(IBigDataGridFS));
            //upload
            var objectId = await gridfs.Upload(File.OpenRead("mongocfg.json"));
            //download
            var buffers = await gridfs.Open(objectId);
            Assert.IsNotNull(buffers);
        }

        [TestMethod]
        public async Task TestFindGridFS()
        {
            var config = new MongoStorageConfig();
            config.LoadFrom("mongocfg.json");
            var bootstrap = StorageBootstrapProvider.CreateBootstrap<MongoStorageBase, MongoStroageBootstrap, IMongoStorageConfig>(config, GetType().Assembly);
            var gridfs = bootstrap.CreateInstanceRef<IBigDataGridFS>();

            Assert.IsInstanceOfType(gridfs, typeof(IBigDataGridFS));
            var gfs = await gridfs.FindOne("mongocfg.json");

            Assert.IsNotNull(gridfs);
        }
    }
}
