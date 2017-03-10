using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Dapper;
using System.Data;

namespace LF.Toolkit.Data
{
    /// <summary>
    /// 表示Sql存储异步查询部分
    /// </summary>
    public abstract partial class SqlStorageBase
    {
        /// <summary>
        /// 【异步】指定类型结果集查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="flags"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<IEnumerable<T>> QueryAsync<T>(string commandText, dynamic param = null, IDbTransaction transaction = null, CommandType? commandType = null, int? commandTimeout = null
            , CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<T> result = null;
            IDbConnection conn = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                conn = transaction != null ? transaction.Connection : this.GetDbConnection();
                var commandDefinition = new CommandDefinition(commandText, param as object, transaction, commandTimeout, commandType, flags, cancellationToken);
                result = await conn.QueryAsync<T>(commandDefinition);
            }
            finally
            {
                CloseDbConnection(conn, transaction);
            }

            return result;
        }

        /// <summary>
        /// 【异步】动态类型结果集查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="flags"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<IEnumerable<dynamic>> QueryAsync(string commandText, dynamic param = null, IDbTransaction transaction = null, CommandType? commandType = null, int? commandTimeout = null
            , CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<dynamic> result = null;
            IDbConnection conn = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                conn = transaction != null ? transaction.Connection : this.GetDbConnection();
                var commandDefinition = new CommandDefinition(commandText, param as object, transaction, commandTimeout, commandType, flags, cancellationToken);
                result = await conn.QueryAsync(commandDefinition);
            }
            finally
            {
                CloseDbConnection(conn, transaction);
            }

            return result;
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
        protected Task<SqlMapper.GridReader> QueryMultipleAsync(string commandText, IDbConnection conn, dynamic param = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            if (conn == null) throw new ArgumentNullException("conn");

            return conn.QueryMultipleAsync(commandText, param as object, null, commandTimeout, commandType);
        }

        /// <summary>
        /// 【异步】多个结果集带事务查询，需要手动调用 CloseDbTransaction 关闭查询连接 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="transaction"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected Task<SqlMapper.GridReader> QueryMultipleAsync(string commandText, IDbTransaction transaction, dynamic param = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            var conn = transaction.Connection;

            return conn.QueryMultipleAsync(commandText, param as object, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// 【异步】执行SQL查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected Task<int> ExecuteAsync(string commandText, dynamic param = null, IDbTransaction transaction = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            Task<int> result = null;
            IDbConnection conn = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                conn = transaction != null ? transaction.Connection : this.GetDbConnection();
                result = conn.ExecuteAsync(commandText, param as object, transaction, commandTimeout, commandType);
            }
            finally
            {
                CloseDbConnection(conn, transaction);
            }

            return result;
        }

        /// <summary>
        /// 【异步】执行SQL查询并返回第一行第一列，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected Task<T> ExecuteScalarAsync<T>(string commandText, dynamic param = null, IDbTransaction transaction = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            Task<T> result = null;
            IDbConnection conn = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                conn = transaction != null ? transaction.Connection : this.GetDbConnection();
                result = conn.ExecuteScalarAsync<T>(commandText, param as object, transaction, commandTimeout, commandType);
            }
            finally
            {
                CloseDbConnection(conn, transaction);
            }

            return result;
        }
    }

    /// <summary>
    /// 表示带Sql映射存储异步查询部分
    /// </summary>
    /// <typeparam name="TSqlMapping"></typeparam>
    public abstract partial class SqlStorageBase<TSqlMapping> : SqlStorageBase
    {
        /// <summary>
        /// 【异步】指定类型结果集查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandKey"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="flags"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected Task<IEnumerable<T>> QueryAsync<T>(string commandKey, dynamic param = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered
            , CancellationToken cancellationToken = default(CancellationToken))
        {
            var cmd = SqlMapping[commandKey];
            int? commandTimeout = null;
            if (cmd.CommandTimeOut > 0)
            {
                commandTimeout = cmd.CommandTimeOut;
            }

            return base.QueryAsync<T>(cmd.CommandText, param as object, transaction, cmd.CommandType, commandTimeout, flags, cancellationToken);
        }

        /// <summary>
        /// 【异步】动态类型结果集查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="flags"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected Task<IEnumerable<dynamic>> QueryAsync(string commandKey, dynamic param = null, IDbTransaction transaction = null, CommandFlags flags = CommandFlags.Buffered
            , CancellationToken cancellationToken = default(CancellationToken))
        {
            var cmd = SqlMapping[commandKey];
            int? commandTimeout = null;
            if (cmd.CommandTimeOut > 0)
            {
                commandTimeout = cmd.CommandTimeOut;
            }

            return base.QueryAsync<dynamic>(cmd.CommandText, param as object, transaction, cmd.CommandType, commandTimeout, flags, cancellationToken);
        }

        /// <summary>
        /// 【异步】多个结果集务查询，需要手动调用 CloseDbConnection 关闭查询连接 
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="conn"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected Task<SqlMapper.GridReader> QueryMultipleAsync(string commandKey, IDbConnection conn, dynamic param = null)
        {
            if (conn == null) throw new ArgumentNullException("conn");

            var cmd = SqlMapping[commandKey];
            int? commandTimeout = null;
            if (cmd.CommandTimeOut > 0)
            {
                commandTimeout = cmd.CommandTimeOut;
            }

            return base.QueryMultipleAsync(cmd.CommandText, conn, param as object, cmd.CommandType, commandTimeout);
        }

        /// <summary>
        /// 【异步】多个结果集带事务查询，需要手动调用 CloseDbTransaction 关闭事务查询连接 
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="transaction"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected Task<SqlMapper.GridReader> QueryMultipleAsync(string commandKey, IDbTransaction transaction, dynamic param = null)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");

            var cmd = SqlMapping[commandKey];
            int? commandTimeout = null;
            if (cmd.CommandTimeOut > 0)
            {
                commandTimeout = cmd.CommandTimeOut;
            }

            return base.QueryMultipleAsync(cmd.CommandText, transaction, param as object, cmd.CommandType, commandTimeout);
        }

        /// <summary>
        /// 【异步】执行SQL查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        protected Task<int> ExecuteAsync(string commandKey, dynamic param = null, IDbTransaction transaction = null)
        {
            Task<int> task = null;

            try
            {
                var cmd = SqlMapping[commandKey];
                int? commandTimeout = null;
                if (cmd.CommandTimeOut > 0)
                {
                    commandTimeout = cmd.CommandTimeOut;
                }
                task = base.ExecuteAsync(cmd.CommandText, param as object, transaction, cmd.CommandType, commandTimeout);
            }
            catch (Exception e)
            {
                throw e;
            }

            return task;
        }

        /// <summary>
        /// 【异步】执行SQL查询并返回第一行第一列，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandKey"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        protected Task<T> ExecuteScalarAsync<T>(string commandKey, dynamic param = null, IDbTransaction transaction = null)
        {
            if (string.IsNullOrEmpty(commandKey)) throw new ArgumentNullException("commandText");

            var cmd = SqlMapping[commandKey];
            int? commandTimeout = null;
            if (cmd.CommandTimeOut > 0)
            {
                commandTimeout = cmd.CommandTimeOut;
            }

            return base.ExecuteScalarAsync<T>(cmd.CommandText, param as object, transaction, cmd.CommandType, commandTimeout);
        }
    }
}
