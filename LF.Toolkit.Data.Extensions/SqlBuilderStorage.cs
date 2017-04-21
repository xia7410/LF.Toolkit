using LF.Toolkit.Data.SqlBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.Extensions
{
    public class SqlBuilderStorage : SqlStorageBase
    {
        ISqlBuilder m_SqlBuilder;
        string m_TableName;

        public SqlBuilderStorage(string connectionKey, string tableName, ISqlBuilder builder)
            : base(connectionKey)
        {
            m_SqlBuilder = builder;
            m_TableName = tableName;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="columns">插入的列（必须）</param>
        /// <returns></returns>
        public Task<int> InsertAsync(IEnumerable<ValueColumn> columns)
        {
            string sql = m_SqlBuilder.Insert(m_TableName, columns);
            object param = columns.ToDictionary(i => i.Name, i => i.Value);

            return base.ExecuteAsync(sql, param);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="columns">更新列（必须）</param>
        /// <param name="cluaseColumns">条件列（必须）</param>
        /// <param name="occur">查询组合条件</param>
        /// <returns></returns>
        public Task<int> UpdateAsync(IEnumerable<ValueColumn> columns, IEnumerable<ClauseColumn> cluaseColumns, Occur occur = Occur.AND)
        {
            string sql = m_SqlBuilder.Update(m_TableName, columns, cluaseColumns, occur);
            var dict = columns.ToDictionary(i => i.Name, i => i.Value);

            //条件
            foreach (var cluase in cluaseColumns)
            {
                if (cluase.Operator == Operators.IN)
                {
                    if (!(cluase.Value is IEnumerable)) throw new Exception("IN操作符传入的值应为集合");
                }
                else if (cluase.Operator == Operators.LIKE)
                {
                    cluase.Value = "%" + cluase.Value + "%";
                }
                dict[cluase.Name] = cluase.Value;
            }

            return base.ExecuteAsync(sql, dict);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="cluaseColumns">条件列（必须）</param>
        /// <param name="occur">查询组合条件</param>
        /// <returns></returns>
        public Task<int> DeleteAsync(IEnumerable<ClauseColumn> cluaseColumns, Occur occur = Occur.AND)
        {
            string sql = m_SqlBuilder.Delete(m_TableName, cluaseColumns, occur);
            var dict = new Dictionary<string, object>();
            //条件
            foreach (var cluase in cluaseColumns)
            {
                if (cluase.Operator == Operators.IN)
                {
                    if (!(cluase.Value is IEnumerable)) throw new Exception("IN操作符传入的值应为集合");
                }
                else if (cluase.Operator == Operators.LIKE)
                {
                    cluase.Value = "%" + cluase.Value + "%";
                }
                dict[cluase.Name] = cluase.Value;
            }

            return base.ExecuteAsync(sql, dict);
        }

        /// <summary>
        /// 查询总数
        /// </summary>
        /// <param name="cluaseColumns">条件列（可选）</param>
        /// <param name="occur">查询组合条件</param>
        /// <returns></returns>
        public Task<int> CountAsync(IEnumerable<ClauseColumn> cluaseColumns, Occur occur = Occur.AND)
        {
            string sql = m_SqlBuilder.Count(m_TableName, cluaseColumns, occur);
            var dict = new Dictionary<string, object>();
            //条件
            foreach (var cluase in cluaseColumns)
            {
                if (cluase.Operator == Operators.IN)
                {
                    if (!(cluase.Value is IEnumerable)) throw new Exception("IN操作符传入的值应为集合");
                }
                else if (cluase.Operator == Operators.LIKE)
                {
                    cluase.Value = "%" + cluase.Value + "%";
                }
                dict[cluase.Name] = cluase.Value;
            }

            return base.ExecuteScalarAsync<int>(sql, dict);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns">查询列（为空则默认：*）</param>
        /// <param name="cluaseColumns">条件列（可选）</param>
        /// <param name="sortColumns">排序列（可选）</param>
        /// <param name="occur">查询组合条件</param>
        /// <returns></returns>
        public Task<IEnumerable<T>> GetListAsync<T>(IEnumerable<string> columns, IEnumerable<ClauseColumn> cluaseColumns, IEnumerable<SortableColumn> sortColumns, Occur occur = Occur.AND)
        {
            string sql = m_SqlBuilder.List(m_TableName, columns, cluaseColumns, sortColumns, occur);

            IDictionary<string, object> dict = null;
            if (cluaseColumns != null && cluaseColumns.Count() > 0)
            {
                dict = new Dictionary<string, object>();
                //条件
                foreach (var cluase in cluaseColumns)
                {
                    if (cluase.Operator == Operators.IN)
                    {
                        if (!(cluase.Value is IEnumerable)) throw new Exception("IN操作符传入的值应为集合");
                    }
                    else if (cluase.Operator == Operators.LIKE)
                    {
                        cluase.Value = "%" + cluase.Value + "%";
                    }
                    dict[cluase.Name] = cluase.Value;
                }
            }

            return base.QueryAsync<T>(sql, dict);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns">查询列（为空则默认：*）</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页显示的条数</param>
        /// <param name="cluaseColumns">条件列（可选）</param>
        /// <param name="sortColumns">排序列（必须）（默认第一个作为分页排序字段）</param>
        /// <param name="occur">查询组合条件</param>
        /// <returns></returns>
        public async Task<PagedList<T>> GetPagedListAsync<T>(IEnumerable<string> columns, int page, int pageSize, IEnumerable<ClauseColumn> cluaseColumns, IEnumerable<SortableColumn> sortColumns, Occur occur = Occur.AND)
        {
            string sql = m_SqlBuilder.PagedList(m_TableName, columns, cluaseColumns, sortColumns, occur);

            IDictionary<string, object> dict = null;
            if (cluaseColumns != null && cluaseColumns.Count() > 0)
            {
                dict = new Dictionary<string, object>();
                //条件
                foreach (var cluase in cluaseColumns)
                {
                    if (cluase.Operator == Operators.IN)
                    {
                        if (!(cluase.Value is IEnumerable)) throw new Exception("IN操作符传入的值应为集合");
                    }
                    else if (cluase.Operator == Operators.LIKE)
                    {
                        cluase.Value = "%" + cluase.Value + "%";
                    }
                    dict[cluase.Name] = cluase.Value;
                }
            }
            if (dict == null)
            {
                dict = new Dictionary<string, object>();
            }
            dict["page"] = page;
            dict["pageSize"] = pageSize;

            var connection = base.GetDbConnection();
            var pageList = new PagedList<T>();

            try
            {
                var grid = await base.QueryMultipleAsync(sql, connection, dict);
                pageList.RowSet = grid.Read<T>();
                pageList.Count = grid.Read<int>().First();
            }
            finally
            {
                base.CloseDbConnection(connection);
            }

            return pageList;
        }
    }
}
