using NUnit.Framework;
using Shouldly;

namespace Enigmatry.Entry.Email.Tests;

[Category("unit")]
public class EmailMessageFixture
{
    [Test]
    public void AssertSingleToAddressPerEmail()
    {
        var originalMessage = new EmailMessage(new[] { "receiver1@enigmatry.com", "receiver2@enigmatry.com" }, "subject", "content");
        var messages = originalMessage.GetBulk();
        messages.Count().ShouldBe(2, "2 receivers in the original message should lead to 2 new EmailMessage objects.");
        foreach (var message in messages)
        {
            message.To.Count.ShouldBe(1, "EmailMessage is allowed to contain only one 'To' email address");
        }
    }

    [Test]
    public void AssertUniqueToAddresses()
    {
        var originalMessage = new EmailMessage(new[] { "receiver1@enigmatry.com", "receiver1@enigmatry.com", "receiver2@enigmatry.com" }, "subject", "content");
        var messages = originalMessage.GetBulk();
        messages.Count().ShouldBe(2, "3 receivers in the original message of which 2 are unique should lead to 2 new EmailMessage objects.");
    }

    [Test]
    public void AssertUniqueCcAddresses()
    {
        var originalMessage = new EmailMessage(new[] { "receiver1@enigmatry.com" }, "subject", "content");
        originalMessage.Cc.AddRange(new[] { "receiver1@enigmatry.com", "receiver2@enigmatry.com" });
        var messages = originalMessage.GetBulk();
        messages.First().Cc.Count.ShouldBe(2, "2 unique CC receivers in the original message should lead to 2 CC receivers.");
    }
}
