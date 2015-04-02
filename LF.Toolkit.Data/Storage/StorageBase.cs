using System;
using System.Collections.Generic;
using Dapper;
using System.Configuration;
using System.Data;
using System.Data.Common;
using LF.Toolkit.Data.Map;
using LF.Toolkit.Singleton;

#if NET45

using System.Threading.Tasks;
using System.Threading;

#endif

namespace LF.Toolkit.Data.Storage
{
    /// <summary>
    /// 数据库存储访问基类
    /// </summary>
    public abstract class StorageBase : IStorage
    {
        private ConnectionStringSettings connectionStringSettings;
        private DbProviderFactory dbProviderFactory;

        internal StorageBase()
        {

        }

        /// <summary>
        /// 带有数据库连接配置项的构造器（适用于无存储映射子类使用）
        /// </summary>
        /// <param name="connectionKey"></param>
        public StorageBase(string connectionKey)
        {
            DbProviderConfig(connectionKey);
        }

        /// <summary>
        /// 关闭（非事务查询）的数据库连接
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        void CloseDbConnection(IDbConnection connection, IDbTransaction transaction)
        {
            try
            {
                //如果为事务查询则，需要在事务查询完毕后手动关闭连接
                if (transaction != null)
                {
                    return;
                }
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            catch { }
        }

        /// <summary>
        /// 数据库连接配置
        /// </summary>
        /// <param name="connectionKey"></param>
        protected virtual void DbProviderConfig(string connectionKey)
        {
            this.connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionKey];
            if (connectionStringSettings == null) throw new Exception(string.Format("未找到 {0} 对应的连接字符串配置项", connectionKey));

            dbProviderFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);
            if (dbProviderFactory == null) throw new Exception(string.Format("未找到 {0} 对应的 system.data/DbProviderFactories 配置项", connectionStringSettings.ProviderName));
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        protected virtual IDbConnection GetDbConnection()
        {
            IDbConnection connection = null;
            if (dbProviderFactory == null) throw new Exception("未配置数据库连接项");

            try
            {
                connection = this.dbProviderFactory.CreateConnection();
                connection.ConnectionString = connectionStringSettings.ConnectionString;
                connection.Open();
            }
            catch (Exception e)
            {
                throw e;
            }

            return connection;
        }

        /// <summary>
        /// 开始数据库事务
        /// </summary>
        /// <returns></returns>
        protected virtual IDbTransaction BeginTransaction()
        {
            IDbTransaction trans = null;

            var conn = GetDbConnection();
            if (conn != null)
            {
                trans = conn.BeginTransaction();
            }

            return trans;
        }

        /// <summary>
        /// 关闭（非事务查询）的数据库连接
        /// </summary>
        /// <param name="connection"></param>
        protected virtual void CloseDbConnection(IDbConnection connection)
        {
            this.CloseDbConnection(connection, null);
        }

        /// <summary>
        /// 关闭事务查询连接
        /// </summary>
        /// <param name="transaction"></param>
        protected virtual void CloseDbTransaction(IDbTransaction transaction)
        {
            try
            {
                if (transaction == null)
                {
                    return;
                }
                if (transaction.Connection.State == ConnectionState.Open)
                {
                    transaction.Connection.Close();
                }
            }
            catch { }
        }

        /// <summary>
        /// 指定类型结果集查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">查询语句</param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected IEnumerable<T> Query<T>(string commandText, dynamic param = null, IDbTransaction transaction = null, CommandType? commandType = null, bool buffered = true, int? commandTimeout = null)
        {
            IEnumerable<T> result = null;
            IDbConnection conn = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                conn = transaction != null ? transaction.Connection : this.GetDbConnection();
                result = conn.Query<T>(commandText, param as object, transaction, buffered, commandTimeout, commandType);
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
        /// 动态类型结果集查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected IEnumerable<dynamic> Query(string commandText, dynamic param = null, IDbTransaction transaction = null, CommandType? commandType = null, bool buffered = true, int? commandTimeout = null)
        {
            IEnumerable<dynamic> result = null;
            IDbConnection conn = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                conn = transaction != null ? transaction.Connection : this.GetDbConnection();
                result = conn.Query(commandText, param as object, transaction, buffered, commandTimeout, commandType);
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
        /// 多个结果集务查询，在调用时候需要使用using来释放连接
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected SqlMapper.GridReader QueryMultiple(string commandText, dynamic param = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            SqlMapper.GridReader result = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                var conn = GetDbConnection();
                result = conn.QueryMultiple(commandText, param as object, null, commandTimeout, commandType);
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        /// <summary>
        /// 多个结果集务查询，需要手动调用 CloseDbConnection 关闭查询连接 
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="conn"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected SqlMapper.GridReader QueryMultiple(string commandText, IDbConnection conn, dynamic param = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            SqlMapper.GridReader result = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                result = conn.QueryMultiple(commandText, param as object, null, commandTimeout, commandType);
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        /// <summary>
        /// 多个结果集带事务查询，需要手动调用 CloseDbTransaction 关闭查询连接 
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="transaction"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected SqlMapper.GridReader QueryMultiple(string commandText, IDbTransaction transaction, dynamic param = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            SqlMapper.GridReader result = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                var conn = transaction.Connection;
                result = conn.QueryMultiple(commandText, param as object, transaction, commandTimeout, commandType);
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        /// <summary>
        /// 执行SQL查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected int Execute(string commandText, dynamic param = null, IDbTransaction transaction = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            int result = 0;
            IDbConnection conn = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                conn = transaction != null ? transaction.Connection : this.GetDbConnection();
                result = conn.Execute(commandText, param as object, transaction, commandTimeout, commandType);
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
        /// 执行SQL查询并返回第一行第一列，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected T ExecuteScalar<T>(string commandText, dynamic param = null, IDbTransaction transaction = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            T result = default(T);
            IDbConnection conn = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                conn = transaction != null ? transaction.Connection : this.GetDbConnection();
                result = conn.ExecuteScalar<T>(commandText, param as object, transaction, commandTimeout, commandType);
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

        #region IStroage

        void IStorage.DbProviderConfig(string connectionKey)
        {
            DbProviderConfig(connectionKey);
        }

        IDbConnection IStorage.GetDbConnection()
        {
            return GetDbConnection();
        }

        IDbTransaction IStorage.BeginTransaction()
        {
            return BeginTransaction();
        }

        void IStorage.CloseDbConnection(IDbConnection connection)
        {
            CloseDbConnection(connection);
        }

        void IStorage.CloseDbTransaction(IDbTransaction transaction)
        {
            CloseDbTransaction(transaction);
        }

        #endregion

#if NET45

        #region 异步部分

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

        #endregion

#endif

    }

    /// <summary>
    /// 带有存储映射的存储访问基类
    /// </summary>
    public abstract class StorageBase<TProvider> : StorageBase where TProvider : class , IDapperProvider, new()
    {
        IDapperMapping mapper;

        public StorageBase()
        {
            try
            {
                string fullname = this.GetType().FullName;
                var factory = SingletonProvider<TProvider>.CurrentSession;
                if (factory.DapperMappings == null)
                {
                    throw new Exception("未运行映射配置 --> Configure(string path);");
                }
                if (factory.DapperMappings.ContainsKey(fullname))
                {
                    mapper = factory.DapperMappings[fullname];
                    base.DbProviderConfig(mapper.ConnectionKey);
                }
                else
                {
                    throw new Exception(string.Format("未找到 {0} 的存储映射文件", fullname));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        IDapperCommand GetDapperCommand(string key)
        {
            IDapperCommand cmd = null;
            if (!string.IsNullOrEmpty(key))
            {
                if (mapper.CommandDictionary.ContainsKey(key))
                {
                    cmd = mapper.CommandDictionary[key];
                }
            }

            if (cmd == null) throw new Exception(string.Format("未找到 {0} 对应的存储映射项", key));

            return cmd;
        }

        /// <summary>
        /// 指定类型结果集查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandKey"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected IEnumerable<T> Query<T>(string commandKey, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
        {
            IEnumerable<T> result = null;

            try
            {
                var cmd = GetDapperCommand(commandKey);
                result = base.Query<T>(cmd.CommandText, param as object, transaction, cmd.CommandType, buffered, commandTimeout);
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        /// <summary>
        /// 动态类型结果集查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected IEnumerable<dynamic> Query(string commandKey, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
        {
            IEnumerable<dynamic> result = null;

            try
            {
                var cmd = GetDapperCommand(commandKey);
                result = base.Query<dynamic>(cmd.CommandText, param as object, transaction, cmd.CommandType, buffered, commandTimeout);
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        /// <summary>
        /// 多个结果集务查询，在调用时候需要使用using来释放连接
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="conn"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected SqlMapper.GridReader QueryMultiple(string commandKey, dynamic param = null, int? commandTimeout = null)
        {
            SqlMapper.GridReader result = null;

            try
            {
                var cmd = GetDapperCommand(commandKey);
                var conn = base.GetDbConnection();

                result = base.QueryMultiple(cmd.CommandText, conn, param as object, cmd.CommandType, commandTimeout);
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        /// <summary>
        /// 多个结果集务查询，需要手动调用 CloseDbConnection 关闭查询连接 
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="conn"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected SqlMapper.GridReader QueryMultiple(string commandKey, IDbConnection conn, dynamic param = null, int? commandTimeout = null)
        {
            SqlMapper.GridReader result = null;

            try
            {
                if (conn == null) throw new ArgumentNullException("conn");
                var cmd = GetDapperCommand(commandKey);
                result = base.QueryMultiple(cmd.CommandText, conn, param as object, cmd.CommandType, commandTimeout);
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        /// <summary>
        /// 多个结果集带事务查询，需要手动调用 CloseDbTransaction 关闭事务查询连接 
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="transaction"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected SqlMapper.GridReader QueryMultiple(string commandKey, IDbTransaction transaction, dynamic param = null, int? commandTimeout = null)
        {
            SqlMapper.GridReader result = null;

            try
            {
                if (transaction == null) throw new ArgumentNullException("transaction");
                var cmd = GetDapperCommand(commandKey);
                result = base.QueryMultiple(cmd.CommandText, transaction, param as object, cmd.CommandType, commandTimeout);
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        /// <summary>
        /// 执行SQL查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected int Execute(string commandKey, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            int result = 0;

            try
            {
                var cmd = GetDapperCommand(commandKey);

                result = base.Execute(cmd.CommandText, param as object, transaction, cmd.CommandType, commandTimeout);
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        /// <summary>
        /// 执行SQL查询并返回第一行第一列，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandKey"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected T ExecuteScalar<T>(string commandKey, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            T result = default(T);

            try
            {
                if (string.IsNullOrEmpty(commandKey)) throw new ArgumentNullException("commandText");

                var cmd = GetDapperCommand(commandKey);
                result = base.ExecuteScalar<T>(cmd.CommandText, param as object, transaction, cmd.CommandType, commandTimeout);
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

#if NET45

        #region 异步部分

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
                var cmd = GetDapperCommand(commandKey);
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
                var cmd = GetDapperCommand(commandKey);
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
                var cmd = GetDapperCommand(commandKey);
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
                var cmd = GetDapperCommand(commandKey);
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
                var cmd = GetDapperCommand(commandKey);
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
                var cmd = GetDapperCommand(commandKey);
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

                var cmd = GetDapperCommand(commandKey);
                task = base.ExecuteScalarAsync<T>(cmd.CommandText, param as object, transaction, cmd.CommandType, commandTimeout);
            }
            catch (Exception e)
            {
                throw e;
            }

            return task;
        }

        #endregion

#endif

    }
}
