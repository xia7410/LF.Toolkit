using LF.Toolkit.Data;
using LF.Toolkit.DataEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.UnitTests
{
    #region Storage

    public interface IRoleStorage : IBootstrap { }

    public interface IUserStorage : IBootstrap
    {
        int GetOne();
    }

    public class UserStorage : SqlStorageBase, IUserStorage
    {
        public UserStorage()
            : base("SF_AuthentDB")
        {

        }

        public int GetOne()
        {
            Console.WriteLine(this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + " Running");
            return base.ExecuteScalar<int>("select 100");
        }
    }

    public class UserMappingStorage : SqlStorageBase<ISqlMapping>, IUserStorage
    {
        public UserMappingStorage(ISqlMapping mapping)
            : base(mapping)
        {

        }

        int IUserStorage.GetOne()
        {
            Console.WriteLine(this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + " Running");
            return base.ExecuteScalar<int>("GetOne");
        }
    }

    #endregion

    [TestClass]
    public class StorageBaseTests
    {
        [TestMethod]
        public void TestCreateInstanceRef()
        {
            var bootstrap = StorageBootstrapProvider.CreateBootstrap<SqlStorageBase, StorageBootstrap>(GetType().Assembly);
            var storage = bootstrap.CreateInstanceRef<IUserStorage>();
            Assert.IsInstanceOfType(storage, typeof(IUserStorage));
            Assert.AreEqual(storage.GetOne(), 100);
        }

        [TestMethod]
        public void TestGenericCreateInstanceRef()
        {
            ISqlMappingCollection collection = new SqlMappingCollection();
            collection.LoadFrom("maps");
            var bootstrap = StorageBootstrapProvider.CreateBootstrap<SqlStorageBase, SqlStorageBootstrap, ISqlMappingCollection>(collection, GetType().Assembly);
            var storage = bootstrap.CreateInstanceRef<IUserStorage>();
            Assert.IsInstanceOfType(storage, typeof(IUserStorage));
            Assert.AreEqual(storage.GetOne(), 100);
        }
    }
}
