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
            catch (Exception e)
            {
                throw e;
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
                conn = transaction != null ? transaction.Connection : this.GetDbConnection();
                var commandDefinition = new CommandDefinition(commandText, param as object, transaction, commandTimeout, commandType, flags, cancellationToken);
                result = await conn.QueryAsync(commandDefinition);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                CloseDbConnection(conn, transaction);
            }

            return result;
        }

        /// <summary>
        /// 【异步】多个结果集务查询，在调用时候需要使用using来释放连接
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected async Task<SqlMapper.GridReader> QueryMultipleAsync(string commandText, dynamic param = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            SqlMapper.GridReader result = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                var conn = GetDbConnection();
                result = await conn.QueryMultipleAsync(commandText, param as object, null, commandTimeout, commandType);
            }
            catch (Exception e)
            {
                throw e;
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
        protected async Task<SqlMapper.GridReader> QueryMultipleAsync(string commandText, IDbConnection conn, dynamic param = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            SqlMapper.GridReader result = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
                if (conn == null) throw new ArgumentNullException("conn");

                result = await conn.QueryMultipleAsync(commandText, param as object, null, commandTimeout, commandType);
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
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
        protected async Task<SqlMapper.GridReader> QueryMultipleAsync(string commandText, IDbTransaction transaction, dynamic param = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            SqlMapper.GridReader result = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                var conn = transaction.Connection;
                result = await conn.QueryMultipleAsync(commandText, param as object, transaction, commandTimeout, commandType);
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
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
        protected async Task<int> ExecuteAsync(string commandText, dynamic param = null, IDbTransaction transaction = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            int result = 0;
            IDbConnection conn = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                conn = transaction != null ? transaction.Connection : this.GetDbConnection();
                result = await conn.ExecuteAsync(commandText, param as object, transaction, commandTimeout, commandType);
            }
            catch (Exception e)
            {
                throw e;
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
        protected async Task<T> ExecuteScalarAsync<T>(string commandText, dynamic param = null, IDbTransaction transaction = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            T result = default(T);
            IDbConnection conn = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                conn = transaction != null ? transaction.Connection : this.GetDbConnection();
                result = await conn.ExecuteScalarAsync<T>(commandText, param as object, transaction, commandTimeout, commandType);
            }
            catch (Exception e)
            {
                throw e;
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
        /// <param name="commandTimeout"></param>
        /// <param name="flags"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected Task<IEnumerable<T>> QueryAsync<T>(string commandKey, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null
            , CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            Task<IEnumerable<T>> task = null;

            try
            {
                var cmd = SqlMapping[commandKey];
                task = base.QueryAsync<T>(cmd.CommandText, param as object, transaction, cmd.CommandType, commandTimeout, flags, cancellationToken);
            }
            catch (Exception e)
            {
                throw e;
            }

            return task;
        }

        /// <summary>
        /// 【异步】动态类型结果集查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="flags"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected Task<IEnumerable<dynamic>> QueryAsync(string commandKey, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null
             , CommandFlags flags = CommandFlags.Buffered, CancellationToken cancellationToken = default(CancellationToken))
        {
            Task<IEnumerable<dynamic>> task = null;

            try
            {
                var cmd = SqlMapping[commandKey];
                task = base.QueryAsync<dynamic>(cmd.CommandText, param as object, transaction, cmd.CommandType, commandTimeout, flags, cancellationToken);
            }
            catch (Exception e)
            {
                throw e;
            }

            return task;
        }

        /// <summary>
        /// 【异步】多个结果集务查询，在调用时候需要使用using来释放连接
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected Task<SqlMapper.GridReader> QueryMultipleAsync(string commandKey, dynamic param = null, int? commandTimeout = null)
        {
            Task<SqlMapper.GridReader> task = null;

            try
            {
                var cmd = SqlMapping[commandKey];
                var conn = base.GetDbConnection();

                task = base.QueryMultipleAsync(cmd.CommandText, conn, param as object, cmd.CommandType, commandTimeout);
            }
            catch (Exception e)
            {
                throw e;
            }

            return task;
        }

        /// <summary>
        /// 【异步】多个结果集务查询，需要手动调用 CloseDbConnection 关闭查询连接 
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="conn"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected Task<SqlMapper.GridReader> QueryMultipleAsync(string commandKey, IDbConnection conn, dynamic param = null, int? commandTimeout = null)
        {
            Task<SqlMapper.GridReader> task = null;

            try
            {
                if (conn == null) throw new ArgumentNullException("conn");
                var cmd = SqlMapping[commandKey];
                task = base.QueryMultipleAsync(cmd.CommandText, conn, param as object, cmd.CommandType, commandTimeout);
            }
            catch (Exception e)
            {
                throw e;
            }

            return task;
        }

        /// <summary>
        /// 【异步】多个结果集带事务查询，需要手动调用 CloseDbTransaction 关闭事务查询连接 
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="transaction"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected Task<SqlMapper.GridReader> QueryMultipleAsync(string commandKey, IDbTransaction transaction, dynamic param = null, int? commandTimeout = null)
        {
            Task<SqlMapper.GridReader> task = null;

            try
            {
                if (transaction == null) throw new ArgumentNullException("transaction");
                var cmd = SqlMapping[commandKey];
                task = base.QueryMultipleAsync(cmd.CommandText, transaction, param as object, cmd.CommandType, commandTimeout);
            }
            catch (Exception e)
            {
                throw e;
            }

            return task;
        }

        /// <summary>
        /// 【异步】执行SQL查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected Task<int> ExecuteAsync(string commandKey, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            Task<int> task = null;

            try
            {
                var cmd = SqlMapping[commandKey];
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
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected Task<T> ExecuteScalarAsync<T>(string commandKey, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            Task<T> task = null;
            IDbConnection conn = null;

            try
            {
                if (string.IsNullOrEmpty(commandKey)) throw new ArgumentNullException("commandText");

                var cmd = SqlMapping[commandKey];
                task = base.ExecuteScalarAsync<T>(cmd.CommandText, param as object, transaction, cmd.CommandType, commandTimeout);
            }
            catch (Exception e)
            {
                throw e;
            }

            return task;
        }

    }
}
