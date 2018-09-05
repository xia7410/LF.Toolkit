using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.Dapper
{
    /// <summary>
    /// 基于Entity Framework DbContext 的存储基类【异步部分】
    /// </summary>
    public abstract partial class SqlStorageBase<TDbContext> : SqlStorageBase
        where TDbContext : DbContext
    {
        /// <summary>
        /// 创建一个新的上下问并将其转换为Queryable类型
        /// </summary>
        /// <typeparam name="TSet"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        internal protected async Task<T> QueryableAsync<TSet, T>(Func<IQueryable<TSet>, Task<T>> func, bool tracking = false)
            where TSet : class
        {
            using (var ctx = GetDbContext())
            {
                var query = ctx.Set<TSet>().AsQueryable();
                if (!tracking)
                {
                    query = query.AsNoTracking();
                }
                return await func(query);
            }
        }

        /// <summary>
        /// 创建一个新的数据库实体上下文并执行事务操作
        /// 注意：实际的数据库上下文需要提供 DbContext(DbConnection existingConnection, bool contextOwnsConnection)构造
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected async Task<T> BeginDbTransactionAsync<T>(Func<TDbContext, DbContextTransaction, Task<T>> func)
        {
            using (var context = GetDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        return await func(context, transaction);
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 从现有的事务上创建一个新的数据库实体上下文并执行相关事务操作
        /// 注意：实际的数据库上下文需要提供 DbContext(DbConnection existingConnection, bool contextOwnsConnection)构造
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        protected async Task<T> BeginDbTransactionAsync<T>(DbTransaction transaction, Func<TDbContext, Task<T>> func)
        {
            using (var context = GetDbContext(transaction.Connection))
            {
                context.Database.UseTransaction(transaction);

                return await func(context);
            }
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
        /// 添加并提交指定对象到数据库，若对象已存在则插入失败
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="predicate">判断插入的对象是否存在</param>
        /// <returns></returns>
        protected async Task<int> AddAsync<T>(T entity, Expression<Func<T, bool>> predicate)
            where T : class
        {
            int count = 0;
            using (var ctx = GetDbContext())
            {
                var set = ctx.Set<T>();
                if (await set.CountAsync(predicate) <= 0)
                {
                    set.Add(entity);
                    count = await ctx.SaveChangesAsync();
                }
            }

            return count;
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
        protected Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, bool tracking = false)
            where T : class
            => QueryableAsync<T, T>(query => query.FirstOrDefaultAsync(predicate), tracking);

        /// <summary>
        /// 获取符合指定条件的首个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="orderBy">排序</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="ascending">是否升序; True=升序 False=降序</param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        protected Task<T> FirstOrDefaultAsync<T, TKey>(Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> predicate = null, bool ascending = false, bool tracking = false)
            where T : class
        {
            return this.QueryableAsync<T, T>(query =>
            {
                //排序
                if (ascending)
                {
                    query = query.OrderBy(orderBy);
                }
                else
                {
                    query = query.OrderByDescending(orderBy);
                }
                //查询
                if (predicate != null)
                {
                    return query.FirstOrDefaultAsync(predicate);
                }
                else
                {
                    return query.FirstOrDefaultAsync();
                }
            }, tracking);
        }

        /// <summary>
        /// 获取符合指定条件的数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        protected Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate, bool tracking = false)
            where T : class
            => QueryableAsync<T, int>(query => query.CountAsync(predicate), tracking);

        /// <summary>
        /// 获取符合指定条件的首个对象，若多个则报错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        protected Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, bool tracking = false)
            where T : class
            => QueryableAsync<T, T>(query => query.SingleOrDefaultAsync(predicate), tracking);

        /// <summary>
        /// 获取当前集合列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">查询表达式</param>
        /// <param name="sortInfo">排序信息</param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        protected Task<IEnumerable<T>> GetListAsync<T>(Expression<Func<T, bool>> predicate = null, SortInfo sortInfo = null, bool tracking = false)
            where T : class
        {
            return this.QueryableAsync<T, IEnumerable<T>>(async query =>
            {
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
            }, tracking);
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
        protected Task<PagedList<T>> GetPagedListAsync<T>(int page, int pageSize, SortInfo sortInfo, Expression<Func<T, bool>> predicate = null, bool tracking = false)
            where T : class
        {
            return this.QueryableAsync<T, PagedList<T>>(async query =>
            {
                //排序
                query = query.OrderBy(sortInfo.Column, sortInfo.Ascending);
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
            }, tracking);
        }
    }
}
