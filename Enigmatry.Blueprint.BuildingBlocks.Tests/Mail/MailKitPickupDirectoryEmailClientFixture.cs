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
        public void TestSendMessage()
        {
            string messageBody = "This is a test message";
            string sender = "sender@enigmatry.com";
            string receiver = "receiver@enigmatry.com";

            EmailMessage message = new EmailMessage(
                "Test message",
                messageBody,
                new List<string>() { receiver },
                new List<string>() { sender });
            _client.Send(message);
            string fullMessage = File.ReadAllText(Directory.GetFiles(TestContext.CurrentContext.TestDirectory, "*.eml").First());

            fullMessage.Should().Contain($"From: {sender}");
            fullMessage.Should().Contain($"To: {receiver}");
            fullMessage.Should().Contain(messageBody);
        }


    }
}
