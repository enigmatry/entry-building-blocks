using FluentAssertions;
using NUnit.Framework;

namespace Enigmatry.BuildingBlocks.Infrastructure.Tests
{
    [Category("unit")]
    public class CurrentTimeProviderFixture
    {
        [Test]
        public void UniqueTime()
        {
            var provider = new CurrentTimeProvider();

            var firstDate = provider.Now;
            var secondDate = provider.Now;

            firstDate.Should().BeBefore(secondDate);
            firstDate.Should().NotBeExactly(secondDate);
        }
    }
}
