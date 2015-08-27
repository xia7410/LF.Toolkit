using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LF.Toolkit.Data
{
    /// <summary>
    /// 带有Sql存储映射的存储基类
    /// </summary>
    public abstract partial class SqlMappingStorageBase : SqlStorageBase
    {
        protected ISqlMapping Mapping { get; private set; }

        public SqlMappingStorageBase(ISqlMapping mapping)
        {
            if (mapping == null) throw new ArgumentNullException("mapping");

            Mapping = mapping;
            base.DbProviderConfig(mapping.ConnectionKey);
        }

        ISqlCommand GetSqlCommand(string key)
        {
            ISqlCommand cmd = null;

            if (!string.IsNullOrEmpty(key))
            {
                if (Mapping.CommandDictionary.ContainsKey(key))
                {
                    cmd = Mapping.CommandDictionary[key];
                }
            }

            if (cmd == null) throw new Exception(string.Format("Could not find the specified SqlCommand '{0}'", key));

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
                var cmd = GetSqlCommand(commandKey);
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
                var cmd = GetSqlCommand(commandKey);
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
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected SqlMapper.GridReader QueryMultiple(string commandKey, dynamic param = null, int? commandTimeout = null)
        {
            SqlMapper.GridReader result = null;

            try
            {
                var cmd = GetSqlCommand(commandKey);
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
                var cmd = GetSqlCommand(commandKey);
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
                var cmd = GetSqlCommand(commandKey);
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
                var cmd = GetSqlCommand(commandKey);

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
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        protected T ExecuteScalar<T>(string commandKey, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            T result = default(T);

            try
            {
                if (string.IsNullOrEmpty(commandKey)) throw new ArgumentNullException("commandText");

                var cmd = GetSqlCommand(commandKey);
                result = base.ExecuteScalar<T>(cmd.CommandText, param as object, transaction, cmd.CommandType, commandTimeout);
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }
    }
}
