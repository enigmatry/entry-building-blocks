using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.Helpers
{
    public static class LambdaExpressionExtensions
    {
        public static PropertyInfo GetPropertyInfo(this LambdaExpression propertyAccessExpression) =>
            GetInternalMemberAccess<PropertyInfo>(propertyAccessExpression);

        private static TMemberInfo GetInternalMemberAccess<TMemberInfo>(this LambdaExpression memberAccessExpression)
            where TMemberInfo : MemberInfo
        {
            var parameterExpression = memberAccessExpression.Parameters[0];
            var memberInfo = parameterExpression.MatchSimpleMemberAccess<TMemberInfo>(memberAccessExpression.Body);

            if (memberInfo == null)
            {
                throw new ArgumentException(nameof(memberInfo));
            }

            var declaringType = memberInfo.DeclaringType;
            var parameterType = parameterExpression.Type;

            if (declaringType != null
                && declaringType != parameterType
                && declaringType.IsInterface
                && declaringType.IsAssignableFrom(parameterType)
                && memberInfo is PropertyInfo propertyInfo)
            {
                var propertyGetter = propertyInfo.GetMethod;
                var interfaceMapping = parameterType.GetTypeInfo().GetRuntimeInterfaceMap(declaringType);
                var index = Array.FindIndex(interfaceMapping.InterfaceMethods, p => p.Equals(propertyGetter));
                var targetMethod = interfaceMapping.TargetMethods[index];
                foreach (var runtimeProperty in parameterType.GetRuntimeProperties())
                {
                    if (targetMethod.Equals(runtimeProperty.GetMethod))
                    {
                        return (TMemberInfo)(object)runtimeProperty;
                    }
                }
            }

            return memberInfo;
        }

        private static TMemberInfo? MatchSimpleMemberAccess<TMemberInfo>(
            this Expression parameterExpression,
            Expression memberAccessExpression)
            where TMemberInfo : MemberInfo
        {
            var memberInfos = MatchMemberAccess<TMemberInfo>(parameterExpression, memberAccessExpression);
            return memberInfos?.Count == 1 ? memberInfos[0] : null;
        }

        private static IReadOnlyList<TMemberInfo>? MatchMemberAccess<TMemberInfo>(
            this Expression parameterExpression,
            Expression memberAccessExpression)
            where TMemberInfo : MemberInfo
        {
            var memberInfos = new List<TMemberInfo>();

            var unwrappedExpression = RemoveTypeAs(RemoveConvert(memberAccessExpression));
            do
            {
                var memberExpression = unwrappedExpression as MemberExpression;

                if (memberExpression?.Member is not TMemberInfo memberInfo)
                {
                    return null;
                }

                memberInfos.Insert(0, memberInfo);

                unwrappedExpression = RemoveTypeAs(RemoveConvert(memberExpression.Expression));
            } while (unwrappedExpression != parameterExpression);

            return memberInfos;
        }

        private static Expression? RemoveTypeAs(this Expression? expression)
        {
            while (expression?.NodeType == ExpressionType.TypeAs)
            {
                expression = ((UnaryExpression)RemoveConvert(expression)!).Operand;
            }

            return expression;
        }

        private static Expression? RemoveConvert(Expression? expression)
        {
            return expression is UnaryExpression unaryExpression
                && (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
                    ? RemoveConvert(unaryExpression.Operand)
                    : expression;
        }
    }
}
