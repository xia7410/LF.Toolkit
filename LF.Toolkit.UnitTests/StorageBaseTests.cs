using LF.Toolkit.Data;
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

    public interface IRoleStorage : ISqlStorageBootstrap { }

    public interface IUserStorage : ISqlStorageBootstrap
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

    public class UserMappingStorage : SqlMappingStorageBase, IUserStorage
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
        public void TestCreateBootstrap()
        {
            SqlStorageBootstrap.CreateBootstrap(GetType().Assembly);
            var storage = SqlStorageBootstrap.CreateInstance<IUserStorage>();
            Assert.IsInstanceOfType(storage, typeof(IUserStorage));
            Assert.AreEqual(storage.GetOne(), 100);
        }

        [TestMethod]
        public void TestCreateMappingBootstrap()
        {
            SqlStorageBootstrap.CreateBootstrap("maps", GetType().Assembly);
            var storage = SqlStorageBootstrap.CreateInstance<IUserStorage>();
            Assert.IsInstanceOfType(storage, typeof(IUserStorage));
            Assert.AreEqual(storage.GetOne(), 100);
        }
    }
}
