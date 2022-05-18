using System;

namespace Enigmatry.BuildingBlocks.Csv
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NameAttribute : Attribute
    {
        public string Name { get; } = String.Empty;

        public NameAttribute()
        {
        }

        public NameAttribute(string name)
        {
            Name = name;
        }
    }
}
