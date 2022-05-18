using System;

namespace Enigmatry.BuildingBlocks.Csv
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IgnoreAttribute : Attribute
    {
    }
}
