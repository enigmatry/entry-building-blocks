using FluentAssertions;
using NUnit.Framework;

namespace Enigmatry.Entry.Infrastructure.Tests;

[Category("unit")]
public class TimeProviderFixture
{
    [Test]
    public async Task SameTime()
    {
        var provider = new TimeProvider();

        var firstDate = provider.FixedUtcNow;
        await Task.Delay(TimeSpan.FromMilliseconds(1));
        var secondDate = provider.FixedUtcNow;

        firstDate.Should().BeExactly(secondDate);
    }

    [Test]
    public async Task UniqueTime()
    {
        var provider = new TimeProvider();

        var firstDate = provider.UtcNow;
        await Task.Delay(TimeSpan.FromMilliseconds(1));
        var secondDate = provider.UtcNow;

        firstDate.Should().BeBefore(secondDate);
        firstDate.Should().NotBeExactly(secondDate);
    }
}
