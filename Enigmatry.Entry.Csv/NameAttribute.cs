using System;

namespace Enigmatry.Entry.Csv
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class NameAttribute : Attribute
    {
        public string Name { get; } = string.Empty;

        public NameAttribute()
        {
        }

        public NameAttribute(string name)
        {
            Name = name;
        }
    }
}
