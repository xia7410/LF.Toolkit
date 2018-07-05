using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.Dapper
{
    /// <summary>
    /// 
    /// </summary>
    internal class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _oldValue)
                return _newValue;
            return base.Visit(node);
        }
    }

    /// <summary>
    /// 查询表达式扩展
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// 返回一个空的执行结果为True的表达式，用于构建新的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Empty<T>()
        {
            return i => true;
        }

        /// <summary>
        /// 根据指定条件以And方式合并表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1">表达式源</param>
        /// <param name="expr2">待合并的表达式</param>
        /// <param name="useCondition">是否合并表达式</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> WhereIf<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2, bool useCondition)
        {
            if (!useCondition)
            {
                return expr1;
            }
            else
            {
                var parameter = Expression.Parameter(typeof(T));
                //左边表达式
                var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
                var left = leftVisitor.Visit(expr1.Body);
                //右边表达式
                var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
                var right = rightVisitor.Visit(expr2.Body);

                return Expression.Lambda<Func<T, bool>>(Expression.And(left, right), parameter);
            }
        }
    }
}
