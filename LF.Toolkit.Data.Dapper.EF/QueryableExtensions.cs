using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.Dapper
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string name, bool ascending = false) where T : class
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
