using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.Helpers
{
    internal static class Extensions
    {
        public static bool IsNumber(this Type type) =>
            new List<Type>
            {
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(decimal),
                typeof(double),
                typeof(float),
                typeof(byte)
            }
            .Any(x => x == type);

        public static bool NotNumber(this Type type) => !type.IsNumber();

        public static bool IsEmpty(this string value) => String.IsNullOrWhiteSpace(value);

        public static bool NotEmpty(this string value) => !value.IsEmpty();

        // Stolen from FluentValidation ;)
        public static PropertyInfo? TryGetProperty<T, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            if (RemoveUnary(expression.Body) is not MemberExpression memberExp)
            {
                return null;
            }

            Expression currentExpr = memberExp.Expression;

            // Unwind the expression to get the root object that the expression acts upon.
            while (true)
            {
                currentExpr = RemoveUnary(currentExpr);

                if (currentExpr != null && currentExpr.NodeType == ExpressionType.MemberAccess)
                {
                    currentExpr = ((MemberExpression)currentExpr).Expression;
                }
                else
                {
                    break;
                }
            }

            if (currentExpr == null || currentExpr.NodeType != ExpressionType.Parameter)
            {
                return null; // We don't care if we're not acting upon the model instance.
            }

            return memberExp.Member is PropertyInfo info ? info : null;
        }

        private static Expression RemoveUnary(Expression toUnwrap) =>
            toUnwrap is UnaryExpression expression ? expression.Operand : toUnwrap;
    }
}
