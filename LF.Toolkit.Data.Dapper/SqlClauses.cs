using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.Dapper
{
    /// <summary>
    /// SQL查询条件集
    /// </summary>
    public class SqlClauses
    {
        string m_Prefix = null;
        StringBuilder m_Builder = new StringBuilder();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix">查询条件前缀</param>
        public SqlClauses(string prefix = "WHERE")
        {
            this.m_Prefix = prefix;
        }

        /// <summary>
        /// 生成一个新的查询条件实例
        /// </summary>
        /// <param name="prefix">查询条件前缀</param>
        /// <returns></returns>
        public static SqlClauses Create(string prefix = "WHERE") => new SqlClauses(prefix);

        /// <summary>
        /// 添加查询条件
        /// </summary>
        /// <param name="condition">查询条件主体</param>
        /// <param name="sqlOp">运算符 AND、OR</param>
        /// <param name="useCondition">是否使用查询条件</param>
        /// <returns></returns>
        SqlClauses AddClause(string condition, string sqlOp, bool useCondition)
        {
            if (string.IsNullOrEmpty(condition)) throw new ArgumentNullException("condition");

            if (useCondition)
            {
                if (m_Builder.Length <= 0 && !string.IsNullOrEmpty(m_Prefix))
                {
                    m_Builder.Append($" {m_Prefix} ");
                }
                else if (m_Builder.Length > 0)
                {
                    m_Builder.Append($" {sqlOp} ");
                }
                // 添加括号分隔
                m_Builder.Append($" ( {condition} ) ");
            }

            return this;
        }

        /// <summary>
        /// 以AND方式添加查询条件
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="useCondition"></param>
        /// <returns></returns>
        public SqlClauses AndCaluse(string condition, bool useCondition)
            => AddClause(condition, "AND", useCondition);

        /// <summary>
        /// 以OR方式添加查询条件
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="useCondition"></param>
        /// <returns></returns>
        public SqlClauses OrCaluse(string condition, bool useCondition)
            => AddClause(condition, "OR", useCondition);

        /// <summary>
        /// 生成查询条件
        /// </summary>
        /// <returns></returns>
        public string ToSql() => m_Builder.ToString();
    }
}
