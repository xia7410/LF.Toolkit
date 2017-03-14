using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Threading.Tasks;
using LF.Toolkit.MongoDB;
using MongoDB.Driver;
using Autofac;
using LF.Toolkit.IOC;
using LF.Toolkit.UnitTests.Container;

namespace LF.Toolkit.UnitTests
{
    public interface ICrawler
    {
        Task<IEnumerable<BsonDocument>> GetListAsync(int count);
    }

    [Injectable]
    public class Crawler : MongoStorageBase, ICrawler
    {
        public Crawler(IMongoStorageConfig config)
            : base(config, "Crawler")
        {

        }

        Task<IEnumerable<BsonDocument>> ICrawler.GetListAsync(int count)
        {
            var filter = Builders<BsonDocument>.Filter.Empty;
            return base.FindAsync("Proxy", filter, count);
        }
    }

    [TestClass]
    public class MongoStorageUnitTest
    {
        [TestMethod]
        public async Task TestGetListAsync()
        {
            var container = MongoStorageContainer.Register("mongocfg.xml", "Crawler", this.GetType().Assembly);
            var crawler = container.Resolve<ICrawler>();
            var crawler_copy = container.Resolve<Crawler>();

            Assert.AreEqual(crawler, crawler_copy);
            int count = 10;
            var list = await crawler.GetListAsync(count);
            Assert.IsTrue(list.Count() == count);
        }

        [TestMethod]
        public async Task TestIOCGetListAsync()
        {
            var dict = new Dictionary<Type, object>();
            IMongoStorageConfig config = null;
            var configs = MongoStorageConfigs.LoadFrom("mongocfg.xml");
            if (!configs.StorageConfigs.TryGetValue("Crawler", out config)) throw new Exception("未找到指定名称的存储配置");

            dict.Add(typeof(IMongoStorageConfig), config);
            InjectionContainer.Register<MongoStorageBase>(this.GetType().Assembly, (t) => dict);
            InjectionContainer.Build();

            var crawler = InjectionContainer.Resolve<ICrawler>();
            var crawler_copy = InjectionContainer.Resolve<Crawler>();

            Assert.AreEqual(crawler, crawler_copy);
            int count = 10;
            var list = await crawler.GetListAsync(count);
            Assert.IsTrue(list.Count() == count);
        }

    }
}
