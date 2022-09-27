using Enigmatry.Entry.HealthChecks.Authorization;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NUnit.Framework;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Enigmatry.Entry.HealthChecks.Tests
{
    [Category("unit")]
    public class HealthChecksTokenHandlerFixture
    {
        private const string ValidToken = "123456";
        private const string InvalidToken = "abcdef";
        private AuthorizationHandlerContext _handlerContext = null!;

        [SetUp]
        public void Setup() =>
            _handlerContext = new(new[] { new TokenRequirement(ValidToken) },
                ClaimsPrincipal.Current!, null);

        [Test]
        public void ConstructorGuards()
        {
            Action act = () => _ = new TokenHandler(null!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public async Task NullContextAuthorizationFailure()
        {
            var contextAccessor = A.Fake<IHttpContextAccessor>();
            contextAccessor.HttpContext = null;
            var handler = new TokenHandler(contextAccessor);

            await handler.HandleAsync(_handlerContext);

            _handlerContext.HasSucceeded.Should().BeFalse();
        }

        [Test]
        public async Task InvalidTokenAuthorizationFailure()
        {
            var handler = ArrangeHandlerWith(InvalidToken);

            await handler.HandleAsync(_handlerContext);

            _handlerContext.HasSucceeded.Should().BeFalse();
        }

        [Test]
        public async Task ValidTokenAuthorizationSuccess()
        {
            var handler = ArrangeHandlerWith(ValidToken);

            await handler.HandleAsync(_handlerContext);

            _handlerContext.HasSucceeded.Should().BeTrue();
        }

        private static TokenHandler ArrangeHandlerWith(string token)
        {
            var contextAccessor = A.Fake<IHttpContextAccessor>();

            var request = new HttpRequestFeature { QueryString = $"token={token}" };
            var features = new FeatureCollection();
            features.Set<IHttpRequestFeature>(request);

            contextAccessor.HttpContext = new DefaultHttpContext(features);
            return new TokenHandler(contextAccessor);
        }
    }
}
