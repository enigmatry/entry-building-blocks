using System.Reflection;
using Quartz;

namespace Enigmatry.Entry.Scheduler;

internal static class TypeExtensions
{
    internal static IEnumerable<Type> FinAllJobTypes(this Assembly assembly) =>
        assembly.GetTypes()
            .Where(type => !type.IsAbstract)
            .Where(type => type.GetInterface(nameof(IJob)) != null);
}
