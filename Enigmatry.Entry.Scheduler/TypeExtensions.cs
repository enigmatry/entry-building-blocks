using System.Reflection;
using Quartz;

namespace Enigmatry.Entry.Scheduler;

internal static class TypeExtensions
{
    internal static IEnumerable<Type> FindAllJobTypes(this Assembly assembly) =>
        assembly.GetTypes()
            .Where(type => !type.IsAbstract)
            .Where(type => type.GetInterface(nameof(IJob)) != null)
            .Where(type => !type.GetGenericArguments().Any()); // exclude jobs that have generic type arguments, i.e. they cannot be instantiated
}
