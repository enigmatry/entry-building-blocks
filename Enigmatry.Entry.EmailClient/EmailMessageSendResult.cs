namespace Enigmatry.Entry.Email
{
    public class EmailMessageSendResult
    {
        public EmailMessage Message { get; set; } = null!;
        public bool Success { get; set; }
    }
}
