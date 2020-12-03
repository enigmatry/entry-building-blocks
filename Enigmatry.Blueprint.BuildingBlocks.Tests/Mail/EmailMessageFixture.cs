using Enigmatry.Blueprint.BuildingBlocks.Email;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;

namespace Enigmatry.Blueprint.BuildingBlocks.Tests.Mail
{
    [Category("unit")]
    public class EmailMessageFixture
    {
        [Test]
        public void AssertSingleToAddressPerEmail()
        {
            var originalMessage = new EmailMessage("subject", "content", new[] { "receiver1@enigmatry.com", "receiver2@enigmatry.com" });
            var messages = originalMessage.GetBulk();
            messages.Should().HaveCount(2, "2 receivers in the original message should lead to 2 new EmailMessage objects.");
            foreach (var message in messages)
            {
                message.To.Should().HaveCount(1, "EmailMessage is allowed to contain only one 'To' email address");
            }
        }

        [Test]
        public void AssertUniqueToAddresses()
        {
            var originalMessage = new EmailMessage("subject", "content", new[] { "receiver1@enigmatry.com", "receiver1@enigmatry.com", "receiver2@enigmatry.com" });
            var messages = originalMessage.GetBulk();
            messages.Should().HaveCount(2, "2 unique receivers in the original message should lead to 2 new EmailMessage objects.");
        }

        [Test]
        public void AssertUniqueCcAddresses()
        {
            var originalMessage = new EmailMessage("subject", "content", new[] { "receiver1@enigmatry.com" });
            originalMessage.SetCc(new[] { "receiver2@enigmatry.com", "receiver2@enigmatry.com" });
            var messages = originalMessage.GetBulk();
            messages.First().Cc.Should().HaveCount(1, "1 unique CC receiver in the original message should lead to 1 CC receiver.");
        }
    }
}
