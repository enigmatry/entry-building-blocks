using Enigmatry.Entry.Email.MailKit;
using Enigmatry.Entry.Email.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Text;

namespace Enigmatry.Entry.Email.Tests;

[Category("unit")]
public class MailKitPickupDirectoryEmailClientFixture
{
    private IEmailClient _client;

    [SetUp]
    public void Setup()
    {
        var configuration = TestConfigurationBuilder.Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddEntryEmailClient(configuration);
        var provider = services.BuildServiceProvider();
        _client = provider.GetService<IEmailClient>()!;
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
