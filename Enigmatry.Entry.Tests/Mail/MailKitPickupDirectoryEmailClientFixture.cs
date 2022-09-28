using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enigmatry.Entry.Email;
using Enigmatry.Entry.Email.MailKit;
using Enigmatry.Entry.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NUnit.Framework;

namespace Enigmatry.Entry.Tests.Mail
{
    [Category("unit")]
    public class MailKitPickupDirectoryEmailClientFixture
    {
#pragma warning disable CS8618
        private IEmailClient _client;
#pragma warning restore CS8618

        [SetUp]
        public void Setup()
        {
            var configuration = new TestConfigurationBuilder()
                .Build();

            var webHost = WebHost.CreateDefaultBuilder()
                .UseConfiguration(configuration)
                .UseStartup<TestStartup>()
                .Build();

            _client = new DependencyResolverHelper(webHost).GetService<IEmailClient>();
        }

        [Test]
        public void ClientShouldGetResolved() => _client.Should().BeOfType(typeof(MailKitPickupDirectoryEmailClient));

        [Test]
        public async Task TestSendMessage()
        {
            var messageBody = "This is a test message";
            var sender = "sender@enigmatry.com";
            var receiver = "receiver@enigmatry.com";
            var attachment1FileName = "test1.txt";
            var attachment1Contents = "This is a test";
            var attachment2FileName = "test2.txt";
            var attachment2Contents = "This is another test";

            var message = new EmailMessage(
                new List<string> { receiver },
                "Test message",
                messageBody)
            {
                From = new EmailMessageAddress(sender)
            };
            message.Attachments.Add(attachment1FileName, Encoding.ASCII.GetBytes(attachment1Contents));
            message.Attachments.Add(attachment2FileName, Encoding.ASCII.GetBytes(attachment2Contents));
            await _client.SendAsync(message);

            var pathToFile = new DirectoryInfo(TestContext.CurrentContext.TestDirectory).EnumerateFiles("*.eml")
                .OrderByDescending(f => f.LastWriteTime).First().FullName;

            var fullMessage =
                await File.ReadAllTextAsync(pathToFile);

            fullMessage.Should().Contain($"From: {sender}");
            fullMessage.Should().Contain($"To: {receiver}");
            fullMessage.Should().Contain(messageBody);
            fullMessage.Should().Contain(attachment1Contents);
            fullMessage.Should().Contain($"Content-Type: text/plain; name={attachment1FileName}");
            fullMessage.Should().Contain(attachment2Contents);
            fullMessage.Should().Contain($"Content-Disposition: attachment; filename={attachment2FileName}");
        }
    }
}
