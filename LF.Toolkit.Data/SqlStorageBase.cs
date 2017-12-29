using System;
using System.Collections.Generic;
using Dapper;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace LF.Toolkit.Data
{
    /// <summary>
    /// 表示Sql数据库存储基类
    /// </summary>
    public abstract partial class SqlStorageBase
    {
        private ConnectionStringSettings connectionStringSettings;
        private DbProviderFactory dbProviderFactory;

        internal SqlStorageBase()
        {

        }

        #region Connection & Config


        public SqlStorageBase(string connectionKey)
        {
            ConfigureDbProvider(connectionKey);
        }

        /// <summary>
        /// 关闭（非事务查询）数据库连接
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
        protected virtual void ConfigureDbProvider(string connectionKey)
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

            connection = this.dbProviderFactory.CreateConnection();
            connection.ConnectionString = connectionStringSettings.ConnectionString;
            connection.Open();

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
        /// 关闭（非事务查询）数据库连接
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

        #endregion

        #region 通用函数

        static readonly string[] m_Orders = new string[] { "ASC", "DESC" };

        /// <summary>
        /// 解析并生成排序语句
        /// 若解析失败则返回默认排序语句
        /// </summary>
        /// <param name="orderBy">格式为：field_(DESC/ASC)</param>
        /// <param name="defaultOrderBy">默认排序语句(可空)</param>
        /// <returns></returns>
        protected string GetOrderBySql(string orderBy, string defaultOrderBy = "")
        {
            if (!string.IsNullOrEmpty(orderBy))
            {
                var arrs = orderBy.Replace(" ", "").Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                if (arrs.Length >= 2 && m_Orders.Contains(arrs[1].ToUpper()))
                {
                    return string.Format(" ORDER BY {0} {1} ", arrs[0], arrs[1]);
                }
            }

            return defaultOrderBy;
        }

        /// <summary>
        /// 转换为dapper支持的like参数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="leftWrapper">是否包含左边%</param>
        /// <param name="rightWrapper">是否包含右边%</param>
        /// <returns></returns>
        protected string ConvertToLikeWrapper(string value, bool leftWrapper = true, bool rightWrapper = true)
        {
            string param = "";
            if (!string.IsNullOrEmpty(value))
            {
                param = value;
                if (leftWrapper) param = "%" + param;
                if (rightWrapper) param += "%";
            }

            return param;
        }

        #endregion

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
            finally
            {
                CloseDbConnection(conn, transaction);
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
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            return conn.QueryMultiple(commandText, param as object, null, commandTimeout, commandType);
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
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            var conn = transaction.Connection;
            return conn.QueryMultiple(commandText, param as object, transaction, commandTimeout, commandType);
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
            finally
            {
                CloseDbConnection(conn, transaction);
            }

            return result;
        }
    }

    /// <summary>
    /// 表示带有Sql映射的泛型存储基类
    /// </summary>
    public abstract partial class SqlStorageBase<TSqlMapping> : SqlStorageBase
        where TSqlMapping : class, ISqlMapping
    {
        TSqlMapping SqlMapping { get; set; }

        public SqlStorageBase(TSqlMapping sqlMapping)
        {
            if (sqlMapping == null) throw new ArgumentNullException("sqlMapping");

            SqlMapping = sqlMapping;
            base.ConfigureDbProvider(sqlMapping.ConnectionKey);
        }

        /// <summary>
        /// 指定类型结果集查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandKey"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        protected IEnumerable<T> Query<T>(string commandKey, dynamic param = null, IDbTransaction transaction = null, bool buffered = true)
        {
            var cmd = SqlMapping[commandKey];
            int? commandTimeout = null;
            if (cmd.CommandTimeOut > 0)
            {
                commandTimeout = cmd.CommandTimeOut;
            }

            return base.Query<T>(cmd.CommandText, param as object, transaction, cmd.CommandType, buffered, commandTimeout);
        }

        /// <summary>
        /// 动态类型结果集查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        protected IEnumerable<dynamic> Query(string commandKey, dynamic param = null, IDbTransaction transaction = null, bool buffered = true)
        {
            var cmd = SqlMapping[commandKey];
            int? commandTimeout = null;
            if (cmd.CommandTimeOut > 0)
            {
                commandTimeout = cmd.CommandTimeOut;
            }

            return base.Query<dynamic>(cmd.CommandText, param as object, transaction, cmd.CommandType, buffered, commandTimeout);
        }

        /// <summary>
        /// 多个结果集务查询，需要手动调用 CloseDbConnection 关闭查询连接 
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="conn"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected SqlMapper.GridReader QueryMultiple(string commandKey, IDbConnection conn, dynamic param = null)
        {
            if (conn == null) throw new ArgumentNullException("conn");
            var cmd = SqlMapping[commandKey];
            int? commandTimeout = null;
            if (cmd.CommandTimeOut > 0)
            {
                commandTimeout = cmd.CommandTimeOut;
            }

            return base.QueryMultiple(cmd.CommandText, conn, param as object, cmd.CommandType, commandTimeout);
        }

        /// <summary>
        /// 多个结果集带事务查询，需要手动调用 CloseDbTransaction 关闭事务查询连接 
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="transaction"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected SqlMapper.GridReader QueryMultiple(string commandKey, IDbTransaction transaction, dynamic param = null)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            var cmd = SqlMapping[commandKey];
            int? commandTimeout = null;
            if (cmd.CommandTimeOut > 0)
            {
                commandTimeout = cmd.CommandTimeOut;
            }

            return base.QueryMultiple(cmd.CommandText, transaction, param as object, cmd.CommandType, commandTimeout);
        }

        /// <summary>
        /// 执行SQL查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        protected int Execute(string commandKey, dynamic param = null, IDbTransaction transaction = null)
        {
            var cmd = SqlMapping[commandKey];
            int? commandTimeout = null;
            if (cmd.CommandTimeOut > 0)
            {
                commandTimeout = cmd.CommandTimeOut;
            }

            return base.Execute(cmd.CommandText, param as object, transaction, cmd.CommandType, commandTimeout);
        }

        /// <summary>
        /// 执行SQL查询并返回第一行第一列，非事务查询会自动关闭连接，事务查询则需手动调用 CloseDbTransaction 关闭连接 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandKey"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        protected T ExecuteScalar<T>(string commandKey, dynamic param = null, IDbTransaction transaction = null)
        {
            if (string.IsNullOrEmpty(commandKey)) throw new ArgumentNullException("commandText");

            var cmd = SqlMapping[commandKey];
            int? commandTimeout = null;
            if (cmd.CommandTimeOut > 0)
            {
                commandTimeout = cmd.CommandTimeOut;
            }

            return base.ExecuteScalar<T>(cmd.CommandText, param as object, transaction, cmd.CommandType, commandTimeout);
        }
    }
}
