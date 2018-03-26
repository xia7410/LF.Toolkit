using System;
using System.Collections.Generic;
using Dapper;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Data.SqlTypes;

namespace LF.Toolkit.Data.Dapper
{
    /// <summary>
    /// 表示Sql数据库存储基类
    /// </summary>
    public abstract partial class SqlStorageBase
    {
        private ConnectionStringSettings m_ConnectionStringSettings;
        private DbProviderFactory m_DbProviderFactory;

        #region Connection & Config

        public SqlStorageBase(string connectionKey)
        {
            InitializeDbProvider(connectionKey);
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
        /// 初始化数据库连接配置
        /// </summary>
        /// <param name="connectionKey"></param>
        protected virtual void InitializeDbProvider(string connectionKey)
        {
            this.m_ConnectionStringSettings = ConfigurationManager.ConnectionStrings[connectionKey];
            if (m_ConnectionStringSettings == null) throw new Exception(string.Format("未找到Key为 {0} 对应的连接字符串配置项", connectionKey));

            m_DbProviderFactory = DbProviderFactories.GetFactory(m_ConnectionStringSettings.ProviderName);
            if (m_DbProviderFactory == null) throw new Exception(string.Format("未找到 {0} 对应的 system.data/DbProviderFactories 配置项", m_ConnectionStringSettings.ProviderName));
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        protected virtual IDbConnection GetConnection()
        {
            IDbConnection connection = null;
            if (m_DbProviderFactory == null) throw new Exception("未配置数据库连接项");

            connection = this.m_DbProviderFactory.CreateConnection();
            connection.ConnectionString = m_ConnectionStringSettings.ConnectionString;
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

            var conn = GetConnection();
            if (conn != null)
            {
                trans = conn.BeginTransaction();
            }

            return trans;
        }

        /// <summary>
        /// 开始数据库事务
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
        protected virtual IDbTransaction BeginTransaction(IsolationLevel il)
        {
            IDbTransaction trans = null;

            var conn = GetConnection();
            if (conn != null)
            {
                trans = conn.BeginTransaction(il);
            }

            return trans;
        }

        /// <summary>
        /// 关闭（非事务查询）数据库连接
        /// </summary>
        /// <param name="connection"></param>
        protected virtual void CloseConnection(IDbConnection connection)
        {
            this.CloseDbConnection(connection, null);
        }

        /// <summary>
        /// 关闭事务查询连接
        /// </summary>
        /// <param name="transaction"></param>
        protected virtual void CloseTransaction(IDbTransaction transaction)
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

        /// <summary>
        /// 解析并生成排序语句
        /// 若解析失败则返回默认排序信息
        /// </summary>
        /// <param name="orderBy">>格式为：field_(DESC/ASC)</param>
        /// <param name="defaultSort"></param>
        /// <param name="allowSortColumns">允许排序的字段信息集合</param>
        /// <returns></returns>
        protected SortInfo GetSortInfo(string orderBy, SortInfo defaultSort, IEnumerable<SortColumn> allowSortColumns = null)
        {
            var sortInfo = SortInfo.Parse(orderBy);
            if (sortInfo == null)
            {
                sortInfo = defaultSort;
            }
            else
            {
                var sortColumn = allowSortColumns.SingleOrDefault(i => i.Column == sortInfo.Column);
                if (sortColumn == null) throw new ArgumentException("无效的排序字符串");
                //判断排序字段是否有别名若有则转换为含有别名的字段
                if (!string.IsNullOrEmpty(sortColumn.Alias))
                {
                    sortInfo.Column = sortColumn.ToString();
                }
            }

            return sortInfo;
        }

        /// <summary>
        /// 转换为dapper支持的like参数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="leftWrapper">是否包含左边%</param>
        /// <param name="rightWrapper">是否包含右边%</param>
        /// <returns></returns>
        protected string ToLikeParam(string value, bool leftWrapper = true, bool rightWrapper = true)
        {
            string param = "";
            if (!string.IsNullOrEmpty(value))
            {
                param = $"{(leftWrapper ? "%" : "")}{value}{(rightWrapper ? "%" : "")}";
            }

            return param;
        }

        /// <summary>
        /// 转换日期字符串为Sql时间
        /// 若日期转换失败、日期小于1753-01-01或大于9999-12-31，则返回Sql最小时间1753-01-01
        /// </summary>
        /// <param name="timeStr"></param>
        /// <returns></returns>
        protected DateTime ParseSqlDateTime(string timeStr)
        {
            var date = DateTime.MinValue;
            DateTime.TryParse(timeStr, out date);

            if (date < SqlDateTime.MinValue.Value || date > SqlDateTime.MaxValue.Value)
            {
                date = SqlDateTime.MinValue.Value;
            }

            return date;
        }

        #endregion

        #region 通用查询函数

        /// <summary>
        /// 查询第一行数据，非事务查询会自动关闭连接，事务查询则需手动调用 CloseTransaction 关闭连接 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected T QueryFirstOrDefault<T>(string commandText, object param = null, IDbTransaction transaction = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            T result = default(T);
            IDbConnection conn = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                conn = transaction != null ? transaction.Connection : this.GetConnection();
                result = conn.QueryFirstOrDefault<T>(commandText, param, transaction, commandTimeout, commandType);
            }
            finally
            {
                CloseDbConnection(conn, transaction);
            }

            return result;
        }

        /// <summary>
        /// 查询第一行数据，非事务查询会自动关闭连接，事务查询则需手动调用 CloseTransaction 关闭连接 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected T QuerySingleOrDefault<T>(string commandText, object param = null, IDbTransaction transaction = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            T result = default(T);
            IDbConnection conn = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                conn = transaction != null ? transaction.Connection : this.GetConnection();
                result = conn.QuerySingleOrDefault<T>(commandText, param, transaction, commandTimeout, commandType);
            }
            finally
            {
                CloseDbConnection(conn, transaction);
            }

            return result;
        }

        /// <summary>
        /// 指定类型结果集查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseTransaction 关闭连接 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">查询语句</param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected IEnumerable<T> Query<T>(string commandText, object param = null, IDbTransaction transaction = null, CommandType? commandType = null, bool buffered = true, int? commandTimeout = null)
        {
            IEnumerable<T> result = null;
            IDbConnection conn = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                conn = transaction != null ? transaction.Connection : this.GetConnection();
                result = conn.Query<T>(commandText, param, transaction, buffered, commandTimeout, commandType);
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
        protected SqlMapper.GridReader QueryMultiple(string commandText, IDbConnection conn, object param = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            return conn.QueryMultiple(commandText, param, null, commandTimeout, commandType);
        }

        /// <summary>
        /// 多个结果集带事务查询，需要手动调用 CloseTransaction 关闭查询连接 
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="transaction"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected SqlMapper.GridReader QueryMultiple(string commandText, IDbTransaction transaction, object param = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            var conn = transaction.Connection;
            return conn.QueryMultiple(commandText, param, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// 执行SQL查询，非事务查询会自动关闭连接，事务查询则需手动调用 CloseTransaction 关闭连接 
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected int Execute(string commandText, object param = null, IDbTransaction transaction = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            int result = 0;
            IDbConnection conn = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                conn = transaction != null ? transaction.Connection : this.GetConnection();
                result = conn.Execute(commandText, param, transaction, commandTimeout, commandType);
            }
            finally
            {
                CloseDbConnection(conn, transaction);
            }

            return result;
        }

        /// <summary>
        /// 执行SQL查询并返回第一行第一列，非事务查询会自动关闭连接，事务查询则需手动调用 CloseTransaction 关闭连接 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected T ExecuteScalar<T>(string commandText, object param = null, IDbTransaction transaction = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            T result = default(T);
            IDbConnection conn = null;

            try
            {
                if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

                conn = transaction != null ? transaction.Connection : this.GetConnection();
                result = conn.ExecuteScalar<T>(commandText, param, transaction, commandTimeout, commandType);
            }
            finally
            {
                CloseDbConnection(conn, transaction);
            }

            return result;
        }

        #endregion

        /// <summary>
        /// 通用查询分页列表，SQL格式为（查询列表语句；查询总数语句）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected PagedList<T> GetPagedList<T>(string commandText, object param = null, CommandType? commandType = null, int? commandTimeout = null)
        {
            var connection = this.GetConnection();
            var pageList = new PagedList<T>();

            try
            {
                var grid = connection.QueryMultiple(commandText, param, null, commandTimeout, commandType);
                pageList.RowSet = grid.Read<T>();
                pageList.Count = grid.Read<int>().First();
            }
            finally
            {
                this.CloseConnection(connection);
            }

            return pageList;
        }
    }
}
