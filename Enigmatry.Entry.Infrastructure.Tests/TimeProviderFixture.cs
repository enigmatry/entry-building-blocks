using NUnit.Framework;
using Shouldly;

namespace Enigmatry.Entry.Infrastructure.Tests;

[Category("unit")]
public class TimeProviderFixture
{
    [Test]
    public void SameTime()
    {
        var provider = new TimeProvider();

        var firstDate = provider.FixedUtcNow;
        Thread.Sleep(TimeSpan.FromMilliseconds(100));
        var secondDate = provider.FixedUtcNow;

        firstDate.ShouldBe(secondDate);
    }

    [Test]
    public void UniqueTime()
    {
        var provider = new TimeProvider();

        var firstDate = provider.UtcNow;
        Thread.Sleep(TimeSpan.FromMilliseconds(100));
        var secondDate = provider.UtcNow;

        firstDate.ShouldBeLessThan(secondDate);
        firstDate.ShouldNotBe(secondDate);
    }
}
