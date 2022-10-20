using FluentAssertions;
using NUnit.Framework;

namespace Enigmatry.Entry.Infrastructure.Tests;

[Category("unit")]
public class TimeProviderFixture
{
    [Test]
    public void SameTime()
    {
        var provider = new TimeProvider();

        var firstDate = provider.FixedUtcNow;
        var secondDate = provider.FixedUtcNow;

        firstDate.Should().BeExactly(secondDate);
    }

    [Test]
    public void UniqueTime()
    {
        var provider = new TimeProvider();

        var firstDate = provider.UtcNow;
        var secondDate = provider.UtcNow;

        firstDate.Should().BeBefore(secondDate);
        firstDate.Should().NotBeExactly(secondDate);
    }
}
