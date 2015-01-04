
using System.Data;

namespace LF.Toolkit.Data.Storage
{
    public interface IStorage
    {
        /// <summary>
        /// 数据库连接配置
        /// </summary>
        /// <param name="connectionKey">连接配置项名称</param>
        void DbProviderConfig(string connectionKey);

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        /// <returns></returns>
        IDbConnection GetDbConnection();

        /// <summary>
        /// 开始数据库事务
        /// </summary>
        /// <returns></returns>
        IDbTransaction BeginTransaction();

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <param name="connection"></param>
        void CloseDbConnection(IDbConnection connection);

        /// <summary>
        /// 关闭数据库事务
        /// </summary>
        /// <param name="transaction"></param>
        void CloseDbTransaction(IDbTransaction transaction);
    }
}
