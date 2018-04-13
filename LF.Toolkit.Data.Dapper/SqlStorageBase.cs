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
        private DbProviderFactory m_DbProviderFactory;
        /// <summary>
        /// 获取数据库连接字符串设置
        /// </summary>
        protected ConnectionStringSettings ConnectionStringSettings { get; private set; }

        #region Connection & Config

        /// <summary>
        /// Sql存储基类构造器
        /// </summary>
        /// <param name="connectionKey"></param>
        public SqlStorageBase(string connectionKey)
        {
            this.ConnectionStringSettings = ConfigurationManager.ConnectionStrings[connectionKey];
            if (ConnectionStringSettings == null) throw new Exception($"未找到名称为 {connectionKey} 对应的连接字符串配置项");
            InitializeDbProvider();
        }

        /// <summary>
        /// 初始化数据库连接配置
        /// </summary>
        protected virtual void InitializeDbProvider()
        {
            m_DbProviderFactory = DbProviderFactories.GetFactory(ConnectionStringSettings.ProviderName);
            if (m_DbProviderFactory == null) throw new Exception($"未找到 { ConnectionStringSettings.ProviderName} 对应的 system.data/DbProviderFactories 配置项");
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        protected virtual IDbConnection GetConnection()
        {
            if (m_DbProviderFactory == null) throw new Exception("未配置数据库连接项");

            var connection = this.m_DbProviderFactory.CreateConnection();
            connection.ConnectionString = ConnectionStringSettings.ConnectionString;
            connection.Open();

            return connection;
        }

        /// <summary>
        /// 开始单机数据库事务
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
        protected virtual IDbTransaction BeginTransaction(IsolationLevel il = IsolationLevel.ReadCommitted)
        {
            var conn = GetConnection();
            return conn.BeginTransaction(il);
        }

        /// <summary>
        /// 关闭（非事务查询）数据库连接
        /// </summary>
        /// <param name="connection"></param>
        protected virtual void CloseConnection(IDbConnection connection)
        {
            if (connection != null && connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
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

        #region 通用Dapper查询函数封装

        /// <summary>
        /// 查询第一行数据【注意：事务查询需要在执行完毕后手动释放】 
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
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            var connection = transaction?.Connection ?? this.GetConnection();

            try
            {
                return connection.QueryFirstOrDefault<T>(commandText, param, transaction, commandTimeout, commandType);
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
        /// 查询第一行数据，若多于一行则抛出异常【注意：事务查询需要在执行完毕后手动释放】 
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
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            var connection = transaction?.Connection ?? this.GetConnection();

            try
            {
                return connection.QuerySingleOrDefault<T>(commandText, param, transaction, commandTimeout, commandType);
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
        /// 指定类型结果集查询【注意：事务查询需要在执行完毕后手动释放】 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected IEnumerable<T> Query<T>(string commandText, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            var connection = transaction?.Connection ?? this.GetConnection();

            try
            {
                return connection.Query<T>(commandText, param, transaction, buffered, commandTimeout, commandType);
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
        /// 多个结果集务查询【注意：需要在查询完毕后手动关闭连接】
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected SqlMapper.GridReader QueryMultiple(IDbConnection conn, string commandText, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            return conn.QueryMultiple(commandText, param, null, commandTimeout, commandType);
        }

        /// <summary>
        /// 多个结果集务查询【注意：需要在查询完毕后手动释放连接】 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected SqlMapper.GridReader QueryMultiple(IDbTransaction transaction, string commandText, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            var conn = transaction.Connection;
            return conn.QueryMultiple(commandText, param, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// 执行SQL查询并返回影响的行数【注意：事务查询需要在执行完毕后手动释放】
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected int Execute(string commandText, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            var connection = transaction?.Connection ?? this.GetConnection();

            try
            {
                return connection.Execute(commandText, param, transaction, commandTimeout, commandType);
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
        /// 执行SQL查询并返回第一行第一列【注意：事务查询需要在执行完毕后手动释放】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected T ExecuteScalar<T>(string commandText, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");
            var connection = transaction?.Connection ?? this.GetConnection();

            try
            {
                return connection.ExecuteScalar<T>(commandText, param, transaction, commandTimeout, commandType);
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
        /// 通用查询分页列表【注意：SQL查询语句顺序为:列表查询、总条数查询】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected PagedList<T> GetPagedList<T>(string commandText, object param = null, int? commandTimeout = null, CommandType? commandType = null)
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

        /// <summary>
        /// 执行委托查询并返回指定类型结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected T QueryFunc<T>(Func<IDbConnection, T> func)
        {
            var connection = this.GetConnection();
            try
            {
                return func.Invoke(connection);
            }
            finally
            {
                this.CloseConnection(connection);
            }
        }

        /// <summary>
        /// 执行委托食物并返回指定类型结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="li"></param>
        /// <returns></returns>
        protected T ExecuteTransactionFunc<T>(Func<IDbTransaction, T> func, IsolationLevel li = IsolationLevel.ReadCommitted)
        {
            var transaction = this.BeginTransaction(li);

            try
            {
                return func.Invoke(transaction);
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
