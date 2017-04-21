using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.SqlBuilder.Impl
{
    /// <summary>
    /// SqlServer语句生成实现类
    /// </summary>
    public class SqlServerSqlBuilder : SqlBuilder
    {
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">查询列（为空则默认：*）</param>
        /// <param name="cluaseColumns">条件列（可选）</param>
        /// <param name="sortColumns">排序列（必须）（默认第一个作为分页排序字段）</param>
        /// <param name="occur">查询组合条件</param>
        /// <returns></returns>
        public override string PagedList(string tableName, IEnumerable<string> columns, IEnumerable<ClauseColumn> cluaseColumns, IEnumerable<SortableColumn> sortColumns, Occur occur = Occur.AND)
        {
            if (sortColumns == null || sortColumns.Count() == 0) throw new Exception("未设置查询语句的排序列");

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM(SELECT ROW_NUMBER() OVER(ORDER BY ");
            //row number sort
            var sort = sortColumns.First();
            sb.Append(sort.Name + " " + (sort.Reverse ? " DESC " : " ASC "));
            sb.Append(" ) AS row, ");
            //查询列
            if (columns == null || columns.Count() == 0)
            {
                sb.Append(" * ");
            }
            else
            {
                sb.Append(string.Join(", ", columns));
            }
            sb.Append(" FROM ");
            sb.Append(tableName);
            //条件列
            var cluaseList = new List<string>();
            if (cluaseColumns != null && cluaseColumns.Count() > 0)
            {
                foreach (var col in cluaseColumns)
                {
                    cluaseList.Add(string.Format("{0}{1}@{2}", col.Name, m_Operators[col.Operator], col.Name));
                }
                sb.Append(" WHERE ");
                sb.Append(string.Join(m_Occurs[occur], cluaseList));
            }
            sb.Append(" ) t WHERE t.row BETWEEN (@page -1) * @pageSize + 1 AND @page * @pageSize;");
            //count
            sb.Append(" SELECT COUNT(1) FROM ");
            sb.Append(tableName);
            if (cluaseList.Count > 0)
            {
                sb.Append(" WHERE ");
                sb.Append(string.Join(m_Occurs[occur], cluaseList));
            }

            return sb.ToString();
        }
    }
}
