using FluentAssertions;
using NUnit.Framework;

namespace Enigmatry.BuildingBlocks.Infrastructure.Tests
{
    [Category("unit")]
    public class FixedTimeProviderFixture
    {
        [Test]
        public void SameTime()
        {
            var provider = new FixedTimeProvider();

            var firstDate = provider.Now;
            var secondDate = provider.Now;

            firstDate.Should().BeExactly(secondDate);
        }
    }
}
