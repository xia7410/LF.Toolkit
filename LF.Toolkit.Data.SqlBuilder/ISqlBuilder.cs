using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.SqlBuilder
{
    /// <summary>
    /// Sql语句生成器接口定义
    /// </summary>
    public interface ISqlBuilder
    {
        /// <summary>
        /// 新增语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">插入的列（必须）</param>
        /// <returns></returns>
        string Insert(string tableName, IEnumerable<ValueColumn> columns);

        /// <summary>
        /// 更新语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">更新列（必须）</param>
        /// <param name="cluaseColumns">条件列（必须）</param>
        /// <param name="occur">查询组合条件</param>
        /// <returns></returns>
        string Update(string tableName, IEnumerable<ValueColumn> columns, IEnumerable<ClauseColumn> cluaseColumns, Occur occur = Occur.AND);

        /// <summary>
        /// 删除语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="cluaseColumns">条件列（必须）</param>
        /// <param name="occur">查询组合条件</param>
        /// <returns></returns>
        string Delete(string tableName, IEnumerable<ClauseColumn> cluaseColumns, Occur occur = Occur.AND);

        /// <summary>
        /// 查询总数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="cluaseColumns">条件列（可选）</param>
        /// <param name="occur">查询组合条件</param>
        /// <returns></returns>
        string Count(string tableName, IEnumerable<ClauseColumn> cluaseColumns, Occur occur = Occur.AND);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">查询列（为空则默认：*）</param>
        /// <param name="cluaseColumns">条件列（可选）</param>
        /// <param name="sortColumns">排序列（可选）</param>
        /// <param name="occur">查询组合条件</param>
        /// <returns></returns>
        string List(string tableName, IEnumerable<string> columns, IEnumerable<ClauseColumn> cluaseColumns, IEnumerable<SortableColumn> sortColumns, Occur occur = Occur.AND);

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">查询列（为空则默认：*）</param>
        /// <param name="cluaseColumns">条件列（可选）</param>
        /// <param name="sortColumns">排序列（必须）（默认第一个作为分页排序字段）</param>
        /// <param name="occur">查询组合条件</param>
        /// <returns></returns>
        string PagedList(string tableName, IEnumerable<string> columns, IEnumerable<ClauseColumn> cluaseColumns, IEnumerable<SortableColumn> sortColumns, Occur occur = Occur.AND);
    }
}
