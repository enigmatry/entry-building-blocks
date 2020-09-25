using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Enigmatry.Blueprint.BuildingBlocks.Email;
using Enigmatry.Blueprint.BuildingBlocks.Email.MailKit;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Enigmatry.Blueprint.BuildingBlocks.Tests.Mail
{
    [Category("unit")]
    public class MailKitPickupDirectoryEmailClientFixture
    {
#pragma warning disable CS8618
        private MailKitPickupDirectoryEmailClient _client;
#pragma warning restore CS8618
        private string _pickupFolder = "tempPickup";

        [SetUp]
        public void Setup()
        {
            SmtpSettings settings = new SmtpSettings()
            {
                UsePickupDirectory = true,
                PickupDirectoryLocation = _pickupFolder
            };
            _client = new MailKitPickupDirectoryEmailClient(settings, new NullLogger<MailKitPickupDirectoryEmailClient>());
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
            string fullMessage = File.ReadAllText(Directory.GetFiles(_pickupFolder, "*.eml").First());

            fullMessage.Should().Contain($"From: {sender}");
            fullMessage.Should().Contain($"To: {receiver}");
            fullMessage.Should().Contain(messageBody);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_pickupFolder))
            {
                Directory.Delete(_pickupFolder, true);
            }
        }
    }
}
