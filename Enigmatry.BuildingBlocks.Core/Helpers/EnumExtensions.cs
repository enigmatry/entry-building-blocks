using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Enigmatry.BuildingBlocks.Core.Helpers
{
    [PublicAPI]
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var enumType = value.GetType();

            var memberInfo = enumType.GetMember(value.ToString());
            Attribute? attribute = memberInfo
                .First()
                .GetCustomAttribute(typeof(DisplayAttribute));

            return attribute is DisplayAttribute attr ? attr.Name : String.Empty;
        }

        public static string GetDescription<TEnum>(this TEnum o) => o.GetAttribute<TEnum, DescriptionAttribute>().Description;

        public static TDescriptionAttribute GetAttribute<TEnum, TDescriptionAttribute>(this TEnum o)
            where TDescriptionAttribute : DescriptionAttribute
        {
            TDescriptionAttribute? result = FindAttribute<TEnum, TDescriptionAttribute>(o);
            Type attributeType = typeof(TDescriptionAttribute);
            return result ?? throw new InvalidOperationException($"Attribute of type {attributeType} was not found");
        }

        private static TDescriptionAttribute? FindAttribute<TEnum, TDescriptionAttribute>(this TEnum o)
            where TDescriptionAttribute : DescriptionAttribute
        {
            Type enumType = o!.GetType();
            FieldInfo? field = enumType.GetField(o.ToString() ?? String.Empty);
            Type attributeType = typeof(TDescriptionAttribute);
            var attributes = field != null ? field.GetCustomAttributes(attributeType, false) : Array.Empty<object>();
            return attributes.Length == 0 ? null : (TDescriptionAttribute)attributes[0];
        }
    }
}
