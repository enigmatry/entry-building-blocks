using System;

namespace Enigmatry.Entry.Csv
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class IgnoreAttribute : Attribute
    {
    }
}
