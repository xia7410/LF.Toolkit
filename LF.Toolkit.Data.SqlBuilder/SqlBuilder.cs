using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.SqlBuilder
{
    #region 查询条件枚举

    /// <summary>
    /// 组合条件
    /// </summary>
    public enum Occur
    {
        AND,
        OR
    }

    /// <summary>
    /// 操作符
    /// </summary>
    public enum Operators
    {
        EQ,
        NEQ,
        GT,
        GTE,
        LT,
        LTE,
        IN,
        LIKE
    }

    #endregion

    #region 列相关

    /// <summary>
    /// 列
    /// </summary>
    public class Column
    {
        /// <summary>
        /// 获取或设置列名
        /// </summary>
        public string Name { get; set; }

        public Column(string name)
        {
            this.Name = name;
        }
    }

    /// <summary>
    /// 带值的列
    /// </summary>
    public class ValueColumn : Column
    {
        /// <summary>
        /// 获取或设置列值
        /// </summary>
        public object Value { get; set; }

        public ValueColumn(string name, object value)
            : base(name)
        {
            this.Value = value;
        }
    }

    /// <summary>
    /// 条件列
    /// </summary>
    public class ClauseColumn : ValueColumn
    {
        public Operators Operator { get; set; }

        public ClauseColumn(string name, object value, Operators op = Operators.EQ)
            : base(name, value)
        {
            this.Operator = op;
        }
    }

    /// <summary>
    /// 排序列
    /// </summary>
    public class SortableColumn : Column
    {
        /// <summary>
        /// 获取或设置是否倒序
        /// </summary>
        public bool Reverse { get; set; }

        public SortableColumn(string name, bool reverse = true)
            : base(name)
        {
            this.Reverse = reverse;
        }
    }

    #endregion

    /// <summary>
    /// Sql语句生成基类
    /// </summary>
    public abstract class SqlBuilder : ISqlBuilder
    {
        /// <summary>
        /// 条件组合
        /// </summary>
        protected static readonly IDictionary<Occur, string> m_Occurs = new Dictionary<Occur, string>
        {
            { Occur.AND, " AND " },
            { Occur.OR, " OR " },
        };

        /// <summary>
        /// 操作符
        /// </summary>
        protected static readonly IDictionary<Operators, string> m_Operators = new Dictionary<Operators, string>
        {
            { Operators.EQ, " = " },
            { Operators.NEQ, " <> " },
            { Operators.GT, " > " },
            { Operators.GTE, " >= " },
            { Operators.LT, " < " },
            { Operators.LTE, " <= " },
            { Operators.IN, " IN " },
            { Operators.LIKE, " LIKE " },
        };

        /// <summary>
        /// 新增语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">插入的列（必须）</param>
        /// <returns></returns>
        public string Insert(string tableName, IEnumerable<ValueColumn> columns)
        {
            if (columns == null || columns.Count() == 0) throw new Exception("未定义插入的列");

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(tableName);
            sb.Append(" ( ");
            sb.Append(string.Join(", ", columns.Select(i => i.Name)));
            sb.Append(" ) ");
            sb.Append(" VALUES ( ");
            sb.Append(string.Join(", ", columns.Select(i => "@" + i.Name)));
            sb.Append(" ); ");

            return sb.ToString();
        }

        /// <summary>
        /// 更新语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">更新列（必须）</param>
        /// <param name="cluaseColumns">条件列（必须）</param>
        /// <param name="occur">查询组合条件</param>
        /// <returns></returns>
        public string Update(string tableName, IEnumerable<ValueColumn> columns, IEnumerable<ClauseColumn> cluaseColumns, Occur occur = Occur.AND)
        {
            if (columns == null || columns.Count() == 0) throw new Exception("未定义更新的列");
            if (cluaseColumns == null || cluaseColumns.Count() == 0) throw new Exception("未定义更新的条件列");

            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE ");
            sb.Append(tableName);
            sb.Append(" SET ");
            //更新列
            var fieldList = new List<string>();
            foreach (var col in columns)
            {
                fieldList.Add(col.Name + " = @" + col.Name);
            }
            //条件列
            var cluaseList = new List<string>();
            foreach (var col in cluaseColumns)
            {
                cluaseList.Add(string.Format("{0}{1}@{2}", col.Name, m_Operators[col.Operator], col.Name));
            }
            sb.Append(string.Join(", ", fieldList));
            sb.Append(" WHERE ");
            sb.Append(string.Join(m_Occurs[occur], cluaseList));

            return sb.ToString();
        }

        /// <summary>
        /// 删除语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="cluaseColumns">条件列（必须）</param>
        /// <param name="occur">查询组合条件</param>
        /// <returns></returns>
        public string Delete(string tableName, IEnumerable<ClauseColumn> cluaseColumns, Occur occur = Occur.AND)
        {
            if (cluaseColumns == null || cluaseColumns.Count() == 0) throw new Exception("未设置删除语句的条件");

            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM ");
            sb.Append(tableName);
            sb.Append(" WHERE ");

            var cluaseList = new List<string>();
            foreach (var col in cluaseColumns)
            {
                cluaseList.Add(string.Format("{0}{1}@{2}", col.Name, m_Operators[col.Operator], col.Name));
            }
            sb.Append(string.Join(m_Occurs[occur], cluaseList));

            return sb.ToString();
        }

        /// <summary>
        /// 查询总数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="cluaseColumns">条件列（可选）</param>
        /// <param name="occur">查询组合条件</param>
        /// <returns></returns>
        public string Count(string tableName, IEnumerable<ClauseColumn> cluaseColumns, Occur occur = Occur.AND)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COUNT(1) FROM ");
            sb.Append(tableName);
            if (cluaseColumns != null && cluaseColumns.Count() > 0)
            {
                sb.Append(" WHERE ");
                var cluaseList = new List<string>();
                foreach (var col in cluaseColumns)
                {
                    cluaseList.Add(string.Format("{0}{1}@{2}", col.Name, m_Operators[col.Operator], col.Name));
                }
                sb.Append(string.Join(m_Occurs[occur], cluaseList));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">查询列（为空则默认：*）</param>
        /// <param name="cluaseColumns">条件列（可选）</param>
        /// <param name="sortColumns">排序列（可选）</param>
        /// <param name="occur">查询组合条件</param>
        /// <returns></returns>
        public string List(string tableName, IEnumerable<string> columns, IEnumerable<ClauseColumn> cluaseColumns, IEnumerable<SortableColumn> sortColumns, Occur occur = Occur.AND)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            //查询列
            if (columns != null && columns.Count() > 0)
            {
                sb.Append(string.Join(", ", columns));
            }
            else
            {
                sb.Append(" * ");
            }
            sb.Append(" FROM ");
            sb.Append(tableName);
            //条件列
            if (cluaseColumns != null && cluaseColumns.Count() > 0)
            {
                var cluaseList = new List<string>();
                foreach (var col in cluaseColumns)
                {
                    cluaseList.Add(string.Format("{0}{1}@{2}", col.Name, m_Operators[col.Operator], col.Name));
                }
                sb.Append(" WHERE ");
                sb.Append(string.Join(m_Occurs[occur], cluaseList));
            }
            //排序列
            if (sortColumns != null && sortColumns.Count() > 0)
            {
                sb.Append(" ORDER BY ");
                sb.Append(string.Join(", ", sortColumns.Select(i => i.Name + (i.Reverse ? " DESC " : " ASC "))));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">查询列（为空则默认：*）</param>
        /// <param name="cluaseColumns">条件列（可选）</param>
        /// <param name="sortColumns">排序列（必须）（默认第一个作为分页排序字段）</param>
        /// <param name="occur">查询组合条件</param>
        /// <returns></returns>
        public abstract string PagedList(string tableName, IEnumerable<string> columns, IEnumerable<ClauseColumn> cluaseColumns, IEnumerable<SortableColumn> sortColumns, Occur occur = Occur.AND);
    }
}
