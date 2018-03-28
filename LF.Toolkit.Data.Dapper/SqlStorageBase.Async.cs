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
        #region 通用Dapper查询函数封装

        /// <summary>
        /// 【异步】查询第一行数据【注意：事务查询需要在执行完毕后手动释放】 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected async Task<T> QueryFirstOrDefaultAsync<T>(string commandText, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            var connection = transaction?.Connection ?? this.GetConnection();

            try
            {
                return await connection.QueryFirstOrDefaultAsync<T>(commandText, param, transaction, commandTimeout, commandType);
            }
            finally
            {
                if (transaction == null)
                {
                    this.CloseConnection(connection);
                }
            }
        }

        /// <summary>
        /// 【异步】查询第一行数据，若多于一行则抛出异常【注意：事务查询需要在执行完毕后手动释放】  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected async Task<T> QuerySingleOrDefaultAsync<T>(string commandText, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            var connection = transaction?.Connection ?? this.GetConnection();

            try
            {
                return await connection.QuerySingleOrDefaultAsync<T>(commandText, param, transaction, commandTimeout, commandType);
            }
            finally
            {
                if (transaction == null)
                {
                    this.CloseConnection(connection);
                }
            }
        }

        /// <summary>
        /// 【异步】指定类型结果集查询【注意：事务查询需要在执行完毕后手动释放】 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected async Task<IEnumerable<T>> QueryAsync<T>(string commandText, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            var connection = transaction?.Connection ?? this.GetConnection();

            try
            {
                return await connection.QueryAsync<T>(commandText, param, transaction, commandTimeout, commandType);
            }
            finally
            {
                if (transaction == null)
                {
                    this.CloseConnection(connection);
                }
            }
        }

        /// <summary>
        /// 【异步】多个结果集务查询【注意：需要在查询完毕后手动关闭连接】
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected Task<SqlMapper.GridReader> QueryMultipleAsync(IDbConnection conn, string commandText, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            if (conn == null) throw new ArgumentNullException("conn");

            return conn.QueryMultipleAsync(commandText, param, null, commandTimeout, commandType);
        }

        /// <summary>
        /// 【异步】多个结果集务查询【注意：需要在查询完毕后手动释放连接】 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected Task<SqlMapper.GridReader> QueryMultipleAsync(IDbTransaction transaction, string commandText, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            var conn = transaction.Connection;
            return conn.QueryMultipleAsync(commandText, param, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// 【异步】执行SQL查询并返回影响的行数【注意：事务查询需要在执行完毕后手动释放】
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected async Task<int> ExecuteAsync(string commandText, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            var connection = transaction?.Connection ?? this.GetConnection();

            try
            {
                return await connection.ExecuteAsync(commandText, param, transaction, commandTimeout, commandType);
            }
            finally
            {
                if (transaction == null)
                {
                    this.CloseConnection(connection);
                }
            }
        }

        /// <summary>
        /// 【异步】执行SQL查询并返回第一行第一列【注意：事务查询需要在执行完毕后手动释放】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected async Task<T> ExecuteScalarAsync<T>(string commandText, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            var connection = transaction?.Connection ?? this.GetConnection();

            try
            {
                return await connection.ExecuteScalarAsync<T>(commandText, param, transaction, commandTimeout, commandType);
            }
            finally
            {
                if (transaction == null)
                {
                    this.CloseConnection(connection);
                }
            }
        }

        #endregion

        /// <summary>
        /// 【异步】通用查询分页列表【注意：SQL查询语句顺序为:列表查询、总条数查询】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected async Task<PagedList<T>> GetPagedListAsync<T>(string commandText, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var connection = this.GetConnection();
            var pageList = new PagedList<T>();

            try
            {
                var grid = await connection.QueryMultipleAsync(commandText, param, null, commandTimeout, commandType);
                pageList.RowSet = grid.Read<T>();
                pageList.Count = grid.Read<int>().First();
            }
            finally
            {
                this.CloseConnection(connection);
            }

            return pageList;
        }

        /// <summary>
        /// 【异步】执行委托查询并返回指定类型结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected async Task<T> QueryFuncAsync<T>(Func<IDbConnection, Task<T>> func)
        {
            var connection = this.GetConnection();

            try
            {
                return await func.Invoke(connection);
            }
            finally
            {
                this.CloseConnection(connection);
            }
        }

        /// <summary>
        /// 【异步】执行委托食物并返回指定类型结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="li"></param>
        /// <returns></returns>
        protected async Task<T> ExecuteTransactionFuncAsync<T>(Func<IDbTransaction, Task<T>> func, IsolationLevel li = IsolationLevel.ReadCommitted)
        {
            var transaction = this.BeginTransaction(li);

            try
            {
                return await func.Invoke(transaction);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                transaction?.Dispose();
            }
        }
    }
}
