using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.Dapper
{
    /// <summary>
    /// 表示Sql存储异步查询部分
    /// </summary>
    public abstract partial class SqlStorageBase
    {
        /// <summary>
        /// 开启一个事务并返回事务执行结果，执行完毕后会自动释放连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">查询委托</param>
        /// <param name="li"></param>
        /// <returns></returns>
        protected async Task<T> BeginTransactionAsync<T>(Func<IDbTransaction, Task<T>> func, IsolationLevel li = IsolationLevel.ReadCommitted)
        {
            using (var transaction = this.BeginTransaction(li))
            {
                try
                {
                    return await func(transaction);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 连接数据库并返回数据库查询结果，执行完毕后会自动释放连接 【异步方式】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">查询委托</param>
        /// <returns></returns>
        protected Task<T> ConnectAsync<T>(Func<IDbConnection, Task<T>> func) => ConnectAsync(func, null);

        /// <summary>
        /// 连接数据库并返回数据库查询结果，执行完毕后会自动释放连接 【异步方式】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">查询委托</param>
        /// <param name="transaction">查询事务，若事务不为空则使用事务中的数据库连接</param>
        /// <returns></returns>
        internal protected async Task<T> ConnectAsync<T>(Func<IDbConnection, Task<T>> func, IDbTransaction transaction)
        {
            if (transaction != null)
            {
                return await func(transaction.Connection);
            }
            else
            {
                using (var connection = this.OpenConnection())
                {
                    return await func(connection);
                }
            }
        }

        #region 通用Dapper查询函数封装

        /// <summary>
        /// 【异步】查询第一行数据
        /// 【注意：事务查询需要在执行完毕后手动释放】 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected Task<T> QueryFirstOrDefaultAsync<T>(string commandText, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => ConnectAsync<T>(conn => conn.QueryFirstOrDefaultAsync<T>(commandText, param, transaction, commandTimeout, commandType), transaction);

        /// <summary>
        /// 【异步】查询第一行数据，若多于一行则抛出异常
        /// 【注意：事务查询需要在执行完毕后手动释放】  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected Task<T> QuerySingleOrDefaultAsync<T>(string commandText, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => ConnectAsync<T>(conn => conn.QuerySingleOrDefaultAsync<T>(commandText, param, transaction, commandTimeout, commandType), transaction);

        /// <summary>
        /// 【异步】指定类型结果集查询
        /// 【注意：事务查询需要在执行完毕后手动释放】 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected Task<IEnumerable<T>> QueryAsync<T>(string commandText, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
             => ConnectAsync<IEnumerable<T>>(conn => conn.QueryAsync<T>(commandText, param, transaction, commandTimeout, commandType), transaction);

        /// <summary>
        /// 【异步】多个结果集务查询
        /// 【注意：需要在查询完毕后手动关闭连接】
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected Task<SqlMapper.GridReader> QueryMultipleAsync(IDbConnection conn, string commandText, object param = null, int? commandTimeout = null, CommandType? commandType = null)
            => conn.QueryMultipleAsync(commandText, param, null, commandTimeout, commandType);

        /// <summary>
        /// 【异步】多个结果集务查询
        /// 【注意：需要在查询完毕后手动释放连接】 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected Task<SqlMapper.GridReader> QueryMultipleAsync(IDbTransaction transaction, string commandText, object param = null, int? commandTimeout = null, CommandType? commandType = null)
            => transaction.Connection.QueryMultipleAsync(commandText, param, transaction, commandTimeout, commandType);

        /// <summary>
        /// 【异步】执行SQL查询并返回影响的行数
        /// 【注意：事务查询需要在执行完毕后手动释放】
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected Task<int> ExecuteAsync(string commandText, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
             => ConnectAsync<int>(conn => conn.ExecuteAsync(commandText, param, transaction, commandTimeout, commandType), transaction);

        /// <summary>
        /// 【异步】执行SQL查询并返回第一行第一列
        /// 【注意：事务查询需要在执行完毕后手动释放】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected Task<T> ExecuteScalarAsync<T>(string commandText, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => ConnectAsync<T>(conn => conn.ExecuteScalarAsync<T>(commandText, param, transaction, commandTimeout, commandType), transaction);

        #endregion

        /// <summary>
        /// 【异步】通用查询分页列表
        /// 【注意：SQL查询语句顺序为:列表查询、总条数查询】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected Task<PagedList<T>> GetPagedListAsync<T>(string commandText, object param = null, int? commandTimeout = null, CommandType? commandType = null)
             => ConnectAsync(async conn =>
             {
                 var grid = await conn.QueryMultipleAsync(commandText, param, null, commandTimeout, commandType);

                 return new PagedList<T>
                 {
                     Count = await grid.ReadFirstAsync<int>(),
                     RowSet = await grid.ReadAsync<T>()
                 };
             });

        /// <summary>
        /// 【异步】执行委托食物并返回指定类型结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="li"></param>
        /// <returns></returns>
        protected async Task<T> ExecuteTransactionFuncAsync<T>(Func<IDbTransaction, Task<T>> func, IsolationLevel li = IsolationLevel.ReadCommitted)
        {
            using (var transaction = this.BeginTransaction(li))
            {
                try
                {
                    return await func.Invoke(transaction);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
