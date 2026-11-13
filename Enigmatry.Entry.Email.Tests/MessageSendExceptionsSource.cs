using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Utils;
using NUnit.Framework;

namespace Enigmatry.Entry.Email.Tests;

internal static class MessageSendExceptionsSource
{
    public static IEnumerable<TestCaseData> Get()
    {
        yield return Case(new SmtpUtf8AddressException(new MailboxAddress("", "user@example.com"), default), true);
        yield return Case(new NotSupportedException(), true);
        yield return Case(new InvalidOperationException(), true);
        yield return Case(new ServiceNotConnectedException(), false);
        yield return Case(new ServiceNotAuthenticatedException(), false);
        yield return Case(new ObjectDisposedException("SmtpClient"), false);
        yield return Case(new IOException(), false);
    }

    private static TestCaseData Case(Exception exception, bool affectsOnlyThisMessage) =>
        new TestCaseData(exception, affectsOnlyThisMessage).SetArgDisplayNames(exception.GetType().Name);
}
