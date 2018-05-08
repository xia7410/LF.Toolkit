using LF.Toolkit.Data.Dapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.Dapper
{
    public abstract class SqlStorageBase<TDbContext> : SqlStorageBase
        where TDbContext : DbContext
    {
        public SqlStorageBase(string connectionKey)
            : base(connectionKey)
        {

        }

        /// <summary>
        /// 获取当前存储对应的数据库实体上下文
        /// </summary>
        /// <returns></returns>
        protected TDbContext GetDbContext()
        {
            return (TDbContext)Activator.CreateInstance(typeof(TDbContext), new object[] { base.ConnectionStringSettings.ConnectionString });
        }

        /// <summary>
        /// 添加并提交指定对象到数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected async Task<int> AddAsync<T>(T entity)
            where T : class
        {
            using (var ctx = GetDbContext())
            {
                ctx.Set<T>().Add(entity);
                return await ctx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 添加并提交多个指定对象到数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entites"></param>
        /// <returns></returns>
        protected async Task<int> AddRangeAsync<T>(IEnumerable<T> entites)
            where T : class
        {
            using (var ctx = GetDbContext())
            {
                ctx.Set<T>().AddRange(entites);
                return await ctx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 更新指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected async Task<int> UpdateAsync<T>(T entity)
            where T : class
        {
            using (var ctx = GetDbContext())
            {
                var entry = ctx.Entry(entity);
                entry.State = EntityState.Modified;

                return await ctx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 更新对象中的指定字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="properties">指定更新或排除的字段集合</param>
        /// <param name="include">是否包含。包含则更新集合中的字段，不包含则集合中的字段不更新</param>
        /// <returns></returns>
        protected async Task<int> UpdateAsync<T>(T entity, IEnumerable<Expression<Func<T, object>>> properties, bool include = true)
            where T : class
        {
            using (var ctx = GetDbContext())
            {
                var entry = ctx.Entry(entity);
                entry.State = EntityState.Modified;
                if (properties.Any())
                {
                    foreach (var property in properties)
                    {
                        entry.Property(property).IsModified = include;
                    }
                }
                return await ctx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 获取符合指定条件的首个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        protected async Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, bool tracking = false)
            where T : class
        {
            using (var ctx = GetDbContext())
            {
                var query = ctx.Set<T>().AsQueryable<T>();
                if (!tracking)
                {
                    query = query.AsNoTracking();
                }
                return await query.FirstOrDefaultAsync(predicate);
            }
        }

        /// <summary>
        /// 获取符合指定条件的首个对象，若多个则报错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        protected async Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, bool tracking = false)
            where T : class
        {
            using (var ctx = GetDbContext())
            {
                var query = ctx.Set<T>().AsQueryable<T>();
                if (!tracking)
                {
                    query = query.AsNoTracking();
                }
                return await query.SingleOrDefaultAsync(predicate);
            }
        }

        /// <summary>
        /// 删除指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        protected async Task<int> DeleteAsync<T>(T model)
            where T : class
        {
            using (var ctx = GetDbContext())
            {
                ctx.Set<T>().Remove(model);
                return await ctx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 删除符合条件的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        protected async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            using (var ctx = GetDbContext())
            {
                await ctx.Set<T>().Where(predicate).ForEachAsync(t =>
                {
                    ctx.Set<T>().Remove(t);
                });
                return await ctx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 获取当前集合列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">查询表达式</param>
        /// <param name="sortInfo">排序信息</param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        protected async Task<IEnumerable<T>> GetListAsync<T>(Expression<Func<T, bool>> predicate = null, SortInfo sortInfo = null, bool tracking = false)
            where T : class
        {
            using (var ctx = GetDbContext())
            {
                var query = ctx.Set<T>().AsQueryable<T>();
                if (!tracking)
                {
                    query = query.AsNoTracking();
                }
                //排序
                if (sortInfo != null)
                {
                    query = query.OrderBy(sortInfo.Column, sortInfo.Ascending);
                }
                //筛选条件
                if (predicate != null)
                {
                    query = query.Where(predicate);
                }

                return await query.ToListAsync();
            }
        }

        /// <summary>
        /// 获取当前集合分页列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortInfo"></param>
        /// <param name="predicate"></param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        protected async Task<PagedList<T>> GetPagedListAsync<T>(int page, int pageSize, SortInfo sortInfo, Expression<Func<T, bool>> predicate = null, bool tracking = false)
            where T : class
        {
            using (var ctx = GetDbContext())
            {
                var query = ctx.Set<T>().AsQueryable<T>();
                if (!tracking)
                {
                    query = query.AsNoTracking();
                }
                //排序
                query.OrderBy(sortInfo.Column, sortInfo.Ascending);
                //筛选条件
                if (predicate != null)
                {
                    query = query.Where(predicate);
                }

                return new PagedList<T>
                {
                    RowSet = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(),
                    Count = await query.CountAsync()
                };
            }
        }
    }
}
