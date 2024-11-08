using Enigmatry.Entry.Core.Cqrs;
using FluentAssertions;
using JetBrains.Annotations;
using MediatR;
using NUnit.Framework;

namespace Enigmatry.Entry.Core.Tests.Cqrs;

[Category("unit")]
public class TypeExtensionsFixture
{
    [TestCase(typeof(AQuery), true)]
    [TestCase(typeof(ARequest), false)]
    [TestCase(typeof(ACommand), false)]
    public void IsQuery(Type type, bool expectedResult)
    {
        var request = (IBaseRequest)Activator.CreateInstance(type)!;
        request.IsQuery().Should().Be(expectedResult);
    }

    [TestCase(typeof(AQuery), false)]
    [TestCase(typeof(ARequest), false)]
    [TestCase(typeof(ACommand), true)]
    public void IsCommand(Type type, bool expectedResult)
    {
        var request = (IBaseRequest)Activator.CreateInstance(type)!;
        request.IsCommand().Should().Be(expectedResult);
    }

    private class AQuery : IQuery<AResponse>;

    private class ARequest : IRequest<AResponse>;

    private class ACommand : ICommand<AResponse>;

    [UsedImplicitly]
    private class AResponse;
}
