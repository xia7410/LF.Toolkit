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

    public class ChatFileEntity : MongoBsonId
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
            : base(config, "im", "chatfile")
        {

        }

        Task<ChatFileEntity> IChatFileStorage.FindOneAsync(string objectId)
        {
            Console.WriteLine(this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + " Running");
            return base.FindOneByIdAsync<ChatFileEntity>(new ObjectId(objectId));
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
        public async Task TestCreateBootstrap()
        {
            var config = new MongoStorageConfig();
            config.LoadFrom("mongocfg.json");
            var bootstrap = StorageBootstrapProvider.CreateBootstrap<MongoStorageBase, MongoStroageBootstrap, IMongoStorageConfig>(config, GetType().Assembly);
            var storage = bootstrap.CreateInstanceRef<IChatFileStorage>();

            Assert.IsInstanceOfType(storage, typeof(IChatFileStorage));
            var entity = await storage.FindOneAsync("552b3d904a4a961d4c0d5001");

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
