using Enigmatry.Entry.Email.MailKit;
using NUnit.Framework;
using Shouldly;

namespace Enigmatry.Entry.Email.Tests;

[Category("unit")]
internal sealed class MessageSendExceptionsFixture
{
    [TestCaseSource(typeof(MessageSendExceptionsSource), nameof(MessageSendExceptionsSource.Get))]
    public void AffectsOnlyThisMessageMatchesMessageLevelFailures(Exception exception, bool expected)
    {
        var result = MessageSendExceptions.AffectsOnlyThisMessage(exception);

        result.ShouldBe(expected);
    }
}
