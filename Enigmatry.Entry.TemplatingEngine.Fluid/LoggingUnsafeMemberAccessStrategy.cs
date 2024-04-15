using Fluid;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.TemplatingEngine.Liquid;

internal class LoggingUnsafeMemberAccessStrategy(ILogger logger) : DefaultMemberAccessStrategy
{
    public override IMemberAccessor GetAccessor(Type type, string name)
    {
        var strategy = UnsafeMemberAccessStrategy.Instance;
        strategy.MemberNameStrategy = MemberNameStrategy;
        strategy.IgnoreCasing = IgnoreCasing;

        var accessor = strategy.GetAccessor(type, name);
        if (accessor == null)
        {
            logger.LogWarning("Unknown member {MemberName} for type {TypeName}", name, type.Name);
        }

        return accessor!;
    }
}
