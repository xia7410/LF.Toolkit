using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.Dapper
{
    /// <summary>
    /// 查询源扩展
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// 查询源排序扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">排序源</param>
        /// <param name="name">排序字段名称</param>
        /// <param name="ascending">是否升序; True=升序 False=降序</param>
        /// <returns></returns>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string name, bool ascending = false) 
            where T : class
        {
            var param = Expression.Parameter(typeof(T), "c");
            var property = typeof(T).GetProperty(name);
            var propertyAccessExpression = Expression.MakeMemberAccess(param, property);
            var le = Expression.Lambda(propertyAccessExpression, param);
            var type = typeof(T);
            var resultExp = Expression.Call(typeof(Queryable), ascending ? "OrderBy" : "OrderByDescending", new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(le));

            return source.Provider.CreateQuery<T>(resultExp);
        }
    }
}
