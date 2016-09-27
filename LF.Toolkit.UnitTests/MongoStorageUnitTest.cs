using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Threading.Tasks;
using LF.Toolkit.MongoDB;
using MongoDB.Driver;
using Autofac;

namespace LF.Toolkit.UnitTests
{
    interface ICrawler
    {
        Task<IEnumerable<BsonDocument>> GetListAsync(int count);
    }

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
        IContainer m_Container = null;

        public MongoStorageUnitTest()
        {
            m_Container = MongoStorageContainer.Register("mongocfg.xml", "Crawler", this.GetType().Assembly);
        }

        [TestMethod]
        public async Task TestGetListAsync()
        {
            var crawler = m_Container.Resolve<ICrawler>();
            var crawler_copy = m_Container.Resolve<Crawler>();
            
            Assert.AreEqual(crawler, crawler_copy);
            int count = 10;
            var list = await crawler.GetListAsync(count);
            Assert.IsTrue(list.Count() == count);
        }
    }
}
