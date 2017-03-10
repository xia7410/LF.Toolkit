using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using LF.Toolkit.Data;
using Autofac;
using LF.Toolkit.UnitTests.Container;

namespace LF.Toolkit.UnitTests
{
    public interface IProxy
    {
        Task<int> GetCountAsync();
    }

    public class MappedProxy : SqlStorageBase<ISqlMapping>, IProxy
    {
        public MappedProxy(ISqlMapping mapping)
            : base(mapping)
        {

        }

        Task<int> IProxy.GetCountAsync()
        {
            return base.ExecuteScalarAsync<int>("GetCountAsync");
        }
    }

    [TestClass]
    public class SqlStorageUnitTest
    {
        [TestMethod]
        public async Task TestMappedGetCountAsync()
        {
            var container = SqlStorageContainer.Register("maps", this.GetType().Assembly);
            var inter = container.Resolve<IProxy>();
            var obj = container.Resolve<MappedProxy>();

            Assert.AreEqual(inter, obj);
            Assert.AreEqual(await inter.GetCountAsync(), 100);
        }
    }
}
