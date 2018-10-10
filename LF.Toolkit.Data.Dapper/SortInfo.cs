using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.Dapper
{
    /// <summary>
    /// 排序信息类
    /// </summary>
    public class SortInfo
    {
        /// <summary>
        /// 获取或设置排序字段名
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// 获取或设置是否升序
        /// </summary>
        public bool Ascending { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="ascending"></param>
        public SortInfo(string column, bool ascending = false)
        {
            this.Column = column;
            this.Ascending = ascending;
        }

        /// <summary>
        /// 返回SQL ORDER BY 语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"ORDER BY {this.Column} {(this.Ascending ? "ASC" : "DESC")}";
        }

        /// <summary>
        /// 将指定排序规则语句转换为排序对象
        /// </summary>
        /// <param name="orderBy">格式为：field_(DESC/ASC)</param>
        /// <returns>若转换失败则返回NULL</returns>
        public static SortInfo Parse(string orderBy)
        {
            SortInfo sortInfo = null;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                string field = "";
                bool ascending = false;
                if (orderBy.IndexOf("_ASC", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    ascending = true;
                    field = orderBy.Substring(0, orderBy.IndexOf("_ASC", StringComparison.OrdinalIgnoreCase));
                }
                else if (orderBy.IndexOf("_DESC", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    ascending = false;
                    field = orderBy.Substring(0, orderBy.IndexOf("_DESC", StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(field))
                {
                    sortInfo = new SortInfo(field, ascending);
                }
            }

            return sortInfo;
        }
    }

    /// <summary>
    /// 带有别名的字段信息
    /// </summary>
    public class AliasColumn
    {
        /// <summary>
        /// 获取或设置排序字段
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// 获取或设置字段别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        public AliasColumn(string column)
            : this(column, "")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="alias"></param>
        public AliasColumn(string column, string alias)
        {
            this.Column = column;
            this.Alias = alias;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(this.Alias) ? this.Column : this.Alias + "." + this.Column;
        }
    }
}
