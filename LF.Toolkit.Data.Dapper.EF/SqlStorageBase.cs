﻿using LF.Toolkit.Data.Dapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LF.Toolkit.Data.Dapper
{
    /// <summary>
    /// 基于Entity Framework DbContext 的存储基类
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public abstract partial class SqlStorageBase<TDbContext> : SqlStorageBase
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
        protected int Add<T>(T entity)
            where T : class
        {
            using (var ctx = GetDbContext())
            {
                ctx.Set<T>().Add(entity);
                return ctx.SaveChanges();
            }
        }

        /// <summary>
        /// 添加并提交指定对象到数据库，若对象已存在则插入失败
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="predicate">判断插入的对象是否存在</param>
        /// <returns></returns>
        protected int Add<T>(T entity, Expression<Func<T, bool>> predicate)
            where T : class
        {
            int count = 0;
            using (var ctx = GetDbContext())
            {
                var set = ctx.Set<T>();
                if (set.Count(predicate) <= 0)
                {
                    set.Add(entity);
                    count = ctx.SaveChanges();
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
        protected int AddRange<T>(IEnumerable<T> entites)
            where T : class
        {
            using (var ctx = GetDbContext())
            {
                ctx.Set<T>().AddRange(entites);
                return ctx.SaveChanges();
            }
        }

        /// <summary>
        /// 更新指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected int Update<T>(T entity)
            where T : class
        {
            using (var ctx = GetDbContext())
            {
                var entry = ctx.Entry(entity);
                entry.State = EntityState.Modified;

                return ctx.SaveChanges();
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
        protected int Update<T>(T entity, IEnumerable<Expression<Func<T, object>>> properties, bool include = true)
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
                return ctx.SaveChanges();
            }
        }

        /// <summary>
        /// 获取符合指定条件的首个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        protected T FirstOrDefault<T>(Expression<Func<T, bool>> predicate, bool tracking = false)
            where T : class
        {
            using (var ctx = GetDbContext())
            {
                var query = ctx.Set<T>().AsQueryable<T>();
                if (!tracking)
                {
                    query = query.AsNoTracking();
                }
                return query.FirstOrDefault(predicate);
            }
        }

        /// <summary>
        /// 获取符合指定条件的首个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="order">排序</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="ascending">是否升序; True=升序 False=降序</param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        protected T FirstOrDefault<T, TKey>(Expression<Func<T, TKey>> order, Expression<Func<T, bool>> predicate = null, bool ascending = false, bool tracking = false)
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
                if (ascending)
                {
                    query = query.OrderBy(order);
                }
                else
                {
                    query = query.OrderByDescending(order);
                }
                //查询
                if (predicate != null)
                {
                    return query.FirstOrDefault(predicate);
                }
                else
                {
                    return query.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// 获取符合指定条件的数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        protected int Count<T>(Expression<Func<T, bool>> predicate, bool tracking = false)
            where T : class
        {
            using (var ctx = GetDbContext())
            {
                var query = ctx.Set<T>().AsQueryable<T>();
                if (!tracking)
                {
                    query = query.AsNoTracking();
                }
                return query.Count(predicate);
            }
        }

        /// <summary>
        /// 获取符合指定条件的首个对象，若多个则报错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        protected T SingleOrDefault<T>(Expression<Func<T, bool>> predicate, bool tracking = false)
            where T : class
        {
            using (var ctx = GetDbContext())
            {
                var query = ctx.Set<T>().AsQueryable<T>();
                if (!tracking)
                {
                    query = query.AsNoTracking();
                }
                return query.SingleOrDefault(predicate);
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
        protected IEnumerable<T> GetList<T>(Expression<Func<T, bool>> predicate = null, SortInfo sortInfo = null, bool tracking = false)
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

                return query.ToList();
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
        protected PagedList<T> GetPagedList<T>(int page, int pageSize, SortInfo sortInfo, Expression<Func<T, bool>> predicate = null, bool tracking = false)
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
                query = query.OrderBy(sortInfo.Column, sortInfo.Ascending);
                //筛选条件
                if (predicate != null)
                {
                    query = query.Where(predicate);
                }

                return new PagedList<T>
                {
                    RowSet = query.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                    Count = query.Count()
                };
            }
        }
    }
}
