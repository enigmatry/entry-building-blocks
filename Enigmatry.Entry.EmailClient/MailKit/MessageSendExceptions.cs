using MailKit.Net.Smtp;

namespace Enigmatry.Entry.Email.MailKit;

internal static class MessageSendExceptions
{
    // Exact types: subclasses such as ServiceNotConnectedException, ServiceNotAuthenticatedException and
    // ObjectDisposedException derive from InvalidOperationException but mean the whole session failed.
    public static bool AffectsOnlyThisMessage(Exception exception) =>
        exception is SmtpUtf8AddressException
        || exception.GetType() == typeof(NotSupportedException)
        || exception.GetType() == typeof(InvalidOperationException);
}
