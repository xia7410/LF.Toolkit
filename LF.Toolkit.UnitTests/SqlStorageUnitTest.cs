using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using LF.Toolkit.Data;
using Autofac;
using LF.Toolkit.UnitTests.Container;
using LF.Toolkit.IOC;
using System.Collections.Generic;

namespace LF.Toolkit.UnitTests
{
    public class Product
    {

        /// <summary>
        /// 获取或设置
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public DateTime Created { get; set; }

    }

    public interface IProductStorage
    {
        /// <summary>
        /// 插入
        /// </summary>
        Task<int> InsertAsync(Product model);

        /// <summary>
        /// 更新
        /// </summary>
        Task<int> UpdateAsync(Product model);

        /// <summary>
        /// 删除
        /// </summary>
        Task<int> DeleteAsync(int id);
    }

    public class ProductStorage : SqlStorageBase<ISqlMapping>, IProductStorage
    {
        public ProductStorage(ISqlMapping mapping)
            : base(mapping)
        {

        }

        /// <summary>
        /// 插入
        /// </summary>
        Task<int> IProductStorage.InsertAsync(Product model)
        {
            return base.ExecuteAsync("InsertAsync", model);
        }

        /// <summary>
        /// 更新
        /// </summary>
        Task<int> IProductStorage.UpdateAsync(Product model)
        {
            return base.ExecuteAsync("InsertAsync", model);
        }

        /// <summary>
        /// 删除
        /// </summary>
        Task<int> IProductStorage.DeleteAsync(int id)
        {
            return base.ExecuteAsync("DeleteAsync", new { Id = id });
        }
    }

    [TestClass]
    public class SqlStorageUnitTest
    {
        [TestMethod]
        public async Task TestMappedGetCountAsync()
        {
            var mapings = SqlMappingCollection.LoadFrom("maps");
            InjectionContainer.Register<SqlStorageBase>(this.GetType().Assembly, (t) =>
            {
                var dict = new Dictionary<Type, object>();
                dict.Add(typeof(ISqlMapping), mapings[t.FullName]);

                return dict;
            });
            InjectionContainer.Build();

            var storage = InjectionContainer.Resolve<IProductStorage>();
            await storage.DeleteAsync(1);
            await storage.DeleteAsync(2);
        }
    }
}
