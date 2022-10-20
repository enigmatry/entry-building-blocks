using FluentAssertions;
using NUnit.Framework;

namespace Enigmatry.Entry.Infrastructure.Tests;

[Category("unit")]
public class FixedTimeProviderFixture
{
    [Test]
    public void SameTime()
    {
        var provider = new TimeProvider();

        var firstDate = provider.FixedUtcNow;
        var secondDate = provider.FixedUtcNow;

        firstDate.Should().BeExactly(secondDate);
    }
}
