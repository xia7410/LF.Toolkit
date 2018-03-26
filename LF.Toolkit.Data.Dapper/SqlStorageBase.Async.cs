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
        /// 【异步】查询第一行数据，非事务查询会自动关闭连接，事务查询则需手动调用 CloseTransaction 关闭连接 
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
            IDbConnection conn = null;

            try
            {
                conn = transaction != null ? transaction.Connection : this.GetConnection();
                return await conn.QueryFirstOrDefaultAsync<T>(commandText, param, transaction, commandTimeout, commandType);
            }
            finally
            {
                CloseDbConnection(conn, transaction);
            }
        }

        /// <summary>
        /// 【异步】查询第一行数据，非事务查询会自动关闭连接，事务查询则需手动调用 CloseTransaction 关闭连接 
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
            IDbConnection conn = null;

            try
            {
                conn = transaction != null ? transaction.Connection : this.GetConnection();
                return await conn.QuerySingleOrDefaultAsync<T>(commandText, param, transaction, commandTimeout, commandType);
            }
            finally
            {
                CloseDbConnection(conn, transaction);
            }
        }

        /// <summary>
        /// 【异步】指定类型结果集查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseTransaction 关闭连接 
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
            IDbConnection conn = null;

            try
            {
                conn = transaction != null ? transaction.Connection : this.GetConnection();
                return await conn.QueryAsync<T>(commandText, param, transaction, commandTimeout, commandType);
            }
            finally
            {
                CloseDbConnection(conn, transaction);
            }
        }

        /// <summary>
        /// 【异步】多个结果集务查询，需要手动调用 CloseDbConnection 关闭查询连接 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="conn"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected Task<SqlMapper.GridReader> QueryMultipleAsync(string commandText, IDbConnection conn, object param = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            if (conn == null) throw new ArgumentNullException("conn");

            return conn.QueryMultipleAsync(commandText, param, null, commandTimeout, commandType);
        }

        /// <summary>
        /// 【异步】多个结果集带事务查询，需要手动调用 CloseTransaction 关闭查询连接 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="transaction"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected Task<SqlMapper.GridReader> QueryMultipleAsync(string commandText, IDbTransaction transaction, object param = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            var conn = transaction.Connection;
            return conn.QueryMultipleAsync(commandText, param, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// 【异步】执行SQL查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseTransaction 关闭连接 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected async Task<int> ExecuteAsync(string commandText, object param = null, IDbTransaction transaction = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            IDbConnection conn = null;

            try
            {
                conn = transaction != null ? transaction.Connection : this.GetConnection();
                return await conn.ExecuteAsync(commandText, param, transaction, commandTimeout, commandType);
            }
            finally
            {
                CloseDbConnection(conn, transaction);
            }
        }

        /// <summary>
        /// 【异步】执行SQL查询并返回第一行第一列，非事务查询会自动关闭连接，事务查询则需手动调用 CloseTransaction 关闭连接 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected async Task<T> ExecuteScalarAsync<T>(string commandText, object param = null, IDbTransaction transaction = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            IDbConnection conn = null;

            try
            {
                conn = transaction != null ? transaction.Connection : this.GetConnection();
                return await conn.ExecuteScalarAsync<T>(commandText, param, transaction, commandTimeout, commandType);
            }
            finally
            {
                CloseDbConnection(conn, transaction);
            }
        }

        #region 通用查询函数

        /// <summary>
        /// 【异步】通用查询分页列表，SQL格式为（查询列表语句；查询总数语句）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected async Task<PagedList<T>> GetPagedListAsync<T>(string commandText, object param = null, CommandType? commandType = null, int? commandTimeout = null)
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

        #endregion
    }
}
