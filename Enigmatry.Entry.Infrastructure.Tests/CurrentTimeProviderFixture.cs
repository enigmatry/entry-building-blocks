using FluentAssertions;
using NUnit.Framework;

namespace Enigmatry.Entry.Infrastructure.Tests;

[Category("unit")]
public class CurrentTimeProviderFixture
{
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
