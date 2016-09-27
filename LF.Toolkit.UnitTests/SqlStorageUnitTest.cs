using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using LF.Toolkit.Data;
using Autofac;

namespace LF.Toolkit.UnitTests
{
    interface IProxy
    {
        Task<int> GetCountAsync();
    }

    //class Proxy : SqlStorageBase, IProxy
    //{
    //    public Proxy()
    //        : base("MSSQLLocalDB")
    //    {

    //    }

    //    Task<int> IProxy.GetCountAsync()
    //    {
    //        return base.ExecuteScalarAsync<int>("select 100");
    //    }
    //}

    class MappedProxy : SqlStorageBase<ISqlMapping>, IProxy
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
        IContainer m_Container;

        public SqlStorageUnitTest()
        {
            //m_Container = SqlStorageContainer.Register(this.GetType().Assembly);
        }

        //[TestMethod]
        //public async Task TestGetCountAsync()
        //{
        //    var inter = m_Container.Resolve<IProxy>();
        //    var obj = m_Container.Resolve<Proxy>();

        //    Assert.AreEqual(inter, obj);
        //    Assert.AreEqual(await inter.GetCountAsync(), 100);
        //}

        [TestMethod]
        public async Task TestMappedGetCountAsync()
        {
            m_Container = SqlStorageContainer.Register("maps", this.GetType().Assembly);
            var inter = m_Container.Resolve<IProxy>();
            var obj = m_Container.Resolve<MappedProxy>();

            Assert.AreEqual(inter, obj);
            Assert.AreEqual(await inter.GetCountAsync(), 100);
        }
    }
}
