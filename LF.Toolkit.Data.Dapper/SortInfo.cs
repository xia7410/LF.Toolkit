using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.Dapper
{
    /// <summary>
    /// 排序类型枚举
    /// </summary>
    public enum SortType
    {
        ASC,
        DESC
    }

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
        /// 获取或设置排序类型
        /// </summary>
        public SortType Type { get; set; }

        public SortInfo(string column, SortType type = SortType.DESC)
        {
            this.Column = column;
            this.Type = type;
        }

        public override string ToString()
        {
            return $"ORDER BY {this.Column} {this.Type}";
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
                SortType type = SortType.DESC;
                if (orderBy.IndexOf("ASC", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    type = SortType.ASC;
                    field = orderBy.Substring(0, orderBy.IndexOf("ASC", StringComparison.OrdinalIgnoreCase) - 1);
                }
                else if (orderBy.IndexOf("DESC", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    type = SortType.DESC;
                    field = orderBy.Substring(0, orderBy.IndexOf("DESC", StringComparison.OrdinalIgnoreCase) - 1);
                }
                if (!string.IsNullOrEmpty(field))
                {
                    sortInfo = new SortInfo(field, type);
                }
            }

            return sortInfo;
        }
    }

    /// <summary>
    /// 排序字段
    /// </summary>
    public class SortColumn
    {
        /// <summary>
        /// 获取或设置排序字段
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// 获取或设置字段别名
        /// </summary>
        public string Alias { get; set; }

        public SortColumn(string column)
            : this(column, "")
        {

        }

        public SortColumn(string column, string alias)
        {
            this.Column = column;
            this.Alias = alias;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(this.Alias) ? this.Column : this.Alias + "." + this.Column;
        }
    }
}
