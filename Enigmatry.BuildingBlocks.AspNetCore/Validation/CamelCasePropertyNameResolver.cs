using System;
using System.Linq.Expressions;
using System.Reflection;
using Enigmatry.BuildingBlocks.Core.Helpers;
using FluentValidation.Internal;

namespace Enigmatry.BuildingBlocks.AspNetCore.Validation
{
    // Enables FluentValidation error messages to contain camel cased property names instead of pascal cased.
    // E.g. Instead of 'UserName' we get 'userName'
    public static class CamelCasePropertyNameResolver
    {
#pragma warning disable CA1801 // Review unused parameters
        public static string ResolvePropertyName(Type type, MemberInfo memberInfo, LambdaExpression? expression)
#pragma warning restore CA1801 // Review unused parameters
        {
            var propertyName = DefaultPropertyNameResolver(memberInfo, expression);

            return propertyName.ToCamelCase();
        }

        private static string DefaultPropertyNameResolver(MemberInfo memberInfo, LambdaExpression? expression)
        {
            if (expression != null)
            {
                var chain = PropertyChain.FromExpression(expression);
                if (chain.Count > 0)
                    return chain.ToString();
            }

            return memberInfo != null ? memberInfo.Name : String.Empty;
        }
    }
}
