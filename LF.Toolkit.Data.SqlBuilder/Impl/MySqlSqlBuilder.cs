using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.SqlBuilder.Impl
{
    /// <summary>
    /// MySQL语句生成实现类
    /// </summary>
    public class MySqlSqlBuilder : SqlBuilder
    {
        public override string PagedList(string tableName, IEnumerable<string> columns, IEnumerable<ClauseColumn> cluaseColumns, IEnumerable<SortableColumn> sortColumns, Occur occur = Occur.AND)
        {
            if (sortColumns == null || sortColumns.Count() == 0) throw new Exception("未设置查询语句的排序列");

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
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
            //order by
            if (sortColumns != null && sortColumns.Count() > 0)
            {
                sb.Append(" ORDER BY ");
                var sortList = new List<string>();
                foreach (var sort in sortColumns)
                {
                    sortList.Add(sort.Name + " " + (sort.Reverse ? " DESC " : " ASC "));
                }
                sb.Append(string.Join(", ", sortList));
            }
            //limit
            sb.Append("  LIMIT (@page - 1) * @pageSize, @pageSize ");
            //count
            sb.Append(" ; SELECT COUNT(1) FROM ");
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
