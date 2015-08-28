using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LF.Toolkit.MongoDB;
using System.Reflection;
using LF.Toolkit.MongoDB.Config;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using LF.Toolkit.DataEngine;

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
        public ChatFileStorage(MongoStorageConfig config)
            : base(config, "im", "chatfile")
        {

        }

        Task<ChatFileEntity> IChatFileStorage.FindOneAsync(string objectId)
        {
            Console.WriteLine(this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + " Running");
            return base.FindOneByIdAsync<ChatFileEntity>(new ObjectId(objectId));
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
            var bootstrap = StorageBootstrapProvider.CreateBootstrap<MongoStorageBase, MongoStroageBootstrap, MongoStorageConfig>(config, GetType().Assembly);
            var storage = bootstrap.CreateInstanceRef<IChatFileStorage>();

            Assert.IsInstanceOfType(storage, typeof(IChatFileStorage));
            var entity = await storage.FindOneAsync("552b3d904a4a961d4c0d5001");

            Assert.IsNotNull(entity);
        }
    }
}
