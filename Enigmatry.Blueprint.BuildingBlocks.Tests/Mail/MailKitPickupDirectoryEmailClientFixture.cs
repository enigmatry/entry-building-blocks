using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Enigmatry.Blueprint.BuildingBlocks.Email;
using Enigmatry.Blueprint.BuildingBlocks.Email.MailKit;
using NUnit.Framework;
using Enigmatry.Blueprint.BuildingBlocks.Tests.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Enigmatry.Blueprint.BuildingBlocks.Tests.Mail
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
        public void ClientShouldGetResolved()
        {
            _client.Should().BeOfType(typeof(MailKitPickupDirectoryEmailClient));
        }

        [Test]
        public async Task TestSendMessage()
        {
            var messageBody = "This is a test message";
            var sender = "sender@enigmatry.com";
            var receiver = "receiver@enigmatry.com";

            var message = new EmailMessage(
                "Test message",
                messageBody,
                new List<string>() { receiver })
            {
                From = new MailAddress(sender)
            };
            await _client.SendAsync(message);
            var fullMessage = File.ReadAllText(Directory.GetFiles(TestContext.CurrentContext.TestDirectory, "*.eml").First());

            fullMessage.Should().Contain($"From: {sender}");
            fullMessage.Should().Contain($"To: {receiver}");
            fullMessage.Should().Contain(messageBody);
        }


    }
}
